// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingMilSymLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Creates a Bing map symbol layer and provides layer services.
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
#if SILVERLIGHT
    using Microsoft.Maps.MapControl;
#else
    using Microsoft.Maps.MapControl.WPF;    // a big shout-out to Microsoft
#endif
    using MilGraph;
    using MilGraph.Support;
    using MilSymbol;

    /// <summary>
    ///  Creates a Bing map symbol layer and provides layer services.
    /// </summary>
    public class BingMilSymLayer : MapLayer, IMilSymLayer
    {
        /// <summary>
        /// An arbitrarily small delta around a symbol to ensure that its bounding box is not simply a point.
        /// </summary>
        private const double ArbitraryDelta = 0.01;

        /// <summary>
        /// Backing store for LayerExtent - initialize to impossible values that will be replaced
        /// on the first compare.
        /// </summary>
        private readonly BingLocationRect layerExtent =
            new BingLocationRect(180.0, 90.0, -180.0, -90.0);

        /// <summary>
        /// Part of backing store for map extent.
        /// </summary>
        private LocationRect locationRect;

        /// <summary>
        /// Backing store for the map extent.
        /// </summary>
        private BingLocationRect mapExtent;

        /// <summary>
        /// Backing store for the Map object.
        /// </summary>
        private Map map;

        /// <summary>
        /// Gets the extent of the symbols on the layer.
        /// </summary>
        public ILocationRect LayerExtent
        {
            get { return this.layerExtent; }
        }

        /// <summary>
        /// Gets the current viewing extent of the map.
        /// </summary>
        public ILocationRect MapExtent
        {
            get
            {
                // both could be null initially so make sure to check for null on return
                if (this.locationRect != this.TheMap.BoundingRectangle)
                {
                    this.locationRect = this.TheMap.BoundingRectangle;
                    this.mapExtent = new BingLocationRect(
                        this.locationRect.West, this.locationRect.South, this.locationRect.East, this.locationRect.North);
                }

                return this.mapExtent;
            }
        }

        /// <summary>
        /// Gets the map containing this layer.
        /// </summary>
        public Map TheMap
        {
            get
            {
                if (this.map != null)
                {
                    return this.map;
                }

                foreach (var bingMap in
#if SILVERLIGHT
                    Application.Current.RootVisual
#else
                    Application.Current.MainWindow
#endif
                    .GetVisuals().OfType<Map>()
                    .Select(control => control)
                    .Where(bingMap => bingMap.Children.Contains(this)))
                {
                    this.map = bingMap;
                    return this.map;    // in general this should execute once
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the map's zoom level - assuming it is available and 0.0 otherwise.
        /// </summary>
        public double ZoomLevel
        {
            get
            {
                if (this.TheMap == null)
                {
                    return 0.0;
                }

                return this.map.ZoomLevel;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the layer can be seen - 
        /// which requires that the map exist and the layer be marked visible.
        /// </summary>
        public bool CanBeSeen
        {
            get
            {
                if (this.Visibility != Visibility.Visible)
                {
                    return false;
                }

                return this.TheMap != null;
            }
        }

        /// <summary>
        /// Updates the location rectangle by including the indicated latitude and longitude.
        /// </summary>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        public void UpdateLocationRectangle(double longitude, double latitude)
        {
            if (this.layerExtent.East < longitude)
            {
                this.layerExtent.East = longitude;
            }

            if (this.layerExtent.West > longitude)
            {
                this.layerExtent.West = longitude;
            }

            if (this.layerExtent.North < latitude)
            {
                this.layerExtent.North = latitude;
            }

            if (this.layerExtent.South > latitude)
            {
                this.layerExtent.South = latitude;
            }
        }

        /// <summary>
        /// Adds a symbol to the map layer at the specified location.
        /// </summary>
        /// <param name="ui">
        /// The symbol to be added.
        /// </param>
        /// <param name="loc">
        /// The location at which to place the symbol.
        /// </param>
        public void AddSymbol(UIElement ui, ILocation loc)
        {
            var mapMilSymbol = ui as MapMilSymbol;
            if (mapMilSymbol != null)
            {
                mapMilSymbol.Layer = this;
                mapMilSymbol.SymbolExtent = new BingLocationRect(
                    loc.Longitude - ArbitraryDelta,
                    loc.Latitude - ArbitraryDelta,
                    loc.Longitude + ArbitraryDelta,
                    loc.Latitude + ArbitraryDelta);
            }

            this.UpdateLocationRectangle(loc.Longitude, loc.Latitude);
            if (!Children.Contains(ui))
            {
                this.AddChild(ui, loc as Location);
            }
            else
            {
// ReSharper disable RedundantNameQualifier
                ui.SetValue(MapLayer.PositionProperty, loc);
// ReSharper restore RedundantNameQualifier
            }
        }

        /// <summary>
        /// Adds a symbol to the map, assuming the symbol has an origin value set.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to add to the map.
        /// </param>
        public void AddSymbol(ILocatable symbol)
        {
            if (symbol is MilGraphic)
            {
                var mg = (MilGraphic)symbol;
                this.UpdateLocationRectangle(mg.LocationRect.West, mg.LocationRect.South);
                this.UpdateLocationRectangle(mg.LocationRect.East, mg.LocationRect.North);
                mg.Layer = this;
// ReSharper disable RedundantNameQualifier
                mg.SetValue(MapLayer.PositionProperty, mg.Origin);
// ReSharper restore RedundantNameQualifier
                Children.Add(mg);
            }
            else if (symbol is UIElement)
            {
                this.AddSymbol((UIElement)symbol, symbol.Origin);
            }
        }

        /// <summary>
        /// Add a polyline to the map layer.
        /// </summary>
        /// <param name="lc">
        /// The collection of map locations making up the polyline.
        /// </param>
        /// <param name="br">
        /// The brush with which to render the polyline.
        /// </param>
        public void AddPolyline(ILocationCollection lc, Brush br)
        {
            foreach (var loc in lc)
            {
                this.UpdateLocationRectangle(loc.Longitude, loc.Latitude);
            }

            Children.Add(new MapPolyline { Locations = lc as LocationCollection, Stroke = br });
        }
    }
}
