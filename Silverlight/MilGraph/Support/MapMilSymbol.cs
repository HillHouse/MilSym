// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MapMilSymbol.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A sample wrapper class around MilSymbol to draw bounding boxes on the map thus 
//   allowing the symbol to be offset as the map is zoomed.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilGraph.Support
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// A sample wrapper class around MilSymbol to draw bounding boxes on the map thus 
    /// allowing the symbol to be offset as the map is zoomed.
    /// </summary>
    public class MapMilSymbol : MilSymbol.MilSymbol, ILocatable
    {
        /// <summary>
        /// The DependencyProperty for Origin.
        /// </summary>
        private static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            "Origin", typeof(ILocation), typeof(MapMilSymbol), new PropertyMetadata(null, OnOriginChanged));        
        
        /// <summary>
        /// The scaleTransform to apply to the symbol to place it correctly on the map.
        /// </summary>
        private ScaleTransform scaleTransform;

        /// <summary>
        /// The last zoom level at which this symbol was rendered.
        /// This needs to be changed in the future to support a View/ViewModel approach.
        /// </summary>
        private double lastZoomLevel;  // = 0.0

        /// <summary>
        /// Initializes a new instance of the <see cref="MapMilSymbol"/> class.
        /// </summary>
        /// <param name="symbolCode">The symbol code for the symbol.</param>
        /// <param name="scale">The relative scale (size) of the symbol.</param>
        /// <param name="labelString">The labels for the symbol.</param>
        /// <param name="lineBrush">A brush to color the base symbol outline </param>
        /// <param name="fillBrush">A brush to color the base symbol background</param>
        /// <param name="labelStyle">A style to use for labels</param>
        /// <param name="opacity">The opacity of the symbol.</param>
        /// <param name="origin">The location (latitude, longitude) at which to place the symbol.</param>
        public MapMilSymbol(
            string symbolCode,
            double scale = 0.14,
            string labelString = "",
            Brush lineBrush = null,
            Brush fillBrush = null,
            Style labelStyle = null,
            double opacity = 0.7,
            ILocation origin = null) :
            base(symbolCode, scale: scale, labelString: labelString, lineBrush: lineBrush, fillBrush: fillBrush, labelStyle: labelStyle)
        {
            Opacity = opacity;
            this.Origin = origin;
            this.LayoutUpdated += this.SymbolLayoutUpdated;
        }

        /// <summary>
        /// Gets or sets the symbol's Origin.
        /// </summary>
        public ILocation Origin
        {
            get
            {
                return (ILocation)GetValue(OriginProperty);
            }

            set
            {
                SetValue(OriginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the extent of the symbol in pixel coordinate space.
        /// </summary>
        public ILocationRect SymbolExtent { get; set; }

        /// <summary>
        /// Gets or sets Layer for this symbol.
        /// </summary>
        public IMilSymLayer Layer { get; set; }

        /// <summary>
        /// Update the symbol position on the map.
        /// </summary>
        /// <param name="dp">
        /// The map symbol whose position is to be updated.
        /// </param>
        /// <param name="e">
        /// This parameter is not used.
        /// </param>
        public static void OnOriginChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var mg = dp as MapMilSymbol;
            if (mg != null && mg.SymbolCode != null && mg.Layer != null && mg.Origin != null)
            {
                mg.Layer.AddSymbol(mg, mg.Origin);
            }
        }

        /// <summary>
        /// Wrap the symbol with a bounding box that points to its location.
        /// </summary>
        public void Wrap()
        {
            var l = Bounds.Left;
            var t = Bounds.Top;
            var h = Bounds.Height;
            var w = Bounds.Width;

            const double Offset = 40.0;
            const double B = 10.0;          // pixel boundary adjustment

            TransformGroup tg = null;
            int i = this.FindScaleTransformIndex(ref tg);

            // If the next child is a TranslateTransform, replace it 
            if (i > 0 && tg.Children[i - 1] is TranslateTransform)
            {
                tg.Children[i - 1] = new TranslateTransform { X = 0.0, Y = -(t + h + Offset) };
            }
            else
            {
                tg.Children.Insert(i, new TranslateTransform { X = 0.0, Y = -(t + h + Offset) });
            }

            var pc = new PointCollection
            {
                new Point(l - B, t + h + B),
                new Point(-B, t + h + B),
                new Point(0, t + h + Offset),
                new Point(B, t + h + B),
                new Point(l + w + B, t + h + B),
                new Point(l + w + B, t - B),
                new Point(l - B, t - B)
            };

            var poly = Children[0] as Polygon;
            if (poly != null)
            {
                poly.Points = pc;
                IsDirty = true;
            }
            else
            {
                // Define and add the polygonal wrapper
                var box = new Polygon
                {
                    Points = pc,
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 10.0,
                    StrokeMiterLimit = 2.0,
                    Fill = new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))
                };
                Children.Insert(0, box);
            }
        }

        /// <summary>
        /// Update the transform matrix for this symbol.
        /// </summary>
        /// <param name="sender">
        /// This parameter is ignored.
        /// </param>
        /// <param name="e">
        /// This parameter is also ignored.
        /// </param>
        public void SymbolLayoutUpdated(object sender, EventArgs e)
        {
            if (this.Layer == null)
            {
                return;
            }

            if (!this.Layer.CanBeSeen)
            {
                return;
            }

            // If graphic is not visible (roughly speaking since these bounds are not precise), then return
            var extent = this.Layer.MapExtent;
            if (extent == null)
            {
                return;
            }

            if (!extent.Intersects(this.SymbolExtent))
            {
                return;
            }

            // Get the zoom so we can compute the map scale
            double zoom = this.Layer.ZoomLevel;
            if (!this.IsDirty && this.lastZoomLevel == zoom)
            {
                return;     // don't recompute if we have the same zoom value
            }

            this.IsDirty = false;
            this.lastZoomLevel = zoom;

            // Get the scale transform the first time, 
            // shouldn't actually be necessary 
            if (this.scaleTransform == null)
            {
                TransformGroup tg = null;
                this.FindScaleTransformIndex(ref tg);
            }

            // Any old scale scheme will work here
            if (this.scaleTransform != null)
            {
                this.scaleTransform.ScaleX =
                this.scaleTransform.ScaleY = Scale * Math.Pow(this.lastZoomLevel, 1.2) / 16.0;
            }
        }

        /// <summary>
        /// Find the first scale transform index and return it. If there aren't any, return 0.
        /// </summary>
        /// <param name="tg">
        /// The TransformGroup in which to find the scale transform.
        /// </param>
        /// <returns>
        /// The scale transform index or 0.
        /// </returns>
        private int FindScaleTransformIndex(ref TransformGroup tg)
        {
            // Get the RenderTransform and it there isn't one, create one
            tg = RenderTransform as TransformGroup;
            if (tg == null)
            {
                tg = new TransformGroup();
                this.scaleTransform = new ScaleTransform { ScaleX = 0.01, ScaleY = 0.01 };
                tg.Children.Add(this.scaleTransform);
                RenderTransform = tg;
                return 0;
            }

            // Find the ScaleTransform index
            for (var i = 0; i < tg.Children.Count; i++)
            {
                var st = tg.Children[i] as ScaleTransform;
                if (st != null)
                {
                    this.scaleTransform = st;

                    // Make the initial display really small until someone processes
                    this.scaleTransform.ScaleX = this.scaleTransform.ScaleY = 0.01;
                    return i;
                }
            }

            return 0;
        }
    }
}
