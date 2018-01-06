// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilLine.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Generator for most MilGraphic multi-point lines.
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
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using MilSym.MilGraph.Support;
    using MilSym.MilSymbol;

    /// <summary>
    /// Enumerated type to describe the decoration of line segments in a polyline.
    /// </summary>
    public enum DecoratedType
    {
        /// <summary>
        /// Typical of obstacle line
        /// </summary>
        Triangular,
       
        /// <summary>
        /// Typical of fortified area
        /// </summary>
        Square,
        
        /// <summary>
        /// Typical of anti-tank ditch under construction
        /// </summary>
        Saw,
        
        /// <summary>
        /// Typical of anti-tank ditch anti-mine
        /// </summary>
        SolidTriangular,
        
        /// <summary>
        /// Space for an echelon marking
        /// </summary>
        Echelon
    }

    /// <summary>
    /// Generator for most MilGraphic multi-point lines
    /// </summary>
    public class MilLine
    {
        /// <summary>
        /// Used to define feature sizes
        /// </summary>
        private const double ArcSize = 30.0;    // empirical

        /// <summary>
        /// Used to define feature sizes
        /// </summary>
        private const double ArcScale = 1.15;    // empirical

        /// <summary>
        /// Half of PI
        /// </summary>
        private const double HalfPi = Math.PI / 2.0;

        /// <summary>
        /// Used to ensure unique part names
        /// </summary>
        private static int counter;
        
        /// <summary>
        /// A line generator method to be called by methods that need various decorated line styles
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the line style.
        /// </param>
        /// <param name="anchors">
        /// The list of map locations associated with the polyline.
        /// </param>
        /// <param name="points">
        /// The list of ResourceDirectory space points associated with the polyline (the space in which the decoration occurs).
        /// </param>
        /// <returns>
        /// The path that represents the decorated polyline
        /// </returns>
        public delegate Path GenerateLines(MilGraphic mg, IList<ILocation> anchors, out IList<Point> points);
      
        /// <summary>
        /// Generate a collection of decorated segments along a polyline
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic to receive the decorated line segments.
        /// </param>
        /// <param name="type">
        /// The type of decoration from the enumerated values.
        /// </param>
        /// <param name="drawInside">
        /// Whether to draw the features on the "interior" of the figure.
        /// </param>
        /// <param name="count">
        /// The vertex count.
        /// </param>
        /// <param name="pts">
        /// The array of vertices.
        /// </param>
        /// <returns>
        /// A graphic object representing the decorated lines.
        /// </returns>
        public static PathFigureCollection GenerateDecoratedSegments(
            MilGraphic mg, DecoratedType type, bool drawInside, int count, IList<Point> pts)
        {
            // Most of the time we'll have a single PathFigure consisting 
            // of a single PolyLineSegment.
            // But occasionally the PathFigure will have more segments
            // and occasionally there will be more than one path figure.
            var pfc = new PathFigureCollection();

            // Collect the points we're passing through
            var ptsLoc = new PointCollection();

            // In many cases there will be only one polyline segment
            // but sometimes we'll have multiple segments which may, 
            // or may not require multiple paths.
            var segments = new PathSegmentCollection();

            double totalHypot;
            var hypots = CalculateHypots(count, pts, out totalHypot);
            var arcSize = 0.5 * (totalHypot / Math.Max(1, (int)(totalHypot / (2.0 * ArcSize))));

            bool odd = true;        // switch between even and odd

            int totalCount = 0;

            var leftOver = 0.0;
            
            // Break each segment up into parts
            for (var i = 1; i < count; ++i)
            {
                // Get the angle of the line segment
                Point para = MapHelper.Normalize(pts[i - 1], pts[i]);
                var perp = new Point(-para.Y, para.X);
                Point outside = PointHelper.Add(para, perp);
                if (drawInside != PointHelper.PointInPolygon(outside, count - 1, pts))
                {
                    perp = new Point(para.Y, -para.X);
                }

                // Need to round here to make sure we get last segment
                var segNum = (int)Math.Floor((hypots[i] + leftOver + 0.5) / arcSize);

                // Advance along the line segment using the arcSize
                var nextPt = pts[i - 1];
                Point lastPt = nextPt;
                for (var j = 0; j < segNum; j++)
                {
                    var stepSize = (j == 0) ? arcSize - leftOver : arcSize;
                    nextPt = PointHelper.LinearCombination(nextPt, stepSize, para);
                    if (type == DecoratedType.Saw)
                    {
                        var mid = PointHelper.MidPoint(nextPt, lastPt);
                        ptsLoc.Add(PointHelper.LinearCombination(mid, stepSize, perp));
                    }

                    if (odd)
                    {
                        // Add the mines to the anti-tank ditch
                        if (type == DecoratedType.SolidTriangular)
                        {
                            var r = stepSize / 3.0;
                            var p1 = PointHelper.MidPoint(nextPt, lastPt);
                            var p2 = PointHelper.LinearCombination(p1, r, perp);
                            var p3 = PointHelper.LinearCombination(p2, 2.0 * r, perp);
                            var size = new Size(r, r);
                            ptsLoc.Add(p1);
                            ptsLoc.Add(p2);
                            segments.Add(new PolyLineSegment { Points = ptsLoc });
                            segments.Add(new ArcSegment { Point = p3, Size = size });
                            segments.Add(new ArcSegment { Point = p2, Size = size });
                            ptsLoc = new PointCollection { p1 };
                        }
                    }
                    else
                    {
                        if (type == DecoratedType.Triangular ||
                            type == DecoratedType.SolidTriangular)
                        {
                            var mid = PointHelper.MidPoint(nextPt, lastPt);
                            ptsLoc.Add(PointHelper.LinearCombination(mid, stepSize, perp));
                        }
                        else if (type == DecoratedType.Square)
                        {
                            ptsLoc.Add(PointHelper.LinearCombination(lastPt, stepSize, perp));
                            ptsLoc.Add(PointHelper.LinearCombination(nextPt, stepSize, perp));
                        }
                    }

                    if (type == DecoratedType.Echelon)
                    {
                        // Skip every fifth line segment - approximately.
                        // Ignore the fixups at the vertices.
                        if ((++totalCount % 5) == 0)
                        {
                            segments.Add(new PolyLineSegment { Points = ptsLoc });
                            pfc.Add(new PathFigure { Segments = segments, StartPoint = ptsLoc[0] });
                            ptsLoc = new PointCollection();
                            segments = new PathSegmentCollection();
                        }

                        odd = false;    // force odd to always be true - picks up every vertex
                    }

                    ptsLoc.Add(nextPt);
                    odd = !odd;
                    lastPt = nextPt;
                }

                if (odd && PointHelper.Magnitude(nextPt, pts[i]) > 1.0)
                {
                    ptsLoc.Add(pts[i]);
                }

                leftOver = hypots[i] - MapHelper.Hypoteneuse(nextPt, pts[i - 1]);
            }

            if (type == DecoratedType.Saw ||
                type == DecoratedType.SolidTriangular)
            {
                for (int i = count - 2; i >= 0; i--)
                {
                    ptsLoc.Add(pts[i]);
                }
            }

            if (ptsLoc.Count > 0)
            {
                segments.Add(new PolyLineSegment { Points = ptsLoc });
            }

            if (segments.Count > 0)
            {
                var pf = new PathFigure { Segments = segments };

                // For the echelon type we need to set the start point of this segment
                if (type == DecoratedType.Echelon)
                {
                    pf.StartPoint = ptsLoc[0];
                }

                pfc.Add(pf);
            }

            return pfc;
        }

        /// <summary>
        /// Generate a label centered on a line segment.
        /// </summary>
        /// <param name="mg">
        /// The mg.
        /// </param>
        /// <param name="labels">
        /// The labels.
        /// </param>
        /// <param name="pointStart">
        /// The point start.
        /// </param>
        /// <param name="pointEnd">
        /// The point end.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <returns>
        /// A label (as a TextBlock) to be added to the graphic.
        /// </returns>
        public static TextBlock GenerateCenteredLabel(
            MilGraphic mg, string[] labels, Point pointStart, Point pointEnd, Point origin)
        {
            bool leftSide;
            MatrixTransform mt = GenerateLabelTransform(pointStart, pointEnd, origin, out leftSide);
            var tb = new TextBlock
            {
                Style = SymbolData.GetStyle("BT20"),
                RenderTransform = mt,
                Foreground = mg.ContentControl.Brush,
                Name = "Skip" + counter++
            };

            var count = labels.Length;
            for (int i = 0; i < count; i++)
            {
                tb.Inlines.Add(new Run { Text = labels[i] });
                if (i < count - 1)
                {
                    tb.Inlines.Add(new LineBreak());
                }
            }

            tb.FindTextExtent();
            double left = leftSide ? tb.Width : 0.0;
            
            // if (left != 0.0)
            if (Math.Abs(left) > double.Epsilon)   
            {
                tb.TextAlignment = TextAlignment.Right;
            }

            Point p = mt.Matrix.Rotate(new Point(tb.Height / 2.0, left));
            tb.SetValue(Canvas.TopProperty, pointStart.Y - p.X);
            tb.SetValue(Canvas.LeftProperty, pointStart.X - p.Y);
            return tb;
        }

        /// <summary>
        /// Set the text labels for the mil lines, in a text block
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the labels.
        /// </param>
        /// <param name="labels">
        /// The label strings.
        /// </param>
        /// <param name="pointStart">
        /// The starting point of the line to label.
        /// </param>
        /// <param name="pointEnd">
        /// The ending point of the line to label.
        /// </param>
        /// <param name="origin">
        /// The origin coordinate for the object as a whole.
        /// </param>
        /// <param name="offset">
        /// The multiplicative offset for the object as a whole.
        /// </param>
        /// <returns>
        /// The TextBlock representing the labels.
        /// </returns>
        public static TextBlock GenerateLabels(
            MilGraphic mg, string[] labels, Point pointStart, Point pointEnd, Point origin, Point offset)
        {
            bool leftSide;
            MatrixTransform mt = GenerateLabelTransform(pointStart, pointEnd, origin, out leftSide);
            var tb = new TextBlock
            {
                Style = SymbolData.GetStyle("BT20"),
                RenderTransform = mt,
                Foreground = mg.ContentControl.Brush,
                Name = "Skip" + counter++
            };

            var count = labels.Length;
            for (int i = 0; i < count; i++)
            {
                tb.Inlines.Add(new Run { Text = labels[i] });
                if (i < count - 1)
                {
                    tb.Inlines.Add(new LineBreak()); 
                }
            }

            tb.FindTextExtent();
            double left;

            // if (offset.Y == 0.5)
            if (Math.Abs(offset.Y - 0.5) <= double.Epsilon)
            {
                left = leftSide ? tb.Width : 0.0;
            }
            else
            {
                left = leftSide ? 0.0 : tb.Width;
            }

            //// if (left != 0)
            ////if (Math.Abs(left) > double.Epsilon)
            ////{
            ////    tb.TextAlignment = TextAlignment.Right;
            ////}

            ////Point p = mt.Matrix.Rotate(new Point(tb.Height / 2.0, left));
            Point p = mt.Matrix.Rotate(new Point(offset.Y * tb.Height, offset.X * left));
            tb.SetValue(Canvas.TopProperty, pointStart.Y - p.X);
            tb.SetValue(Canvas.LeftProperty, pointStart.X - p.Y);
            return tb;
        }

        /// <summary>
        /// Generates phase lines for the associated MilGraphic.
        /// </summary>
        /// <param name="mg">
        /// The associated MilGraphic.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the vertices.
        /// </param>
        /// <param name="header">
        /// The labels.
        /// </param>
        /// <returns>
        /// The map locations converted to the ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddPhaseLineType(MilGraphic mg, IList<ILocation> anchors, string header)
        {
            return AddLines(mg, GeneratePhaseLine, new[] { header, "(PL " + mg.LabelT + ")" }, anchors);
        }

        /// <summary>
        /// Generates boundary lines for the associated MilGraphic.
        /// </summary>
        /// <param name="mg">
        /// The associated MilGraphic.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the vertices.
        /// </param>
        /// <returns>
        /// The map locations converted to the ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddBoundary(MilGraphic mg, IList<ILocation> anchors)
        {
            return AddLines(mg, GenerateBoundaryLine, null, anchors);
        }

        /// <summary>
        /// Generates phase lines for the associated MilGraphic.
        /// </summary>
        /// <param name="mg">
        /// The associated MilGraphic.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the vertices.
        /// </param>
        /// <returns>
        /// The map locations converted to the ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddPhaseLine(MilGraphic mg, IList<ILocation> anchors)
        {
            return AddLines(mg, GeneratePhaseLine, new[] { "PL " + mg.LabelT }, anchors);
        }

        /// <summary>
        /// Generates obstacle lines for the associated MilGraphic.
        /// </summary>
        /// <param name="mg">
        /// The associated MilGraphic.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the vertices.
        /// </param>
        /// <returns>
        /// The map locations converted to the ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddObstacles(MilGraphic mg, IList<ILocation> anchors)
        {
            return AddLines(mg, GenerateFlot, null, anchors);
        }

        /// <summary>
        /// Generates forward line of own troops for the associated MilGraphic.
        /// </summary>
        /// <param name="mg">
        /// The associated MilGraphic.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the vertices.
        /// </param>
        /// <returns>
        /// The map locations converted to the ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddFlot(MilGraphic mg, IList<ILocation> anchors)
        {
            return AddLines(mg, GenerateFlot, new[] { "FLOT" }, anchors);
        }

        /// <summary>
        /// A generic polyline drawing method with labels at either end of end
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic for the associated polyline.
        /// </param>
        /// <param name="gl">
        /// The actual line generator to call.
        /// </param>
        /// <param name="labels">
        /// The labels for the line ends.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the vertices.
        /// </param>
        /// <returns>
        /// The map locations converted to the ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddLines(
            MilGraphic mg, GenerateLines gl, string[] labels, IList<ILocation> anchors)
        {
            IList<Point> points;
            mg.AddChild("Boundary", gl(mg, anchors, out points));

            if (labels != null)
            {
                var c = points.Count;
                Point origin = points[0];

                // Offset = new Point(1.0, 0.5)
                mg.AddChild("StartLabel", GenerateLabels(mg, labels, points[0], points[1], origin, new Point(1.0, 0.5)));
                mg.AddChild("EndLabel", GenerateLabels(mg, labels, points[c - 1], points[c - 2], origin, new Point(1.0, 0.5)));
            }

            return points;
        }

        /// <summary>
        /// Generates boundary lines.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the boundary lines.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the polyline vertices.
        /// </param>
        /// <param name="points">
        /// The map locations converted to the ResourceDictionary space.
        /// </param>
        /// <returns>
        /// The graphical Path associated with the boundary lines.
        /// </returns>
        public static Path GenerateBoundaryLine(
            MilGraphic mg,
            IList<ILocation> anchors,
            out IList<Point> points)
        {
            return MilZone.GenerateLines(mg, false, anchors.Count, anchors, out points);
        }

        /// <summary>
        /// Generates phase lines.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the phase lines.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the polyline vertices.
        /// </param>
        /// <param name="points">
        /// The map locations converted to the ResourceDictionary space.
        /// </param>
        /// <returns>
        /// The graphical Path associated with the phase lines.
        /// </returns>
        public static Path GeneratePhaseLine(
            MilGraphic mg,
            IList<ILocation> anchors,
            out IList<Point> points)
        {
            return MilZone.GenerateLines(mg, false, anchors.Count, anchors, out points);
        }

        /// <summary>
        /// Generates multiple types of decorated lines.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the decorated lines.
        /// </param>
        /// <param name="anchors">
        /// The map locations of the polyline vertices.
        /// </param>
        /// <param name="points">
        /// The map locations converted to the ResourceDictionary space.
        /// </param>
        /// <returns>
        /// The graphical Path associated with the decorated lines.
        /// </returns>
        public static Path GenerateFlot(
            MilGraphic mg,
            IList<ILocation> anchors,
            out IList<Point> points)
        {
            // Convert the points into "pixel" space. To keep a constant arrow 
            // width it makes more sense to do the computations in this space.
            var count = anchors.Count;
            points = MapHelper.LatLonsToPixels(anchors, 4.0);

            // Convert everything into a Cartesian space about [-300, 300] in each direction
            MilZone.ScaleData(points, null, null);

            // Now generate a collection of arcs along these line segments
            var style = mg.ContentControl.UnframedLine;

            PathFigureCollection figures;
            if (mg.StencilType == "FortifiedArea")
            {
                points.Add(points[0]);
                figures = GenerateDecoratedSegments(mg, DecoratedType.Square, false, count + 1, points);
                points.RemoveAt(count);
            }
            else if (mg.StencilType == "ObstacleLine")
            {
                figures = GenerateDecoratedSegments(mg, DecoratedType.Triangular, false, count, points);
            }
            else if (mg.StencilType == "AntiTankDitchUnderConstruction")
            {
                figures = GenerateDecoratedSegments(mg, DecoratedType.Saw, false, count, points);
            }
            else if (mg.StencilType == "AntiTankDitchComplete")
            {
                figures = GenerateDecoratedSegments(mg, DecoratedType.Saw, false, count, points);
                style = mg.ContentControl.UnframedLineFill;
            }
            else if (mg.StencilType == "AntiTankDitchAntiMine")
            {
                figures = GenerateDecoratedSegments(mg, DecoratedType.SolidTriangular, false, count, points);
                style = mg.ContentControl.UnframedLineFill;
            }
            else
            {
                figures = new PathFigureCollection { new PathFigure { Segments = GenerateArcSegments(count, points) } };
            }

            // Set the start point for the first figure (usually there is only one figure)
            figures[0].StartPoint = points[0];
            var path = new Path
            {
                Data = new PathGeometry { Figures = figures },
                Style = style
            };

            path.SetBinding(Shape.StrokeThicknessProperty, new Binding("LineThickness") { Source = mg.ContentControl });
            return path;
        }

        /// <summary>
        /// Generate a matrix transform (rotation + offset) based on two points (rotation angle) and an origin (offset).
        /// </summary>
        /// <param name="pointStart">
        /// The starting point for defining the rotation angle.
        /// </param>
        /// <param name="pointEnd">
        /// The ending point for defining the rotation angle.
        /// </param>
        /// <param name="origin">
        /// The origin for defining the offset.
        /// </param>
        /// <param name="leftSide">
        /// A boolean indicating if the rotation angle is on the left side of the coordinate plane.
        /// </param>
        /// <returns>
        /// The generated MatrixTransform.
        /// </returns>
        private static MatrixTransform GenerateLabelTransform(Point pointStart, Point pointEnd, Point origin, out bool leftSide)
        {
            var angle = Math.Atan2(pointStart.Y - pointEnd.Y, pointStart.X - pointEnd.X);
            var ca = Math.Cos(angle);
            var sa = Math.Sin(angle);

            leftSide = angle <= -HalfPi || HalfPi <= angle;
            if (leftSide)
            {
                ca = -ca;
                sa = -sa;
            }

            var mt = new MatrixTransform
            {
                Matrix = new Matrix
                {
                    M11 = ca,
                    M12 = sa,
                    M21 = -sa,
                    M22 = ca,
                    OffsetX = -origin.X,
                    OffsetY = -origin.Y
                }
            };
            return mt;
        }
        
        /// <summary>
        /// Computes the segment lengths and total length around a polyline/polygon
        /// </summary>
        /// <param name="count">
        /// The line segment count.
        /// </param>
        /// <param name="pts">
        /// The points making up the polyline/polygon.
        /// </param>
        /// <param name="totalHypot">
        /// The total perimeter value.
        /// </param>
        /// <returns>
        /// The length of each line segment in a double array.
        /// </returns>
        private static double[] CalculateHypots(int count, IList<Point> pts, out double totalHypot)
        {
            totalHypot = 0.0;
            var hypots = new double[count];
            for (var i = 1; i < count; i++)
            {
                hypots[i] = MapHelper.Hypoteneuse(pts[i], pts[i - 1]);
                totalHypot += hypots[i];
            }

            return hypots;
        }

        /// <summary>
        /// Generate a collection of arc segments along a polyline
        /// </summary>
        /// <param name="count">
        /// The vertex count.
        /// </param>
        /// <param name="pts">
        /// The list of vertices.
        /// </param>
        /// <returns>
        /// The graphical representation of the arc segments along the polyline
        /// </returns>
        private static PathSegmentCollection GenerateArcSegments(int count, IList<Point> pts)
        {
            var segments = new PathSegmentCollection();
            double totalHypot;
            var hypots = CalculateHypots(count, pts, out totalHypot);
            var arcSize = totalHypot / Math.Max(1, (int)(totalHypot / ArcSize));
            var halfSize = arcSize / 2.0;

            var leftOver = 0.0;

            // Break each segment up into parts
            for (var i = 1; i < count; ++i)
            {
                // Get the angle of the line segment
                double angle = Math.Atan2(pts[i].Y - pts[i - 1].Y, pts[i].X - pts[i - 1].X);

                // Need to round here to make sure we get last segment
                var segNum = (int)Math.Floor((hypots[i] + leftOver + 0.5) / arcSize);

                // Advance along the line segment using the arcSize
                var nextPt = pts[i - 1];
                for (var j = 0; j < segNum; j++)
                {
                    var stepSize = (j == 0) ? arcSize - leftOver : arcSize;

                    nextPt = new Point(
                        nextPt.X + (stepSize * Math.Cos(angle)),
                        nextPt.Y + (stepSize * Math.Sin(angle)));
                    segments.Add(
                        new ArcSegment
                        {
                            // ArcScale is empirical
                            Size = new Size(ArcScale * halfSize, ArcScale * halfSize),
                            Point = nextPt,
                            SweepDirection = SweepDirection.Clockwise
                        });
                }

                leftOver = hypots[i] - MapHelper.Hypoteneuse(nextPt, pts[i - 1]);
            }

            return segments;
        }
    }
}
