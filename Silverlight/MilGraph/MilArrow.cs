// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilArrow.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support file for drawing the various arrows in Appendix B of MIL STD-2525C
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilGraph
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Support;

    /// <summary>
    /// MilArrow is an internal class, accessed through MilGraphic, that generates the arrows described in Appendix B
    /// of MIL STD 2525-C. The generation is mostly associated with providing the outline for the multi-point part of
    /// the arrow since the header portion is defined through a resource dictionary entry, where possible.
    /// </summary>
    internal class MilArrow
    {
        /// <summary>
        /// AddArrow generates the missing points for an arrow's tail as appropriate for Bing's mercator projection
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic representing the arrow shape.
        /// </param>
        /// <param name="inPoints">
        /// The points from the resource dictionary entry for the particular symbol. 
        /// </param>
        /// <param name="inAnchors">
        /// The points representing the map coordinates of the arrow's head to tail points.
        /// </param>
        /// <returns>
        /// A list of points that represent the head to tail coordinates of the arrow
        /// </returns>
        public static IList<Point> AddArrow(
            MilGraphic mg, IList<Point> inPoints, IList<ILocation> inAnchors)
        {
            ////                  ·N-1
            ////                  |\
            ////      ------------  \ 
            ////      ·1             ·0 
            ////      ------------  /
            ////                  |/ 

            // First point [0] is the head of the arrow. 
            // Second point [1] is the end of the first arrow segment.
            // Subsequent points define additional arrow segment ends.
            // Last point [N - 1] is the top of the arrowhead (different from 2525B).
            // Looks like the height of the arrow body is 1/2 of the height of the arrow head.

            // Convert the points into "pixel" space. To keep a constant arrow 
            // width it makes more sense to do the computations in this space.
            var count = mg.Anchors.Count;
            IList<Point> points = MapHelper.LatLonsToPixels(mg.Anchors, 4.0);

            // This is the projection of the arrowhead onto the arrow baseline 
            var pt = MapHelper.CalculateProjection(points[0], points[1], points[count - 1]);

            // The origin is the first point in this points array
            var origin = points[0];

            // Compute half the width of the arrow
            var distance = 0.5 * PointHelper.Magnitude(pt, points[count - 1]);

            // Let's go ahead and compute the unit vectors in the direction
            // of each vector making up the arrow, starting at the head and
            // moving towards the tail.
            var unitVectors = new Point[count - 2];
            for (int i = 0; i < count - 2; i++)
            {
                unitVectors[i] = MapHelper.Normalize(points[i], points[i + 1]);
            }

            var leftNormal = new Point[count - 2];
            var rightNormal = new Point[count - 2];

            // Now compute the "left" and "right" perpendicular unit vectors
            Point normal;
            for (int i = 0; i < count - 2; i++)
            {
                // The rightNormal vectors cross with the unit vector to give a +1 in Z
                // The leftNormal vectors cross with the unit vector to give a -1 in Z
                normal = new Point(unitVectors[i].Y, -unitVectors[i].X);
                var cross = (normal.X * unitVectors[i].Y) - (normal.Y * unitVectors[i].X);
                leftNormal[i] = (cross < 0.0) ? normal : new Point(-normal.X, -normal.Y);
                rightNormal[i] = (cross < 0.0) ? new Point(-normal.X, -normal.Y) : normal;
            }

            // We're not mitering any of these angles - that's up to the user.
            // So we're bisecting the angle between two vectors and moving
            // out a distance based on the tangent of that angle.
            // This gives us the outside point.
            // So outsidePoint = point[i] + d * outsideVector[i-1] + d / tan(<) * unitVector[i-1]
            // where < is acos(dot(unitVector[i-1], unitVector[i])).
            //                                    
            //                         ______. d * tan(<)
            //                        d|<
            //        i-1------->-------+i
            //                          \
            //                           \
            //                             i+1

            // Need to flip the starting points if Aviation, Airborne or RotaryWing
            bool flip = mg.StencilType == "AxisOfAdvanceAviation" ||
                        mg.StencilType == "AxisOfAdvanceAirborne" ||
                        mg.StencilType == "AxisOfAdvanceRotaryWing";
            var fwd = PointHelper.LinearCombination(pt, distance, leftNormal[0]);
            var bck = PointHelper.LinearCombination(pt, distance, rightNormal[0]);

            // Start the arrow's body - first point defined by the top of the arrowhead
            var arrow = new List<Point> { flip ? bck : fwd };
            var backArrow = new List<Point> { flip ? fwd : bck };

            for (int i = 0; i < count - 2; i++)
            {
                // Get bisected angle
                // Use the half-angle tangent formula here instead of the brute force method
                // var angle = Math.Atan2(
                //    unitVectors[i].Y * unitVectors[i + 1].X - unitVectors[i].X * unitVectors[i + 1].Y,
                //    unitVectors[i].X * unitVectors[i + 1].X + unitVectors[i].Y * unitVectors[i + 1].Y) / 2.0;
                // var tangent = Math.Tan(angle);
                var tangent = (i == count - 3) ? 0.0 :
                    ((unitVectors[i].Y * unitVectors[i + 1].X) - (unitVectors[i].X * unitVectors[i + 1].Y)) /
                    ((unitVectors[i].X * unitVectors[i + 1].X) + (unitVectors[i].Y * unitVectors[i + 1].Y) + 1.0);
                
                // points[i+1] + distance * (leftNormal[i] + tangent * unitVectors[i])
                arrow.Add(
                    PointHelper.LinearCombination(
                        points[i + 1], distance, PointHelper.LinearCombination(leftNormal[i], tangent, unitVectors[i])));

                // points[i+1] + distance * (rightNormal[i] - tangent * unitVectors[i])
                backArrow.Add(
                    PointHelper.LinearCombination(
                        points[i + 1], distance, PointHelper.LinearCombination(rightNormal[i], -tangent, unitVectors[i])));
            }

            points.Clear();

            count = backArrow.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                arrow.Add(backArrow[i]);
            }

            // Add the origin and the projected arrowhead point to the list of arrow points.
            // We need to get these points transformed and this
            // is currently the only way to do it.
            // We'll have to remove them later from the list.
            arrow.Add(origin);
            arrow.Add(pt);

            mg.IsSpline = false;
            IList<ILocation> anchors = arrow.Select(a => MapHelper.PixelToLatLon(a, 4.0)).ToList();

            Path path = MilZone.GenerateLines(mg, false, anchors.Count - 2, anchors, out points);

            // OK, let's record the transformed origin and kill these extra points.
            int index = points.Count - 1;
            pt = points[index];
            points.RemoveAt(index);
            anchors.RemoveAt(index);
            index--;
            origin = points[index];
            points.RemoveAt(index);
            anchors.RemoveAt(index);

            double newScaleFactor;
            MilGraphic.FullTransformation(path, null, points, anchors, origin, out newScaleFactor);

            // We also need the oldScaleFactor - the scaleFactor that the rest of the graphic will be using. 
            double oldScaleFactor;
            MilGraphic.ComputeScales(MapHelper.ComputeTransform(inPoints, inAnchors), out oldScaleFactor);

            // OK now we need to scale the transform matrix relative to the oldScaleFactor
            var r = newScaleFactor / oldScaleFactor;
            SetPathMatrix(r, path);
            mg.AddChild("Boundary", path);
            if (mg.StencilType == "AxisOfAdvanceForFeint")
            {
                // We need to add LabelT but the position (right after the projection
                // of the arrow head onto the main arrow axis) needs to be scaled according 
                // to our transformation which uses the "r" scale factor.
                var pointNew = PointHelper.LinearCombination(origin, r, PointHelper.Minus(pt, origin));
                mg.AddChild(
                    "Label",
                    MilLine.GenerateLabels(mg, new[] { mg.LabelT }, pointNew, origin, origin, new Point(1.0, 0.5)));
            }
            else if (mg.StencilType == "AxisOfAdvanceRotaryWing")
            {
                // Add rotary wing symbol by appending to existing path
                AddRotaryWing(path, points);
            }

            return points;
        }

        /// <summary>
        /// Scales every element of the Path's PathGeometry transform matrix by the value "r".
        /// </summary>
        /// <param name="r">
        /// The amount by which to scale the Path's PathGeometry transform matrix.
        /// </param>
        /// <param name="path">
        /// The path containing the transform matrix to be scaled.
        /// </param>
        private static void SetPathMatrix(double r, Path path)
        {
            var pg = path.Data as PathGeometry;
            if (pg != null)
            {
                var mt = pg.Transform as MatrixTransform;
                if (mt != null)
                {
                    var m = mt.Matrix;
                    mt.Matrix = new Matrix(m.M11 * r, m.M12 * r, m.M21 * r, m.M22 * r, m.OffsetX * r, m.OffsetY * r);
                }
            }
        }

        /// <summary>
        /// Creates a PathFigure from the passed-in array of points - "pts"
        /// </summary>
        /// <param name="pts">
        /// The array of points to use when generating the PathFigure.
        /// </param>
        /// <returns>
        /// The PathFigure representing the array of points.
        /// </returns>
        private static PathFigure Polyline(params Point[] pts)
        {
            if (pts == null)
            {
                return null;
            }

            int count = pts.Count();
            if (count < 2)
            {
                return null;
            }

            var pf = new PathFigure { StartPoint = pts[0], Segments = new PathSegmentCollection() };
            for (int i = 1; i < count; i++)
            {
                pf.Segments.Add(new LineSegment { Point = pts[i] });
            }

            return pf;
        }

        /// <summary>
        /// AddRotaryWing deals with the messiness that a rotary wing introduces such as the crossing
        /// of the arrow boundary lines and the insertion of the rotary symbol.
        /// This is a brute force method which really knocks up the code count.
        /// Should revisit when all else is done.
        /// </summary>
        /// <param name="path">
        /// The Path element that represents the rotary wing.
        /// </param>
        /// <param name="pts">
        /// The array of points representing the head to tail rotary wing arrow vertices.
        /// </param>
        private static void AddRotaryWing(Path path, IList<Point> pts)
        {
            // We need to find the intersection of the first and last lines of the arrow.
            // Find alpha or beta to make the two lines intersect. We can dot the following
            // equation with the normal to the vector from pts[0] to pts[1].
            //
            // pts[0] + alpha * (pts[1] - pts[0]) = pts[Count-2] + beta * (pts[Count-1] - pts[Count-2])
            int count = pts.Count;
            var normal = PointHelper.LeftNormal(pts[0], pts[1]);                // vector perpendicular to one arrow boundary
            var boundary = PointHelper.Vector(pts[count - 2], pts[count - 1]);    // vector parallel to opposite arrow boundary
            var axis = PointHelper.Vector(pts[count - 2], pts[0]);                // vector parallel to arrow axis
            double beta = PointHelper.DotProduct(axis, normal) /
                          PointHelper.DotProduct(boundary, normal);
            var intersect = PointHelper.LinearCombination(pts[count - 2], beta, boundary);    // intersection of arrow boundaries

            var topNormal = PointHelper.LeftUnitNormal(pts[0], pts[count - 2]);            // unit vector perpendicular to arrow axis
            var bottomNormal = PointHelper.RightUnitNormal(pts[0], pts[count - 2]);        // other perpendicular unit vector

            // Y is smaller at the top in this coordinate system
            if (topNormal.Y > bottomNormal.Y)
            {
                var temp = topNormal;
                topNormal = bottomNormal;
                bottomNormal = temp;
            }

            double distance = PointHelper.Magnitude(pts[0], pts[pts.Count - 1]) / 2.0;        // half the arrow width
            var top = PointHelper.LinearCombination(intersect, distance, topNormal);
            var bottom = PointHelper.LinearCombination(intersect, distance, bottomNormal);

            // Compute the base of the rotary wing symbol
            var unitAxis = PointHelper.UnitVector(axis);
            var startBase = PointHelper.LinearCombination(bottom, 0.25 * distance, unitAxis);
            var endBase = PointHelper.LinearCombination(bottom, -0.25 * distance, unitAxis);

            // Compute the tip of the rotary wing symbol
            var t = PointHelper.LinearCombination(intersect, 0.75 * distance, topNormal);
            var startTip = PointHelper.LinearCombination(t, 0.25 * distance, unitAxis);
            var endTip = PointHelper.LinearCombination(t, -0.25 * distance, unitAxis);

            // Compute the sides of the rotary wing symbol
            var bnA = PointHelper.UnitVector(pts[0], pts[1]);
            var bnB = PointHelper.UnitVector(pts[count - 1], pts[count - 2]);
            distance = PointHelper.Magnitude(pts[0], pts[1]);
            var sideOneA = PointHelper.LinearCombination(intersect, 0.1 * distance, bnA);
            var sideOneB = PointHelper.LinearCombination(intersect, 0.1 * distance, bnB);
            var sideTwoA = PointHelper.LinearCombination(intersect, -0.1 * distance, bnA);
            var sideTwoB = PointHelper.LinearCombination(intersect, -0.1 * distance, bnB);

            PathFigureCollection pfc = ((PathGeometry)path.Data).Figures;
            IEnumerable<PathFigure> pfs = new List<PathFigure>
            {
                Polyline(top, bottom), 
                Polyline(startBase, endBase),
                Polyline(startTip, top, endTip),
                Polyline(sideOneA, sideOneB),
                Polyline(sideTwoA, sideTwoB)
            };
            foreach (var p in pfs)
            {
                pfc.Add(p);
            }
        }
    }
}
