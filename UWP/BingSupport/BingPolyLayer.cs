// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingPolyLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Provides map polyline layer services.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.BingSupport
{
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Devices.Geolocation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Controls.Maps;

    using MilGraph.Support;
    using MilSym.MilSymbol;

    /// <summary>
    /// Provides map polyline layer services.
    /// </summary>
    public class BingPolyLayer : IPolyLayer
    {
        /// <summary>
        /// Needed for simple polygons and polylines.
        /// </summary>
        private MapElementsLayer mel = new MapElementsLayer();
        private MapControl bingMap = null;

        public BingPolyLayer()
        {
            this.bingMap = Window.Current.Content.GetVisuals().OfType<MapControl>().FirstOrDefault();
            if (this.bingMap == null)
            {
                return;
            }
            if (this.bingMap.Layers == null)
            {
                this.bingMap.Layers = new List<MapLayer>();
            }
            if (!this.bingMap.Layers.Contains(this.mel))
            {
                this.bingMap.Layers.Add(this.mel);
            }
        }



        /// <summary>
        /// Adds a polyline to this map layer.
        /// </summary>
        /// <param name="lc">
        /// The collection of map locations.
        /// </param>
        /// <param name="br">
        /// The brush to use when rendering the polyline.
        /// </param>
        public void AddPolyline(ILocationCollection lc, Brush br)
        {
            if (this.bingMap == null)
            {
                return;
            }
            var bg = new List<BasicGeoposition>();
            foreach (var loc in lc)
            {
                bg.Add(BingMilSymLayer.LocationToGeoposition(loc));
            }

            if (br is SolidColorBrush scb)
            {
                var mp = new MapPolyline
                {
                    Path = new Geopath(bg),
                    StrokeColor = scb.Color,
                    StrokeThickness = 1.0,
                    StrokeDashed = false
                };
                this.mel.MapElements.Add(mp);
            }
        }
    }
}
