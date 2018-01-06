// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="TestDynamic.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A sample dynamic test for updating a generic tactical graphic.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicUpdateTest
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    using MilSym.MilGraph;
    using MilSym.MilGraph.Support;
    using MilSym.MilSymbol;

    /// <summary>
    /// The generic map test drawing code used for Bing Silverlight and ESRI Silverlight and WPF.
    /// </summary>
    public class TestDynamic
    {
        /// <summary>
        /// The map factory for generating locations and other map dependent items.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// The map layer for displaying symbols.
        /// </summary>
        private readonly IMilSymLayer milsymLayer;

        /// <summary>
        /// The map layer for displaying polygons and polylines.
        /// </summary>
        private readonly IPolyLayer polyLayer;

        /// <summary>
        /// Some triple point locations for some tactical graphics.
        /// </summary>
        private readonly Point[] triad = new[]
        {
            new Point(24.65, -70.0), new Point(24.5, -70.0), new Point(24.575, -70.135)
        };

        /// <summary>
        /// A timer to control the animation.
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// The tactical graphic to be animated.
        /// </summary>
        private MilGraphic pg;

        /// <summary>
        /// A counter variable used to drive the animation.
        /// </summary>
        private double count;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDynamic"/> class.
        /// </summary>
        /// <param name="milsymFactory">
        /// The instance of the map factory.
        /// </param>
        /// <param name="ml">
        /// The symbology layer.
        /// </param>
        /// <param name="pl">
        /// The polygon/polyline layer.
        /// </param>
        public TestDynamic(IMilSymFactory milsymFactory, IMilSymLayer ml, IPolyLayer pl)
        {
            this.timer = new DispatcherTimer();
            this.milsymFactory = milsymFactory;
            this.milsymLayer = ml;
            this.polyLayer = pl;
        }

        /// <summary>
        /// This is the workhorse method that generates both tactial and point symbology.
        /// </summary>
        public void DrawStuff()
        {
            MilBrush.ColorScheme = ColorSchemeProperty.Dark;
            this.PlotSymbol("GHTPWP----****X", this.RotateCollection(0.0));
            this.timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50) };
            this.timer.Tick += this.TimerTick;
            this.timer.Start();
        }

        /// <summary>
        /// Plots a 3+ point tactical graphic
        /// </summary>
        /// <param name="sc">
        /// The symbol code to plot.
        /// </param>
        /// <param name="loc">
        /// The location collection that defines the spatial extent of the symbol.
        /// </param>
        public void PlotSymbol(string sc, ILocationCollection loc)
        {
                this.pg = new MilGraphic(sc, loc);
                ToolTipService.SetToolTip(this.pg, sc);
                this.milsymLayer.AddSymbol(this.pg);

                // Draw the base and transformed base vectors
                this.polyLayer.AddPolyline(loc, new SolidColorBrush(Colors.Blue));
        }

        /// <summary>
        /// Updates the symbol when timer expires.
        /// </summary>
        /// <param name="sender">
        /// This parameter is not used.
        /// </param>
        /// <param name="ea">
        /// The parameter is also not used.
        /// </param>
        private void TimerTick(object sender, EventArgs ea)
        {
            this.pg.Anchors = this.RotateCollection(this.count);
            this.count += 3.0;
        }

        /// <summary>
        /// Rotates a collection of points about the first point and returns a location collection of the rotated points.
        /// </summary>
        /// <param name="angle">
        /// The angle in degrees by which to rotate the point collection.
        /// </param>
        /// <returns>
        /// The location collection of rotated points.
        /// </returns>
        private ILocationCollection RotateCollection(double angle)
        {
            // In this example, X is latitude.
            var saveX1 = this.triad[1].X;
            var saveX2 = this.triad[2].X;
            var off = 0.5 + (0.2 * Math.Sin(1.3 * this.count * Math.PI / 180.0));
            this.triad[1].X += off;
            this.triad[2].X += off;
            
            var rt = new RotateTransform { Angle = angle, CenterX = this.triad[0].X, CenterY = this.triad[0].Y };

            var locOut = this.milsymFactory.LocationCollection();
            foreach (var p in this.triad.Select(rt.Transform))
            {
                locOut.Add(this.milsymFactory.Location(Order.LatLon, p.X, p.Y));
            }

            this.triad[1].X = saveX1;
            this.triad[2].X = saveX2;
            return locOut;
        }
    }
}
