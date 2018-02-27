// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="DrawMobility.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for rendering the mobility portion of symbols in MIL STD-2525C
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilSymbol
{
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif
    using Schemas;

    /// <summary>
    /// Support methods for rendering the mobility portion of symbols in MIL STD-2525C
    /// </summary>
    internal static class DrawMobility
    {
        /// <summary>
        /// The maximum positive x-coordinate value of many mobility indicators
        /// </summary>
        private const double End = 132.0;

        /// <summary>
        /// Half of the End x-coordinate value
        /// </summary>
        private const double HalfEnd = 0.5 * End;

        /// <summary>
        /// Half of the default line width, i.e, 0.5 * 10.0
        /// </summary>
        private const double HalfWidth = SymbolData.HalfWidth;

        /// <summary>
        /// The radius of the most curved parts making up mobility indicators
        /// </summary>
        private const double Rad = 19.0; // defining radius

        /// <summary>
        /// Another common dimension for most objects, about half the normal height
        /// </summary>
        private const double Sqr = 10;

        /// <summary>
        /// Three times the commmon "Sqr" dimension
        /// </summary>
        private const double ThreeSqr = Sqr + TwoSqr;

        /// <summary>
        /// Twice the default radius for most curved parts making up mobility indicators
        /// </summary>
        private const double TwoRad = 2.0 * Rad; // defining radius

        /// <summary>
        /// Twice the common "Sqr" dimension
        /// </summary>
        private const double TwoSqr = Sqr + Sqr;

        /// <summary>
        /// Generates the wheels for mobility indicators, e.g., wheeled transport.
        /// </summary>
        /// <param name="exes">
        /// The center x-coordinates of the wheels to be generated.
        /// </param>
        /// <param name="y">
        /// The common, center y-coordinate of the wheels to be generated.
        /// </param>
        /// <returns>
        /// A shape that represents the necessary wheels.
        /// </returns>
        public static Shape GenerateWheels(double[] exes, double y)
        {
            var pg = new PathGeometry();
            pg.Figures.Add(Segment(-End - HalfWidth, y, End + HalfWidth, y));
            foreach (double x in exes)
            {
                pg.Figures.Add(Arc(x + 1, y, x - 1, y, SweepDirection.Clockwise));
            }

            return new Path { Style = MilBrush.BlackPresent, Data = pg };
        }

        /// <summary>
        /// Generates the "towed" shape.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate at which to generate the towed shape.
        /// </param>
        /// <returns>
        /// The "towed" shape as a Shape object.
        /// </returns>
        public static Shape GenerateTowed(double y)
        {
            const double X = End - TwoRad;
            return new Path
            {
                Style = MilBrush.BlackPresent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        Segment(-X, y, X, y),
                        Arc(X - HalfWidth, y - 1, X - HalfWidth, y + 1, SweepDirection.Clockwise),
                        Arc(-X + HalfWidth, y + 1, -X + HalfWidth, y - 1, SweepDirection.Clockwise)
                    }
                }
            };
        }

        /// <summary>
        /// Generates the sled shape using the SledGeometry
        /// </summary>
        /// <param name="y">
        /// The y- coordinate that is passed to SledGeometry.
        /// </param>
        /// <returns>
        /// A shape representing the sled geometry.
        /// </returns>
        public static Shape GenerateSled(double y)
        {
            return new Path { Style = MilBrush.BlackPresent, Data = SledGeometry(y) };
        }

        /// <summary>
        /// Generates tracked mobility object by combining SledGeometry with a line segment.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate of the mobility indicator.
        /// </param>
        /// <returns>
        /// A Shape object representing the modified tracked mobility object.
        /// </returns>
        public static Shape GenerateTracked(double y)
        {
            PathGeometry pg = SledGeometry(y);
            pg.Figures.Add(Segment(Rad - End, y, End - Rad, y));
            return new Path { Style = MilBrush.BlackPresent, Data = pg };
        }

        /// <summary>
        /// Generates wheeled, tracked mobility object by combining SledGeometry with a line segment and wheels.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate of the mobility indicator.
        /// </param>
        /// <returns>
        /// A Shape object representing the modified wheeled, tracked mobility object.
        /// </returns>
        public static Shape GenerateWheeledTracked(double y)
        {
            PathGeometry pg = SledGeometry(y);
            pg.Figures.Add(Segment(Rad - End, y, End - Rad, y));
            pg.Figures.Add(Arc(-End - TwoRad, y + TwoRad, -End - TwoRad, y, SweepDirection.Counterclockwise));
            pg.Figures.Add(Arc(-End - TwoRad, y + TwoRad, -End - TwoRad, y, SweepDirection.Clockwise));
            return new Path { Style = MilBrush.BlackPresent, Data = pg };
        }

        /// <summary>
        /// Generates snow mobility object.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate of the mobility indicator.
        /// </param>
        /// <returns>
        /// A Shape object representing the modified snow mobility object.
        /// </returns>
        public static Shape GenerateOverSnow(double y)
        {
            return new Path
            {
                Style = MilBrush.BlackPresent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(End, y),
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment { Point = new Point(TwoRad - End, y) },
                                new LineSegment { Point = new Point(-End, y - TwoRad) }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Generates pack animals mobility object.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate of the mobility indicator.
        /// </param>
        /// <returns>
        /// A Shape object representing the pack animals mobility object.
        /// </returns>
        public static Shape GeneratePackAnimals(double y)
        {
            return new Path
            {
                Style = MilBrush.BlackPresent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(-TwoRad, y + TwoRad),
                            Segments = new PathSegmentCollection
                            {
                                new LineSegment { Point = new Point(-Rad, y) },
                                new LineSegment { Point = new Point(0, y + TwoRad) },
                                new LineSegment { Point = new Point(Rad, y) },
                                new LineSegment { Point = new Point(TwoRad, y + TwoRad) }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Generates barge mobility object.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate of the mobility indicator.
        /// </param>
        /// <returns>
        /// A Shape object representing the barge mobility object.
        /// </returns>
        public static Shape GenerateBarge(double y)
        {
            return new Path
            {
                Style = MilBrush.BlackPresent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        Segment(-End + 10, y, End - 10, y),
                        new PathFigure
                        {
                            StartPoint = new Point(End, y - 10),
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = new Point(-End, y - 10),
                                    IsLargeArc = false,
                                    Size = new Size(200, 200),
                                    SweepDirection = SweepDirection.Clockwise
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Generates amphibious mobility object.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate of the mobility indicator.
        /// </param>
        /// <returns>
        /// A Shape object representing the amphibious mobility object.
        /// </returns>
        public static Shape GenerateAmphibious(double y)
        {
            const double Start = -End - 1;
            var pg = new PathGeometry();
            var pf = new PathFigure { StartPoint = new Point(Start, y + Rad) };
            for (int i = 1; i < 8; i++)
            {
                SweepDirection sd = (i & 1) != 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                pf.Segments.Add(HalfFib(Start + (i * TwoRad), y + Rad, sd));
            }

            pg.Figures.Add(pf);
            return new Path { Style = MilBrush.BlackPresent, Data = pg };
        }

        /// <summary>
        /// Generates generic array mobility object.
        /// </summary>
        /// <param name="exes">
        /// The x-coordinates of the centers of the array objects.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the centers of the array objects.
        /// </param>
        /// <returns>
        /// A Shape representing the generic array mobility object.
        /// </returns>
        public static Shape GenerateArray(double[] exes, double y)
        {
            bool odd = false;
            var pg = new PathGeometry();
            foreach (double x in exes)
            {
                pg.Figures.Add(Block(x, y));
                if (odd)
                {
                    pg.Figures.Add(Segment(x, y - ThreeSqr, x, y + ThreeSqr));
                }

                odd = !odd;
            }

            pg.Figures.Add(Segment(-End - ThreeSqr, y, End + ThreeSqr, y));
            return new Path { Style = MilBrush.BlackPresent, Data = pg };
        }

        /// <summary>
        /// Generates barge mobility object.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code containing the mobility indicator for which to create a mobility Shape.
        /// </param>
        /// <returns>
        /// A Shape object representing the mobility object.
        /// </returns>
        public static Shape GenerateMobility(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            if (!Mobility.IsMobility(symbolCode))
            {
                return null;
            }

            Rect b = SymbolData.GetBounds(symbolCode);
            double bottom = b.Bottom + HalfWidth;
            bool flat = SymbolData.IsBaseFlat(symbolCode);
            const double X = End - Rad;

            switch (Mobility.GetCode(symbolCode))
            {
                case Mobility.WheeledLimited:
                    return GenerateWheels(new[] { -X, X }, bottom);
                case Mobility.WheeledCrossCountry:
                    return GenerateWheels(new[] { -X, 0, X }, bottom);
                case Mobility.Tracked:
                    return GenerateTracked(bottom);
                case Mobility.WheeledTracked:
                    return GenerateWheeledTracked(bottom);
                case Mobility.Towed:
                    if (flat)
                    {
                        bottom += Rad;
                    }

                    return GenerateTowed(bottom);
                case Mobility.Railway:
                    return GenerateWheels(new[] { -X, -X + TwoRad, X - TwoRad, X }, bottom);
                case Mobility.OverSnow:
                    if (flat)
                    {
                        bottom += TwoRad;
                    }

                    return GenerateOverSnow(bottom);
                case Mobility.Sled:
                    if (!flat)
                    {
                        bottom -= TwoRad;
                    }

                    return GenerateSled(bottom);
                case Mobility.PackAnimals:
                    return GeneratePackAnimals(bottom);
                case Mobility.Barge:
                    return GenerateBarge(bottom);
                case Mobility.Amphibious:
                    return GenerateAmphibious(bottom);
                case Mobility.TowedArrayShort:
                    return GenerateArray(new[] { -End - TwoSqr, 0.0, End + TwoSqr }, bottom + ThreeSqr - HalfWidth);
                case Mobility.TowedArrayLong:
                    if (!flat)
                    {
                        bottom -= Sqr + HalfWidth;
                    }

                    return GenerateArray(
                        new[]
                            {
                                -End - TwoSqr, -HalfEnd - Sqr, 0.0, HalfEnd + Sqr, End + TwoSqr
                        },
                        bottom + ThreeSqr - HalfWidth);
            }

            return null;
        }

        /// <summary>
        /// Generates an arc segment to support the amphibious mobility object
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the start of the arc segment.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the start of the arc segment.
        /// </param>
        /// <param name="sd">
        /// The sweep direction of the arc segment.
        /// </param>
        /// <returns>
        /// An ArcSegment representing the arc segment making up the amphibious mobility object.
        /// </returns>
        private static ArcSegment HalfFib(double x, double y, SweepDirection sd)
        {
            return new ArcSegment { Point = new Point(x, y), Size = new Size(Rad, Rad), SweepDirection = sd };
        }

        /// <summary>
        /// Generates the square blocks making up a towed array.
        /// </summary>
        /// <param name="x">
        /// The x-coordinates of the square blocks.
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the square blocks.
        /// </param>
        /// <returns>
        /// A PathFigure representing the square blocks making up the towed array.
        /// </returns>
        private static PathFigure Block(double x, double y)
        {
            return new PathFigure
            {
                StartPoint = new Point(x - Sqr, y - Sqr),
                Segments = new PathSegmentCollection
                {
                    new LineSegment { Point = new Point(x + Sqr, y - Sqr) },
                    new LineSegment { Point = new Point(x + Sqr, y + Sqr) },
                    new LineSegment { Point = new Point(x - Sqr, y + Sqr) },
                    new LineSegment { Point = new Point(x - Sqr, y - Sqr) },
                    new LineSegment { Point = new Point(x + Sqr, y - Sqr) }
                }
            };
        }

        /// <summary>
        /// Helper method for generating a line segment.
        /// </summary>
        /// <param name="x1">
        /// The initial x-coordinate of the line segment.
        /// </param>
        /// <param name="y1">
        /// The initial y-coordinate of the line segment.
        /// </param>
        /// <param name="x2">
        /// The final x-coordinate of the line segment.
        /// </param>
        /// <param name="y2">
        /// The final y-coordinate of the line segment.
        /// </param>
        /// <returns>
        /// A PathFigure representing the requested line segment.
        /// </returns>
        private static PathFigure Segment(double x1, double y1, double x2, double y2)
        {
            return new PathFigure
            {
                StartPoint = new Point(x1, y1),
                Segments = new PathSegmentCollection { new LineSegment { Point = new Point(x2, y2) } }
            };
        }

        /// <summary>
        /// Draw an arc segment of radius Rad between the two indicated points.
        /// </summary>
        /// <param name="x1">
        /// The initial x-coordinate of the arc segment.
        /// </param>
        /// <param name="y1">
        /// The initial y-coordinate of the arc segment.
        /// </param>
        /// <param name="x2">
        /// The final x-coordinate of the arc segment.
        /// </param>
        /// <param name="y2">
        /// The final y-coordinate of the arc segment.
        /// </param>
        /// <param name="sd">
        /// The sweep direction - clockwise or counter-clockwise.
        /// </param>
        /// <returns>
        /// A PathFigure representing the arc segment.
        /// </returns>
        private static PathFigure Arc(double x1, double y1, double x2, double y2, SweepDirection sd)
        {
            return new PathFigure
            {
                StartPoint = new Point(x1, y1),
                Segments = new PathSegmentCollection
                {
                    new ArcSegment
                    {
                        Point = new Point(x2, y2),
                        IsLargeArc = true,
                        Size = new Size(Rad, Rad),
                        SweepDirection = sd
                    }
                }
            };
        }

        /// <summary>
        /// Generates the geometry required by the sled geometry.
        /// </summary>
        /// <param name="y">
        /// The y-coordinate for the sled geometry.
        /// </param>
        /// <returns>
        /// A PathGeometry representing the sled geometry.
        /// </returns>
        private static PathGeometry SledGeometry(double y)
        {
            return new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    Segment(Rad - End, y + TwoRad, End - Rad, y + TwoRad),
                    Arc(End - Rad, y + TwoRad, End - Rad, y, SweepDirection.Counterclockwise),
                    Arc(Rad - End, y + TwoRad, Rad - End, y, SweepDirection.Clockwise)
                }
            };
        }
    }
}