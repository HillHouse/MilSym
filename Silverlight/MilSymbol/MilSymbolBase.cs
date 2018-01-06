// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilSymbolBase.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   The core code for identifying the right base symbol for a symbol defined by MIL STD-2525C
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
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Schemas;

    /// <summary>
    /// The core code for identifying the right base symbol for a symbol defined by MIL-STD 2525C
    /// </summary>
    public class MilSymbolBase : ContentControl, INotifyPropertyChanged
    {
        // These styles are are readonly. They are for the bindings in the resource dictionary.

        /// <summary>
        /// Whether or not the symbol base is empty after going to the resource dictionary.
        /// </summary>
        private readonly bool empty;

        /// <summary>
        /// The framed fill style (works for unframed too).
        /// </summary>
        private Style fill;

        /// <summary>
        /// The framed line style.
        /// </summary>
        private Style line;
        
        /// <summary>
        /// The framed line and fill style.
        /// </summary>
        private Style lineFill; 

        /// <summary>
        /// The unframed line style.
        /// </summary>
        private Style unframedLine;

        /// <summary>
        /// The unframed line and fill style.
        /// </summary>
        private Style unframedLineFill;

        /// <summary>
        /// The backing store for NeedDashed.
        /// </summary>
        private bool needDashed;

        /// <summary>
        /// The fill brush.
        /// </summary>
        private Brush brush;

        /// <summary>
        /// The backing store for LineThickness, defaults to 3.0
        /// </summary>
        private double lineThickness = 3.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MilSymbolBase"/> class.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for this symbol.
        /// </param>
        /// <param name="lineBrush">
        /// An optional line brush for outlining the symbol.
        /// </param>
        /// <param name="fillBrush">
        /// An optional fill brush for filling the symbol's background.
        /// </param>
        public MilSymbolBase(string symbolCode, Brush lineBrush, Brush fillBrush)
        {
            if (symbolCode == null)
            {
                this.empty = true;
                return;
            }

            string stencil = CodeToStencil(symbolCode);

            this.DataContext = this;

            // Will need to treat weather and tactical graphics carefully
            int schemeKey = CodingScheme.GetCode(symbolCode);
            if (schemeKey == CodingScheme.Weather)
            {
                // These symbols can change color - so we bind to Line and Fill
                if (symbolCode.StartsWith("WAS-WSTS"))
                {
                    if (lineBrush == null)
                    {
                        lineBrush = MilBrush.Rust;
                    }

                    if (fillBrush == null)
                    {
                        fillBrush = MilBrush.Rust;
                    }

                    this.SetLines(lineBrush);
                    this.SetLineFills(lineBrush, fillBrush);
                }

                this.Template = SymbolData.GetControlTemplate(stencil); // gets the template - the main thing
                this.empty = this.Template == null;
                if (!this.empty)
                {
                    this.SetLimits(symbolCode);
                }

                return;
            }

            // If the standard identity (StandardIdentity) is some type of pending, we'll need the
            // anticipated (dashhed) outline for the frame.
            this.needDashed = SymbolData.IsDashed(symbolCode);

            // There are occasions when we need a line style that matches affiliation and present
            int dimensionKey = CategoryBattleDimension.GetCode(symbolCode);
            bool needUnframed =
                schemeKey == CodingScheme.TacticalGraphics ||
                 dimensionKey == CategoryBattleDimension.BdOther ||
                 (schemeKey == CodingScheme.Warfighting &&
                  dimensionKey == CategoryBattleDimension.BdSubsurface);
            if (needUnframed)
            {
                this.unframedLine = MilBrush.GetLinePresent(MilBrush.FindColorScheme(symbolCode));
                this.SetUnframedLines(lineBrush);
            }

            this.SetLines(lineBrush);

            // Get a brush style if user didn't specify
            if (fillBrush == null)
            {
                fillBrush = MilBrush.FindColorScheme(symbolCode);
            }

            if (needUnframed)
            {
                this.SetUnframedLineFills(fillBrush);
            }

            this.SetLineFills(lineBrush, fillBrush);

            this.Template = SymbolData.GetControlTemplate(stencil); // gets the template - the main thing
            this.empty = this.Template == null;
            if (!this.empty)
            {
                this.SetLimits(symbolCode);
            }
        }

        /// <summary>
        /// The PropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the current default brush for drawing regular framed symbol lines.
        /// </summary>
        public Brush Brush
        {
            get { return this.brush; }
        }

        /// <summary>
        /// Gets the current default brush for drawing unframed symbol lines.
        /// </summary>
        public Style UnframedLine
        {
            get
            {
                if (!this.needDashed)
                {
                    return this.unframedLine;
                }

                return GetAnticipated(this.unframedLine);
            }
        }

        /// <summary>
        /// Gets the current default fill style for framed symbols.
        /// </summary>
        public Style Fill
        {
            get { return this.fill; }
        }

        /// <summary>
        /// Gets the current default line style for framed symbols.
        /// </summary>
        public Style Line
        {
            get
            {
                if (!this.needDashed)
                {
                    return this.line;
                }

                return GetAnticipated(this.line);
            }
        }

        /// <summary>
        /// Gets the current default line and fill style for unframed symbols.
        /// </summary>
        public Style UnframedLineFill
        {
            get
            {
                if (!this.needDashed)
                {
                    return this.unframedLineFill;
                }

                return GetAnticipated(this.unframedLineFill);
            }
        }

        /// <summary>
        /// Gets the current default line and fill styles for framed symbols.
        /// </summary>
        public Style LineFill
        {
            get
            {
                if (!this.needDashed)
                {
                    return this.lineFill;
                }

                return GetAnticipated(this.lineFill);
            }
        }

        /// <summary>
        /// Gets or sets the line thickness for the base symbol.
        /// </summary>
        public double LineThickness
        {
            get
            {
                return this.lineThickness;
            }

            set
            {
                this.lineThickness = value;
                this.NotifyPropertyChanged("LineThickness");
            }
        }

        /// <summary>
        /// Sets a value indicating whether we need a dashed line style for unframed.
        /// </summary>
        public bool NeedDashed
        {
            set
            {
                this.needDashed = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the base symbol is Empty (missing).
        /// </summary>
        internal bool Empty
        {
            get { return this.empty; }
        }

        /// <summary>
        /// Takes the symbol code and matches one of the stencils in the resource dictionary.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for which to look up the stencil.
        /// </param>
        /// <returns>
        /// The string that should be used to get the ResourceDictionary entry for this symbol.
        /// </returns>
        public static string CodeToStencil(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            int schemeKey = CodingScheme.GetCode(symbolCode);

            // Weather short circuits most considerations, at least for now
            if (schemeKey == CodingScheme.Weather)
            {
                return symbolCode;
            }

            string stencil = symbolCode.Substring(0, 10).ToUpper(CultureInfo.InvariantCulture);

            int dash = stencil.IndexOf('-', 4);
            if (dash > 0)
            {
                stencil = stencil.Substring(0, dash);
            }

            // Replace remaining '-'s with '_'s
            stencil = stencil.Replace('-', '_');

            // Return one of the four (approximately) that we know about
            int identityKey = StandardIdentity.GetNormalizedStandardIdentity(symbolCode);

            // Need the battle dimension
            int dimensionKey = CategoryBattleDimension.GetCode(symbolCode);

            // If we have a Z battle dimension cut right to the chase.
            // Get a blank stencil so we can add a question mark.
            if (dimensionKey == CategoryBattleDimension.BdUnknown)
            {
                return "I" + StandardIdentity.ToChar(identityKey) + "ZP";
            }

            string tail = stencil.Substring(2, 1) + "P" + stencil.Substring(4);

            // If we have EmergenctManagement and a NaturalEvent, get rid of the standard identity
            // or if we have a tactical graphic, get rid of the standard identity
            if ((schemeKey == CodingScheme.EmergencyManagement &&
                dimensionKey == CategoryBattleDimension.EmNaturalEvents) ||
                schemeKey == CodingScheme.TacticalGraphics)
            {
                return stencil.Substring(0, 1) + "_" + tail;
            }

            // If framed, we'll include the base affiliation
            if (dimensionKey != CategoryBattleDimension.BdOther)
            {
                return stencil.Substring(0, 1) + StandardIdentity.ToChar(identityKey) + tail;
            }

            // Otherwise, we're looking for an unframed element
            return stencil.Substring(0, 1) + "." + tail;
        }

        /// <summary>
        /// Notify property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name that changed.
        /// </param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Adds the black ribbon at the top of the symbol for the standard identity "Space".
        /// </summary>
        /// <param name="symbolCode">The symbol code for the space entity.</param>
        /// <returns>The shape representing the black ribbon overlay for a space entity.</returns>
        internal static Shape GenerateSpace(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            if (CategoryBattleDimension.GetCode(symbolCode) != CategoryBattleDimension.BdSpace)
            {
                return null;
            }

            // Handy points for future reference
            // Inside for unknown ground
            // <Path Data="F0 M-61,-140 C-30,-175 30,-175 61,-140" />
            // Inside for hostile ground
            // <Polygon Points="-61,-105 0,-166 61,-105" />
            switch (StandardIdentity.GetNormalizedStandardIdentity(symbolCode))
            {
                case StandardIdentity.Friend:
                {
                    return GenerateBlackSpline(
                    new Point(-61, -107), new Point(-23, -148), new Point(23, -148), new Point(61, -107));
                }

                case StandardIdentity.Unknown:
                {
                    return GenerateBlackSpline(
                    new Point(-61, -117), new Point(-30, -160), new Point(30, -160), new Point(61, -117));
                }

                case StandardIdentity.Hostile:
                {
                    return new Path
                    {
                        Fill = new SolidColorBrush(Colors.Black),
                        Data = new PathGeometry
                        {
                            Figures = new PathFigureCollection
                            {
                                new PathFigure
                                {
                                    StartPoint = new Point(-61, -93),
                                    Segments = new PathSegmentCollection
                                    {
                                        new LineSegment { Point = new Point(0, -149) },
                                        new LineSegment { Point = new Point(61, -93) }
                                    }
                                }
                            }
                        }
                    };
                }

                case StandardIdentity.Neutral:
                {
                    return new Path
                    {
                        Fill = new SolidColorBrush(Colors.Black),
                        Data = new PathGeometry
                        {
                            Figures = new PathFigureCollection
                                {
                                new PathFigure
                                {
                                    StartPoint = new Point(-127, -104),
                                    Segments = new PathSegmentCollection
                                    {
                                        new LineSegment { Point = new Point(127, -104) },
                                        new LineSegment { Point = new Point(127, -139) },
                                        new LineSegment { Point = new Point(-127, -139) }
                                    }
                                }
                            }
                        }
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Since we can't reuse this style in the visual tree, because of StrokeDashArrayProperty, we need to
        /// generate a new one on demand, as this method does.
        /// </summary>
        /// <param name="baseStyle">
        /// The base style for which to generate the anticipated style.
        /// </param>
        /// <returns>
        /// The new Style with dashed instead of solid lines.
        /// </returns>
        private static Style GetAnticipated(Style baseStyle)
        {
            var st = new Style(typeof(Shape)) { BasedOn = baseStyle };
            st.SetDashArray(2, 1);
            return st;
        }

        /// <summary>
        /// Generates a black spline outline, typically used to overlay on a space element.
        /// </summary>
        /// <param name="p0">
        /// The first point for the spline.
        /// </param>
        /// <param name="p1">
        /// The second point for the spline.
        /// </param>
        /// <param name="p2">
        /// The third point for the spline.
        /// </param>
        /// <param name="p3">
        /// The fourth point for the spline.
        /// </param>
        /// <returns>
        /// A Path that incorporates the spline.
        /// </returns>
        private static Path GenerateBlackSpline(Point p0, Point p1, Point p2, Point p3)
        {
            var p = new Path { Fill = new SolidColorBrush(Colors.Black) };
            var pg = new PathGeometry();
            var pf = new PathFigure { StartPoint = p0 };
            var bs = new BezierSegment { Point1 = p1, Point2 = p2, Point3 = p3 };
            pf.Segments.Add(bs);
            pg.Figures.Add(pf);
            p.Data = pg;
            return p;
        }

        /// <summary>
        /// Sets the width and height of the base symbol based on the symbol code, 
        /// as well as the top and left properties,
        /// checking first to see if the canvas already has such limits.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for the symbol.
        /// </param>
        private void SetLimits(string symbolCode)
        {
            this.SetValue(Canvas.TopProperty, 0.0);
            this.SetValue(Canvas.LeftProperty, 0.0);

            if (this.ApplyTemplate())
            {
                var canvas = VisualTreeHelper.GetChild(this, 0) as Canvas;

                if (canvas != null && !double.IsNaN(canvas.Height))
                {
                    this.Height = canvas.Height;
                    this.Width = canvas.Width;
                    this.SetValue(Canvas.TopProperty, canvas.GetValue(Canvas.TopProperty));
                    this.SetValue(Canvas.LeftProperty, canvas.GetValue(Canvas.LeftProperty));
                    return;
                }
            }

            // Need to handle those symbols that still don't have bounds
            Rect rect = SymbolData.GetBounds(symbolCode);
            if (!rect.IsEmpty)
            {
                this.Height = rect.Height;
                this.Width = rect.Width;
            }
            else
            {
                this.Height = this.Width = 0.0;
            }
        }

        /// <summary>
        /// Sets the brush for unframed lines.
        /// </summary>
        /// <param name="lineBrush">
        /// The line brush to use for unframed lines.
        /// </param>
        private void SetUnframedLines(Brush lineBrush)
        {
            if (lineBrush != null)
            {
                this.unframedLine = new Style(typeof(Shape)) { BasedOn = this.unframedLine };
                this.unframedLine.Setters.Add(new Setter(Shape.StrokeProperty, lineBrush));
            }
        }

        /// <summary>
        /// Sets the brush for unframed lines and fills
        /// </summary>
        /// <param name="fillBrush">
        /// The fill brush to use for unframed lines and fills.
        /// </param>
        private void SetUnframedLineFills(Brush fillBrush)
        {
            if (fillBrush == null)
            {
                return;
            }

            // Use brush to find fill styles
            this.fill = MilBrush.GetFill(fillBrush);
            this.unframedLineFill = MilBrush.GetLineFillPresent(fillBrush);
            this.unframedLineFill = new Style(typeof(Shape)) { BasedOn = this.unframedLineFill };
            this.unframedLineFill.Setters.Add(new Setter(Shape.StrokeProperty, fillBrush));
        }

        /// <summary>
        /// Sets the standard line brush.
        /// </summary>
        /// <param name="lineBrush">
        /// The line brush to be used as the standard.
        /// </param>
        private void SetLines(Brush lineBrush)
        {
            // Standard calls for black lines and black text
            this.line = MilBrush.GetLinePresent(MilBrush.Black);
            if (lineBrush != null)
            {
                this.line = new Style(typeof(Shape)) { BasedOn = this.line };
                this.line.Setters.Add(new Setter(Shape.StrokeProperty, lineBrush));
            }
        }

        /// <summary>
        /// Sets the standard line and fill brush.
        /// </summary>
        /// <param name="lineBrush">
        /// The line brush to be used for lines.
        /// </param>
        /// <param name="fillBrush">
        /// The fill brush to be used for fills.
        /// </param>
        private void SetLineFills(Brush lineBrush, Brush fillBrush)
        {
            if (fillBrush == null)
            {
                return;
            }

            // Use brush to find fill styles
            this.fill = MilBrush.GetFill(fillBrush);
            this.lineFill = MilBrush.GetLineFillPresent(fillBrush);
            if (lineBrush != null)
            {
                this.lineFill = new Style(typeof(Shape)) { BasedOn = this.lineFill };
                this.lineFill.Setters.Add(new Setter(Shape.StrokeProperty, lineBrush));
            }

            this.brush = fillBrush;        // may need this brush for unframed symbols
        }
    }
}