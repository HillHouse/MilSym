// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriMilSymLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Methods supporting IMilSymLayer for use by the Esri maps.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.EsriSupport
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls;
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Bing;
    using ESRI.ArcGIS.Client.Geometry;
    using MilGraph;
    using MilGraph.Support;
    using MilSymbol;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// Methods supporting IMilSymLayer for use by the Esri maps.
    /// </summary>
    public class EsriMilSymLayer : ElementLayer, IMilSymLayer
    {
        /// <summary>
        /// The Esri element layer appears to require a rectangle to place an element.
        /// This parameter supports a very small rectangle.
        /// </summary>
        private const double LonM = 1.0e-5;

        /// <summary>
        /// The Esri element layer appears to require a rectangle to place an element.
        /// This parameter supports a very small rectangle.
        /// </summary>
        private const double LatM = 1.0e-5;

        /// <summary>
        /// The Esri element layer appears to require a rectangle to place an element.
        /// This parameter supports a very small rectangle.
        /// </summary>
        private const double LonP = 1.0e-5;

        /// <summary>
        /// The Esri element layer appears to require a rectangle to place an element.
        /// This parameter supports a very small rectangle.
        /// </summary>
        private const double LatP = 1.0e-5;

        // Ran -85.05112878 degrees (the minimum mercator latitude value) through the converter
        // Ran -180.0 degrees (the minimum mercator longitude value) through the converter

        /// <summary>
        /// Min/max web mercator coordinates supported by Esri which apparently uses a non-degreed extent.
        /// </summary>
        private const double MinWebY = -20037508.3430389;

        /// <summary>
        /// Min/max web mercator coordinates supported by Esri which apparently uses a non-degreed extent.
        /// </summary>
        private const double MaxWebY = 20037508.3430389;

        /// <summary>
        /// Min/max web mercator coordinates supported by Esri which apparently uses a non-degreed extent.
        /// </summary>
        private const double MinWebX = -20037508.3427892;

        /// <summary>
        /// Min/max web mercator coordinates supported by Esri which apparently uses a non-degreed extent.
        /// </summary>
        private const double MaxWebX = 20037508.3427892;

        /// <summary>
        /// The backing store for the layer's extent.
        /// </summary>
        private readonly EsriLocationRect layerExtent =
            new EsriLocationRect(180.0, 90.0, -180.0, -90.0);
        
        /// <summary>
        /// Supports the MapExtent backing store.
        /// </summary>
        private Envelope webExtent = new Envelope();   // the map extent in Web mercator coordinates

        /// <summary>
        /// Supports the MapExtent backing store.
        /// </summary>
        private EsriLocationRect mapExtent;            // the map extent in lat-lon coordinates

        /// <summary>
        /// The backing store for the Map.
        /// </summary>
        private Map map;

        /// <summary>
        /// Supports the ZoomLevel backing store.
        /// </summary>
        private double lastResolution = -1.0;

        /// <summary>
        /// Supports the ZoomLevel backing store.
        /// </summary>
        private double zoomLevel; // = 0.0

        /// <summary>
        /// Gets the map layer's extent.
        /// </summary>
        public ILocationRect LayerExtent 
        { 
            get
            {
                return this.layerExtent; 
            }
        }

        /// <summary>
        /// Gets MapExtent.
        /// </summary>
        public ILocationRect MapExtent
        {
            get
            {
                if (!this.webExtent.Equals(this.TheMap.Extent))
                {
                    this.webExtent = this.TheMap.Extent.Clone();

                    // There is a hard "x" and "y" limit on the WebMercatorToGeographic conversion functions
                    var min = new MapPoint(
                        Math.Max(this.webExtent.XMin, MinWebX), Math.Max(this.webExtent.YMin, MinWebY));
                    var max = new MapPoint(
                        Math.Min(this.webExtent.XMax, MaxWebX), Math.Min(this.webExtent.YMax, MaxWebY));

                    this.mapExtent = new EsriLocationRect(
                        min.WebMercatorToGeographic(), 
                        max.WebMercatorToGeographic());
                }

                return this.mapExtent;
            }
        }

        /// <summary>
        /// Gets the Map that contains this layer.
        /// </summary>
        public Map TheMap
        {
            get
            {
                if (this.map != null)
                {
                    return this.map;
                }

                foreach (var esriMap in
#if SILVERLIGHT
                    Application.Current.RootVisual
#else
                    Application.Current.MainWindow
#endif
                    .GetVisuals().OfType<Map>()
                    .Select(control => control)
                    .Where(esriMap => esriMap.Layers.Contains(this)))
                {
                    this.map = esriMap;
                    return this.map;    // in general this should be called once
                }

                return null;
            }
        }

        /// <summary>
        /// Finds some (but maybe not all) of the map elements associated with a point.
        /// </summary>
        /// <param name="pos">The point at which to check for associated map elements</param>
        /// <returns>An enumerable list of associated map elements</returns>
        public IEnumerable<UIElement> ElementsAtPoint(Point pos)
        {
            var htr = VisualTreeHelper.HitTest(this.TheMap, pos);
            if (htr != null && htr.VisualHit is UIElement ele)
            {
                return new List<UIElement> { ele };
            }
            return null;
        }

        /// <summary>
        /// Returns a list of children associated with the layer.
        /// </summary>
        public IList<DependencyObject> ChildList
        {
            get
            {
                return base.Children.ToList<DependencyObject>();
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
            var prea = ea as MouseButtonEventArgs;
            if (prea != null)
            {
                return prea.GetPosition(TheMap);
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
            var loc = TheMap.ScreenToMap(p);
            return new EsriLocation(loc.Y, loc.X);
        }

        /// <summary>
        /// Gets the zoom level for the Esri map.
        /// </summary>
        public double ZoomLevel
        {
            get
            {
                if (this.TheMap == null)
                {
                    return 0.0;
                }

                var resolution = this.map.Resolution;
                if (double.IsNaN(resolution))
                {
                    return this.zoomLevel;
                }

                if (Math.Abs(resolution - this.lastResolution) < double.Epsilon)
                {
                    return this.zoomLevel;
                }

                this.lastResolution = resolution;

                // If we were matching to ESRI's lat-lon mapping this would be it
                this.zoomLevel = (Math.Log((360.0 / 512.0) / resolution) / Math.Log(2)) + 1.0;

                // But we also need this empirically derived value to match  
                // ESRI's Web zoom level to the true Bing map zoom level
                this.zoomLevel += 16.7643466889403;
                return this.zoomLevel;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this map layer can be seen, 
        /// specifically the map exists, has a real extent, and the layer's Visibility is true.
        /// </summary>
        public bool CanBeSeen
        {
            get
            {
                if (!this.Visible)
                {
                    return false;
                }

                if (this.TheMap == null)
                {
                    return false;
                }

                if (this.map.Extent == null)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Adds a UIElement to the layer at the indicated location.
        /// </summary>
        /// <param name="ue">
        /// The UIElement to add to the layer.
        /// </param>
        /// <param name="geo">
        /// The map location at which to add the UIElement.
        /// </param>
        public void AddSymbol(UIElement ue, ILocation geo)
        {
            if (geo == null)
            {
                return;
            }

            // ESRI apparently doesn't use lat-lon with Bing maps
            var web = (geo as MapPoint).GeographicToWebMercator();
            var webEnvelope = new Envelope(web.X - LonM, web.Y - LatM, web.X + LonP, web.Y + LatP);
// ReSharper disable RedundantNameQualifier
            ElementLayer.SetEnvelope(ue, webEnvelope);
// ReSharper restore RedundantNameQualifier
            this.layerExtent.Expand(geo);

            // Extra work for special symbol types
            if (ue is MapMilSymbol)
            {
                ((MapMilSymbol)ue).Layer = this;

                // Similar computation in lat-lon space
                var geoEnvelope = new EsriLocationRect(
                    geo.Longitude - LonM, geo.Latitude - LatM, geo.Longitude + LonP, geo.Latitude + LatP);
                ((MapMilSymbol)ue).SymbolExtent = geoEnvelope;
            }
            else if (ue is MilGraphic)
            {
                ((MilGraphic)ue).Layer = this;
            }

            if (!base.Children.Contains(ue))
            {
                base.Children.Add(ue);
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
            var uiElement = symbol as UIElement;
            if (uiElement != null)
            {
                this.AddSymbol(uiElement, symbol.Origin);
            }
        }
    }
}
