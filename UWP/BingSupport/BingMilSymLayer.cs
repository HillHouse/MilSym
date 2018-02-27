// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingMilSymLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Creates a Ms map symbol layer and provides layer services.
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
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Controls.Maps;

    using MilGraph;
    using MilGraph.Support;
    using MilSymbol;
    using MilSym.LoadResources;
    using System;

    /// <summary>
    ///  Creates a Ms map symbol layer and provides layer services.
    /// </summary>
    public class BingMilSymLayer : IMilSymLayer
    {
        /// <summary>
        /// Needed for simple polygons and polylines.
        /// </summary>
        private MapElementsLayer mel = new MapElementsLayer();

        /// The message logger
        private static readonly ILogger Log = LoggerFactory<BingMilSymLayer>.GetLogger();

        /// <summary>
        /// An arbitrarily small delta around a symbol to ensure that its bounding box is not simply a point.
        /// </summary>
        private const double ArbitraryDelta = 0.01;

        /// <summary>
        /// Backing store for LayerExtent - initialize to impossible values that will be replaced
        /// on the first compare.
        /// </summary>
        private readonly ILocationRect layerExtent = new BingLocationRect(180.0, 90.0, -180.0, -90.0);

        /// <summary>
        /// Part of backing store for map extent.
        /// </summary>
        //private BingLocationRect locationRect;

        /// <summary>
        /// Backing store for the map extent.
        /// </summary>
        private ILocationRect mapExtent;

        /// <summary>
        /// Backing store for the Map object.
        /// </summary>
        private MapControl map;

        public BingMilSymLayer()
        {
            this.map = Window.Current.Content.GetVisuals().OfType<MapControl>().FirstOrDefault<MapControl>();
            if (this.map == null)
            {
                return;
            }
            if (this.map.Layers == null)
            {
                this.map.Layers = new List<MapLayer>();
            }
            if (!this.map.Layers.Contains(this.mel))
            {
                this.map.Layers.Add(this.mel);
            }
        }

        /// <summary>
        /// Gets the extent of the symbols on the layer.
        /// </summary>
        public ILocationRect LayerExtent
        {
            get { return this.layerExtent; }
        }

        private Geopath saveGeopath;

        /// <summary>
        /// Gets the current viewing extent of the map.
        /// </summary>
        public ILocationRect MapExtent
        {
            get
            {
                var gp = map.GetVisibleRegion(MapVisibleRegionKind.Full);

                // both could be null initially so make sure to check for null on return
                if (this.saveGeopath != gp)
                {
                    this.saveGeopath = gp;

                    var west = 180.0;
                    var east = -180.0;
                    var south = 90.0;
                    var north = -90.0;
                    foreach (var pt in gp.Positions)
                    {
                        if (west > pt.Longitude) west = pt.Longitude;
                        if (east < pt.Longitude) east = pt.Longitude;
                        if (south > pt.Latitude) south = pt.Latitude;
                        if (north < pt.Latitude) north = pt.Latitude;
                    }

                    this.mapExtent = new BingLocationRect(west, south, east, north);
                }

                return this.mapExtent;
            }
        }

        /// <summary>
        /// Gets the map containing this layer.
        /// </summary>
        public MapControl TheMap
        {
            get
            {
                return this.map;
            }
        }

        /// <summary>
        /// Finds some (but maybe not all) of the map elements associated with a point.
        /// </summary>
        /// <param name="pos">The point at which to check for associated map elements</param>
        /// <returns>An enumerable list of associated map elements</returns>
        public IEnumerable<UIElement> ElementsAtPoint(Point pos)
        {
            var eleList = new List<UIElement>();
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(pos, TheMap, false);
            try
            {
                var count = 0;
                var length = elements.Count();

                // Elements can have several hundred elements but throws an EETypeLoadException 
                // after four elements have been enumerated. Four is enough for us.
                foreach (var element in elements)
                {
                    eleList.Add(element);
                    if (++count > 3) break;
                }
            }
            catch (Exception ex)
            {
                Log.WriteMessage(LogLevel.Warn, "Error reading list of elements at host coordinates", ex);
            }
            return eleList;
        }

        /// <summary>
        /// Returns a list of children associated with the layer.
        /// </summary>
        public IList<DependencyObject> ChildList
        {
            get
            {
                return TheMap.Children;
            }
        }

        /// <summary>
        /// Returns the point associated with the passed in event
        /// </summary>
        /// <typeparam name="T">The type of the event argument</typeparam>
        /// <param name="ea">The event argument</param>
        /// <returns>The point associated with the passed in event</returns>
        public Point EventToPoint<T>(T ea)
        {
            var prea = ea as PointerRoutedEventArgs;
            if (prea != null)
            {
                return prea.GetCurrentPoint(TheMap).Position;
            }
            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Returns the latitude and longitude corresponding to a screen point.
        /// </summary>
        /// <param name="p">
        /// The screen point.
        /// </param>
        /// <returns>The latitude and longitude corresponding to the screen point.</returns>
        public ILocation PointToLocation(Point p)
        {
            TheMap.GetLocationFromOffset(p, out Geopoint loc);
            return new BingLocation(loc.Position.Latitude, loc.Position.Longitude);
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
                if (mel.Visible != true)
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

        public static BasicGeoposition LocationToGeoposition(ILocation loc)
        {
            return new BasicGeoposition
            {
                Latitude = loc.Latitude,
                Longitude = loc.Longitude,
                Altitude = loc.Altitude
            };
        }

        public static Geopoint LocationToGeopoint(ILocation loc)
        {
            return new Geopoint(LocationToGeoposition(loc));
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
            if (ui is MapMilSymbol mapMilSymbol)
            {
                mapMilSymbol.Layer = this;
                mapMilSymbol.SymbolExtent = new BingLocationRect(
                    loc.Longitude - ArbitraryDelta,
                    loc.Latitude - ArbitraryDelta,
                    loc.Longitude + ArbitraryDelta,
                    loc.Latitude + ArbitraryDelta);

                if (!this.TheMap.Children.Contains(mapMilSymbol.MapParent))
                {
                    // These are necessary for the border to properly size the symbol
                    mapMilSymbol.Width = 0;
                    mapMilSymbol.Height = 0;
                    mapMilSymbol.MaxHeight = 0;
                    mapMilSymbol.MaxWidth = 0;

                    mapMilSymbol.MapParent = new Border { Child = mapMilSymbol };
                    this.map.Children.Add(mapMilSymbol.MapParent);
                }
                mapMilSymbol.MapParent.SetValue(MapControl.LocationProperty, LocationToGeopoint(loc));
            }
            this.UpdateLocationRectangle(loc.Longitude, loc.Latitude);
        }

        /// <summary>
        /// Adds a symbol to the map, assuming the symbol has an origin value set.
        /// </summary>
        /// <param name="symbol">
        /// The symbol to add to the map.
        /// </param>
        public void AddSymbol(ILocatable symbol)
        {
            if (symbol is MilGraphic mg)
            {
                this.UpdateLocationRectangle(mg.LocationRect.West, mg.LocationRect.South);
                this.UpdateLocationRectangle(mg.LocationRect.East, mg.LocationRect.North);
                mg.Layer = this;

                // These are necessary for the border to properly size the symbol
                mg.Width = 0;
                mg.Height = 0;
                mg.MaxHeight = 0;
                mg.MaxWidth = 0;

                mg.MapParent = new Border { Child = mg };
                mg.MapParent.SetValue(MapControl.LocationProperty, 
                    LocationToGeopoint(mg.Origin));
                this.map.Children.Add(mg.MapParent);
            }
            else if (symbol is UIElement ele)
            {
                this.AddSymbol(ele, symbol.Origin);
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
            if (this.map == null)
            {
                return;
            }

            var bg = new List<BasicGeoposition>();
            foreach (var loc in lc)
            {
                this.UpdateLocationRectangle(loc.Longitude, loc.Latitude);
                bg.Add(LocationToGeoposition(loc));
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
