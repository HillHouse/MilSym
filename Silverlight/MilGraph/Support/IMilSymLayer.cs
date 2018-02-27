// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="IMilSymLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Interface for a map-independent map symbology layer.
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
    using System.Collections.Generic;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interface for a map-independent map symbology layer.
    /// </summary>
    public interface IMilSymLayer
    {
        // Properties

        /// <summary>
        /// Gets a value indicating whether the given layer can currently be seen on the map.
        /// This is a poor-man's operation since it doesn't actually interest the map and layer visibility rectangles.
        /// Instead it checks the existence and visibility of the map and the layer.
        /// </summary>
        bool CanBeSeen { get; }                // can this layer be seen right now

        /// <summary>
        /// Gets the zoom level for the map as defined by Microsoft's Bing maps. The equivalent zoom level
        /// is returned for Esri's map.
        /// </summary>
        double ZoomLevel { get; }            // the map's current zoom level

        /// <summary>
        /// Gets the bounding rectangle that contains the elements drawn on the map layer.
        /// </summary>
        ILocationRect LayerExtent { get; }    // the rectangle containing the layer

        /// <summary>
        /// Gets the bounding rectangle that contains the elements drawn on the map.
        /// </summary>
        ILocationRect MapExtent { get; }    // the rectangle containing the map

        /// <summary>
        /// Returns the point associated with the passed in event
        /// </summary>
        /// <typeparam name="T">The type of the event argument</typeparam>
        /// <param name="ea">The event argument</param>
        /// <returns>The point associated with the passed in event</returns>
        Point EventToPoint<T>(T ea);

        /// <summary>
        /// Returns a list of children associated with the layer.
        /// </summary>
        IList<DependencyObject> ChildList { get; }

        /// <summary>
        /// Finds some (but maybe not all) of the map elements associated with a point.
        /// </summary>
        /// <param name="pos">The point at which to check for associated map elements</param>
        /// <returns></returns>
        IEnumerable<UIElement> ElementsAtPoint(Point pos);

        /// <summary>
        /// Returns the latitude and longitude corresponding to a screen point.
        /// </summary>
        /// <param name="p">
        /// The screen point.
        /// </param>
        /// <returns>The latitude and longitude corresponding to the screen point.</returns>
        ILocation PointToLocation(Point p);

        /// <summary>
        /// Adds a locatable UIElement to the map layer.
        /// </summary>
        /// <param name="mg">
        /// The locatable UIElement to add.
        /// </param>
        void AddSymbol(ILocatable mg);

        /// <summary>
        /// Adds a UIElement to the map layer.
        /// </summary>
        /// <param name="ui">
        /// The ILocation representing the location of the UIElement.
        /// </param>
        /// <param name="loc">
        /// The UIElement to add to the map layer at the given location.
        /// </param>
        void AddSymbol(UIElement ui, ILocation loc);
    }
}