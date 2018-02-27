// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="FischerMilSymLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Creates a Fischer map symbol layer and provides layer services.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.FischerSupport
{
    using System.Linq;
    using System.Collections.Generic;

#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Media;
#elif WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using MapControl;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using MapControl;
#endif
    using MilGraph;
    using MilGraph.Support;
    using MilSymbol;

    /// <summary>
    ///  Creates a Fischer map symbol layer and provides layer services.
    /// </summary>
    public class FischerMilSymLayer : MapPanel, IMilSymLayer
    {
        /// <summary>
        /// An arbitrarily small delta around a symbol to ensure that its bounding box is not simply a point.
        /// </summary>
        private const double ArbitraryDelta = 0.01;

        /// <summary>
        /// Backing store for LayerExtent - initialize to impossible values that will be replaced
        /// on the first compare.
        /// </summary>
        private readonly FischerLocationRect layerExtent =
            new FischerLocationRect(-180.0, -90.0, 180.0, 90.0);

        /// <summary>
        /// Part of backing store for map extent.
        /// </summary>
        //private ILocationRect locationRect;

        /// <summary>
        /// Backing store for the map extent.
        /// </summary>
        private FischerLocationRect mapExtent;

        /// <summary>
        /// Backing store for the map's transformation matrix.
        /// </summary>
        private Matrix saveMatrix = Matrix.Identity;

        /// <summary>
        /// Backing store for the Map object.
        /// </summary>
        private MapControl.Map map;

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
                var mp = TheMap.ParentMap.MapProjection;
                if (mp.ViewportTransform.Matrix == this.saveMatrix)
                {
                    return this.mapExtent;
                }
                this.saveMatrix = mp.ViewportTransform.Matrix;

#if WINDOWS_UWP
                this.mapExtent = new FischerLocationRect(
                    mp.ViewportRectToBoundingBox(TheMap.ParentMap.Clip.Bounds));
#else
                var rect = new Rect(0, 0, TheMap.ActualWidth, TheMap.ActualHeight);
                this.mapExtent = new FischerLocationRect(mp.ViewportRectToBoundingBox(rect));
#endif
                return this.mapExtent;
            }
        }

        /// <summary>
        /// Gets the map containing this layer.
        /// </summary>
        public MapControl.Map TheMap
        {
            get
            {
                if (this.map != null)
                {
                    return this.map;
                }

                foreach (var greatMap in
#if SILVERLIGHT
                    Application.Current.RootVisual
#elif WINDOWS_UWP
                    Window.Current.Content
#else
                    Application.Current.MainWindow
#endif
                    .GetVisuals().OfType<Map>()
                    .Select(control => control)
                    .Where(greatMap => greatMap.Children.Contains(this)))
                {
                    this.map = greatMap;
                    return this.map;    // in general this should execute once
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
#if WINDOWS_UWP
            return VisualTreeHelper.FindElementsInHostCoordinates(pos, TheMap, false).OfType<UIElement>().ToList<UIElement>();
#else
            var htr = VisualTreeHelper.HitTest(this.TheMap, pos);
            if (htr != null && htr.VisualHit is UIElement ele)
            {
                return new List<UIElement> { ele };
            }
            return null;
#endif
        }

        /// <summary>
        /// Returns a list of children associated with the layer.
        /// </summary>
#if WINDOWS_UWP
        public IList<DependencyObject> ChildList { get { return base.Children.ToList<DependencyObject>(); } }
#else
        public IList<DependencyObject> ChildList { get { return base.Children.OfType<DependencyObject>().ToList<DependencyObject>(); } }
#endif

        /// <summary>
        /// Returns the point associated with the passed in event
        /// </summary>
        /// <typeparam name="T">The type of the event argument</typeparam>
        /// <param name="ea">The event argument</param>
        /// <returns>The point associated with the passed in event</returns>
        public Point EventToPoint<T>(T ea)
        {
#if WINDOWS_UWP
            var prea = ea as PointerRoutedEventArgs;
            if (prea != null)
            {
                return prea.GetCurrentPoint(TheMap).Position;
            }
#else
            var prea = ea as MouseButtonEventArgs;
            if (prea != null)
            {
                return prea.GetPosition(TheMap);
            }
#endif
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
            var loc = TheMap.ViewportPointToLocation(p);
            return new FischerLocation(loc.Latitude, loc.Longitude);
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

                return this.TheMap.ZoomLevel;
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
                if (this.TheMap.Visibility != Visibility.Visible)
                {
                    return false;
                }

                return true;
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
            if (ui is MapMilSymbol mapMilSymbol)
            {
                mapMilSymbol.Layer = this;
                mapMilSymbol.SymbolExtent = new FischerLocationRect(
                    loc.Longitude - ArbitraryDelta,
                    loc.Latitude - ArbitraryDelta,
                    loc.Longitude + ArbitraryDelta,
                    loc.Latitude + ArbitraryDelta);

                if (!base.Children.Contains(mapMilSymbol.MapParent))
                {
                    // These are necessary for the border to properly size the symbol
                    mapMilSymbol.Width = 0;
                    mapMilSymbol.Height = 0;
                    mapMilSymbol.MaxHeight = 0;
                    mapMilSymbol.MaxWidth = 0;

                    mapMilSymbol.MapParent = new Border { Child = mapMilSymbol };
                    base.Children.Add(mapMilSymbol.MapParent);
                }
                mapMilSymbol.MapParent.SetValue(MapPanel.LocationProperty, loc);
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
                mg.MapParent.SetValue(LocationProperty,
                    new FischerLocation(mg.Origin.Latitude, mg.Origin.Longitude));
                base.Children.Add(mg.MapParent);
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
            foreach (var loc in lc)
            {
                this.UpdateLocationRectangle(loc.Longitude, loc.Latitude);
            }

            base.Children.Add(new MapPolyline { Locations = lc as LocationCollection, Stroke = br });
        }
    }
}
