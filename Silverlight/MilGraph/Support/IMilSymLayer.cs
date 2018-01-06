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
    using System.Windows;

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