// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilZone.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Generator for most MilGraphic polygons.
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using MilSym.MilGraph.Support;
    using MilSym.MilSymbol;
    using MilSym.MilSymbol.Schemas;

    /// <summary>
    /// Generator for most MilGraphic polygons.
    /// </summary>
    public class MilZone
    {
        /// <summary>
        /// Basically this is the default symbol size, similar to the single point symbol sizes.
        /// </summary>
        private const double DefaultZoneSize = 300.0;

        /// <summary>
        /// Half of the default zone size.
        /// </summary>
        private const double HalfSize = DefaultZoneSize / 2.0;

        /// <summary>
        /// Adds a weapons free zone to the MilGraphic and returns the list of transformed points.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic to receive the weapons free zone.
        /// </param>
        /// <param name="anchors">
        /// The list of map locations making up the zone.
        /// </param>
        /// <returns>
        /// The list of map locations transformed into ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddWeaponsFreeZone(MilGraphic mg, IList<ILocation> anchors)
        {
            IList<Point> points;
            ApplyOffset(mg);    // applies offset to any existing text
            Path path = GenerateLines(mg, true, anchors.Count, anchors, out points);
            path.Fill = HatchBrush(mg.ContentControl.Brush);
            mg.AddChild("Boundary", path);
            var labels = new[]
            {
                mg.LabelT,
                "Time from: " + mg.LabelW,
                "Time to: " + mg.LabelW1
            };
            mg.AddChild("Labels", GenerateLabels(mg, labels));
            return points;
        }

        /// <summary>
        /// Adds an air zone to the MilGraphic and returns the list of transformed points.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic to receive the air zone.
        /// </param>
        /// <param name="anchors">
        /// The list of map locations making up the zone.
        /// </param>
        /// <returns>
        /// The list of map locations transformed into ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddAirZone(MilGraphic mg, IList<ILocation> anchors)
        {
            IList<Point> points;
            ApplyOffset(mg);    // applies offset to any existing text
            mg.AddChild("Boundary", GenerateLines(mg, true, anchors.Count, anchors, out points));
            var labels = new[]
            {
                mg.LabelT,
                "Min Alt: " + mg.LabelX, 
                "Max Alt: " + mg.LabelX1,
                "Time from: " + mg.LabelW,
                "Time to: " + mg.LabelW1
            };
            mg.AddChild("Labels", GenerateLabels(mg, labels));
            return points;
        }

        /// <summary>
        /// Adds a generic zone to the MilGraphic and returns the list of transformed points.
        /// Assumes that LabelT will be used to label the zone, the most common situation.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic to receive the zone.
        /// </param>
        /// <param name="anchors">
        /// The list of map locations making up the zone.
        /// </param>
        /// <returns>
        /// The list of map locations transformed into ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddZone(MilGraphic mg, IList<ILocation> anchors)
        {
            IList<Point> points;
            ApplyOffset(mg);    // applies offset to any existing text
            mg.AddChild("Boundary", GenerateLines(mg, true, anchors.Count, anchors, out points));
            mg.AddChild("LabelT", GenerateLabels(mg, new[] { mg.LabelT }));
            return points;
        }

        /// <summary>
        /// Adds a generic zone with labels to the MilGraphic and returns the list of transformed points.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic to receive the zone and the labels.
        /// </param>
        /// <param name="anchors">
        /// The list of map locations making up the zone.
        /// </param>
        /// <param name="labels">
        /// The labels associated with the zone.
        /// </param>
        /// <returns>
        /// The list of map locations transformed into ResourceDictionary space.
        /// </returns>
        public static IList<Point> AddZone(MilGraphic mg, IList<ILocation> anchors, string[] labels)
        {
            IList<Point> points;
            ApplyOffset(mg);    // applies offset to any existing text
            mg.AddChild("Boundary", GenerateLines(mg, true, anchors.Count, anchors, out points));
            mg.AddChild("Labels", GenerateLabels(mg, labels));
            return points;
        }

        /// <summary>
        /// Transforms a set of points according to a passed in matrix.
        /// </summary>
        /// <param name="m">
        /// The matrix used to transform the points.
        /// </param>
        /// <param name="pts">
        /// The list of points to be transformed, in place.
        /// </param>
        public static void TransformPoints(Matrix m, IList<Point> pts)
        {
            if (pts == null)
            {
                return;
            }

            var count = pts.Count;
            for (var i = 0; i < count; i++)
            {
                pts[i] = m.Transform(pts[i]);
            }
        }

        /// <summary>
        /// Get the matrix needed to transform the passed in points
        /// to the DefaultZoneSize square box. Sometimes we don't
        /// necessarily want to use all the points to find the transform.
        /// </summary>
        /// <param name="count">
        /// The number of points to be considered from the list of points.
        /// </param>
        /// <param name="pts">
        /// The list of points used to define the transform.
        /// </param>
        /// <returns>
        /// The transform needed to convert map locations into ResourceDictionary space.
        /// </returns>
        public static Matrix GetTransform(int count, IList<Point> pts)
        {
            Point min, max;
            FindMinMax(count, pts, out min, out max);

            // Get the mid-points and compute some scale factors
            // The factor DefaultZoneSize sets the rough extent (width and height)
            //  of the symbol which helps control the relative text size.
            var scaleX = (Math.Abs(max.X - min.X) > double.Epsilon) ? DefaultZoneSize / (max.X - min.X) : DefaultZoneSize;
            var scaleY = (Math.Abs(max.Y - min.Y) > double.Epsilon) ? DefaultZoneSize / (max.Y - min.Y) : DefaultZoneSize;

            // To keep the text from looking odd, need to use same scale factor in X and Y
            var scale = (scaleX < scaleY) ? scaleY : scaleX;
            return new Matrix
            {
                M11 = scale,
                M22 = scale,
                OffsetX = -scale * (max.X + min.X) / 2.0,
                OffsetY = -scale * (max.Y + min.Y) / 2.0
            };
        }

        /// <summary>
        /// Finds the extent of the data and scales and translates it about the origin
        /// according to the size of the DefaultZoneSize. This is the ResourceDictionary space.
        /// </summary>
        /// <param name="pointAnchors">
        /// The user-defined points.
        /// </param>
        /// <param name="pointCp1">
        /// The first set of control points.
        /// </param>
        /// <param name="pointCp2">
        /// The second set of control points.
        /// </param>
        public static void ScaleData(
            IList<Point> pointAnchors, IList<Point> pointCp1, IList<Point> pointCp2)
        {
            Matrix m = GetTransform(pointAnchors.Count, pointAnchors);
            TransformPoints(m, pointAnchors);
            TransformPoints(m, pointCp1);
            TransformPoints(m, pointCp2);
        }

        /// <summary>
        /// The main zone generating method.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the zone to be generated.
        /// </param>
        /// <param name="isClosed">
        /// Whether or not the zone outline is to be closed since this method can be used for polylines as well.
        /// </param>
        /// <param name="count">
        /// The count of the map locations making up the zone.
        /// </param>
        /// <param name="anchors">
        /// The map locations making up the zone.
        /// </param>
        /// <param name="pointAnchors">
        /// The locations in ResourceDictionary space.
        /// </param>
        /// <returns>
        /// A Path representing the map locations, to be associated with the MilGraphic. 
        /// </returns>
        public static Path GenerateLines(
            MilGraphic mg,
            bool isClosed,        // do we close the boundary
            int count,            // the number of transformed points to use in boundary
            IList<ILocation> anchors,
            out IList<Point> pointAnchors)
        {
            IList<Point> pointCp1 = null;
            IList<Point> pointCp2 = null;

            pointAnchors = MapHelper.LatLonsToPixels(anchors, 2.0);

            // Need to convert the cardinal spline points into Bezier spline control points
            if (mg.IsSpline)
            {
                ClosedBezierSpline.GetCurveControlPoints(count, pointAnchors, out pointCp1, out pointCp2);
            }

            ScaleData(pointAnchors, pointCp1, pointCp2);

            // Generate Bezier or line segments for insertion into a figure collection
            PathFigureCollection figures;

            if (mg.IsSpline)
            {
                figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            Segments = GenerateBezierSegments(count, pointAnchors, pointCp1, pointCp2, isClosed) 
                        }
                    };
            }
            else
            {
                switch (mg.StencilType)
                {
                    case "ObstacleZone":
                    case "ObstacleBelt":
                        pointAnchors.Add(pointAnchors[0]);
                        figures = MilLine.GenerateDecoratedSegments(
                            mg, DecoratedType.Triangular, false, count + 1, pointAnchors);
                        pointAnchors.RemoveAt(count);
                        break;
                    case "ObstacleRestrictedArea":
                    case "ObstacleFreeArea":
                        pointAnchors.Add(pointAnchors[0]);
                        figures = MilLine.GenerateDecoratedSegments(
                            mg, DecoratedType.Triangular, true, count + 1, pointAnchors);
                        pointAnchors.RemoveAt(count);
                        break;
                    case "Boundaries":
                        figures = MilLine.GenerateDecoratedSegments(mg, DecoratedType.Echelon, true, count, pointAnchors);
                        var labels = new[] { mg.LabelT, Echelon.GetEchelonSymbol(mg.SymbolCode), mg.LabelT1 };
                        int figureCount = figures.Count;
                        for (int i = 0; i < figureCount - 1; i++)
                        {
                            var pls = figures[i].Segments[0] as PolyLineSegment;
                            if (pls == null)
                            {
                                continue;
                            }

                            var pc = pls.Points.Count - 1;
                            if (pc > 0)
                            {
                                mg.AddChild(
                                    "Label" + i,
                                    MilLine.GenerateLabels(
                                        mg, labels, pls.Points[pc], pls.Points[pc - 1], pointAnchors[0], new Point(1.0, 0.5)));
                            }
                        }

                        break;
                    default:
                        figures = new PathFigureCollection
                            {
                                new PathFigure
                                    {
                                        Segments = GeneratePolygonSegments(count, pointAnchors)
                                    }
                            };
                        break;
                }
            }

            // In most cases we have a single figure
            figures[0].StartPoint = pointAnchors[0];
            figures[0].IsClosed = isClosed;

            var path = new Path
            {
                Data = new PathGeometry { Figures = figures },
                Style = mg.ContentControl.UnframedLine
            };

            if (mg.StencilType == "ObstacleRestrictedArea")
            {
                path.Fill = HatchBrush(mg.ContentControl.Brush);
            }

            path.SetBinding(Shape.StrokeThicknessProperty, new Binding("LineThickness") { Source = mg.ContentControl });
            return path;
        }

        /// <summary>
        /// We need a hatch brush for one type of fill area.
        /// </summary>
        /// <param name="inBrush">
        /// The input brush defines the color of the hatch lines.
        /// </param>
        /// <returns>
        /// A hatch brush style.
        /// </returns>
        private static Brush HatchBrush(Brush inBrush)
        {
            // Get a semi-transparent brush for hatch lines
            var solid = inBrush as SolidColorBrush;
            var color = (solid != null) ? solid.Color : Colors.Black;
            color = Color.FromArgb(0x60, color.R, color.G, color.B);

            var brush = new LinearGradientBrush
            {
                MappingMode = BrushMappingMode.Absolute,
                SpreadMethod = GradientSpreadMethod.Repeat,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(3.0, 3.0)
            };
            brush.GradientStops.Add(new GradientStop { Color = color });
            brush.GradientStops.Add(new GradientStop { Color = color, Offset = 0.25 });
            brush.GradientStops.Add(new GradientStop { Color = Colors.Transparent, Offset = 0.25 });
            brush.GradientStops.Add(new GradientStop { Color = Colors.Transparent, Offset = 1 });
            return brush;
        }

        /// <summary>
        /// Set the text labels for the mil air zones, in a text block.
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic associated with the labels.
        /// </param>
        /// <param name="labels">
        /// The label strings.
        /// </param>
        /// <returns>
        /// A TextBlock containing the label strings.
        /// </returns>
        private static TextBlock GenerateLabels(MilGraphic mg, string[] labels)
        {
            var tb = new TextBlock
            {
                Style = SymbolData.GetStyle("BT20"),
                Foreground = mg.ContentControl.Brush
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
            tb.SetValue(Canvas.TopProperty, mg.LabelOffset.Y * HalfSize);
            tb.SetValue(Canvas.LeftProperty, (mg.LabelOffset.X * HalfSize) - (tb.Width / 2.0));

            return tb;
        }

        /// <summary>
        /// Although not called for in the standard, some styles could benefit from the use of splines.
        /// This method creates a set of splines passing through the user-specified coordinates.
        /// These computations are in the ResourceDictionary space.
        /// </summary>
        /// <param name="count">
        /// The number of user-specified vertices.
        /// </param>
        /// <param name="pointAnchors">
        /// The array of user-specified vertices.
        /// </param>
        /// <param name="pointCp1">
        /// The first set of control points.
        /// </param>
        /// <param name="pointCp2">
        /// The second set of control points.
        /// </param>
        /// <param name="isClosed">
        /// Whether or not the curve is closed.
        /// </param>
        /// <returns>
        /// The PathSegmentCollection representing splines passing through user-specified vertices.
        /// </returns>
        private static PathSegmentCollection GenerateBezierSegments(
            int count,
            IList<Point> pointAnchors,
            IList<Point> pointCp1,
            IList<Point> pointCp2,
            bool isClosed)
        {
            var segments = new PathSegmentCollection();
            for (var i = 1; i < count; ++i)
            {
                segments.Add(
                    new BezierSegment
                    {
                        Point1 = pointCp1[i - 1],
                        Point2 = pointCp2[i],
                        Point3 = pointAnchors[i]
                    });
            }

            if (isClosed)
            {
                segments.Add(
                    new BezierSegment
                    {
                        Point1 = pointCp1[pointCp1.Count - 1],
                        Point2 = pointCp2[0],
                        Point3 = pointAnchors[0]
                    });
            }

            return segments;
        }

        /// <summary>
        /// Apply an offset to all the text elements from the ResourceDictionary's template
        /// </summary>
        /// <param name="mg">
        /// The MilGraphic containing the text elements.
        /// </param>
        private static void ApplyOffset(MilGraphic mg)
        {
            var canvas = VisualTreeHelper.GetChild(mg.ContentControl, 0) as Canvas;
            if (canvas == null)
            {
                return;
            }

            foreach (var tb in canvas.Children.OfType<TextBlock>())
            {
                tb.FindTextExtent();
                tb.SetValue(Canvas.TopProperty, (mg.LabelOffset.Y * HalfSize) - tb.Height);
                tb.SetValue(Canvas.LeftProperty, (mg.LabelOffset.X * HalfSize) - (tb.Width / 2.0));
            }
        }

        /// <summary>
        /// Find the bounds of a set of points
        /// </summary>
        /// <param name="count">
        /// The number of points.
        /// </param>
        /// <param name="pts">
        /// The list of points.
        /// </param>
        /// <param name="min">
        /// The minimum x and y coordinates.
        /// </param>
        /// <param name="max">
        /// The maximum x and y coordinates.
        /// </param>
        private static void FindMinMax(int count, IList<Point> pts, out Point min, out Point max)
        {
            min = new Point(double.MaxValue, double.MaxValue);
            max = new Point(-double.MaxValue, -double.MaxValue);
            if (pts == null)
            {
                return;
            }

            // Find the min-max x and y values so we can scale
            for (int i = 0; i < count; i++)
            {
                if (min.X > pts[i].X)
                {
                    min.X = pts[i].X;
                }

                if (min.Y > pts[i].Y)
                {
                    min.Y = pts[i].Y;
                }

                if (max.X < pts[i].X)
                {
                    max.X = pts[i].X;
                }

                if (max.Y < pts[i].Y)
                {
                    max.Y = pts[i].Y;
                }
            }
        }

        /// <summary>
        /// Creates a PathSegmentCollection for a set of user-defined points in ResourceDictionary space.
        /// </summary>
        /// <param name="count">
        /// The vertex count.
        /// </param>
        /// <param name="pointAnchors">
        /// The list of vertices.
        /// </param>
        /// <returns>
        /// The PathSegmentCollection.
        /// </returns>
        private static PathSegmentCollection GeneratePolygonSegments(int count, IList<Point> pointAnchors)
        {
            var segments = new PathSegmentCollection();
            for (var i = 1; i < count; ++i)
            {
                segments.Add(new LineSegment { Point = pointAnchors[i] });
            }

            return segments;
        }
    }
}
