// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="IMilSymFactory.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Interface for a factory of map-independent interfaces.
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
    /// <summary>
    /// Interface for a factory of map-independent interfaces.
    /// </summary>
    public interface IMilSymFactory
    {
        /// <summary>
        /// Returns a map layer suitable for displaying map symbology.
        /// </summary>
        /// <returns>
        /// An interface to a map symbology compatible layer
        /// </returns>
        IMilSymLayer MilSymLayer();

        /// <summary>
        /// Returns a map layer suitable for displaying polygons and polylines.
        /// </summary>
        /// <returns>
        /// An interface to a map polygon and polyline compatible layer.
        /// </returns>
        IPolyLayer PolyLayer();

        /// <summary>
        /// Returns an interface to a map location interface.
        /// </summary>
        /// <param name="order">
        /// The order of the passed in coordinates, either Order.LatLon or Order.LonLat.
        /// </param>
        /// <param name="a">
        /// The first passed in coordinate.
        /// </param>
        /// <param name="b">
        /// The second passed in coordinate.
        /// </param>
        /// <returns>
        /// An interface to a location object reflecting the passed in location.
        /// </returns>
        ILocation Location(Order order, double a, double b);

        /// <summary>
        /// Returns an interface to a map location interface.
        /// </summary>
        /// <param name="location">
        /// The location to be duplicated.
        /// </param>
        /// <returns>
        /// An interface to a location object reflecting the passed in location.
        /// </returns>
        ILocation Location(ILocation location);

        /// <summary>
        /// Returns an interface to a map location bounding rectangle.
        /// </summary>
        /// <param name="one">
        /// The first corner location of the bounding rectangle, representing the southwest corner.
        /// </param>
        /// <param name="two">
        /// The second corner location of the bounding rectangle, representing the northeast corner.
        /// </param>
        /// <returns>
        /// An interface to a location rectangle object reflecting the passed in locations.
        /// </returns>
        ILocationRect LocationRect(ILocation one, ILocation two);

        /// <summary>
        /// Creates an interface to a location collection, used by multipoint symbology.
        /// </summary>
        /// <returns>
        /// An interface to the location collection.
        /// </returns>
        ILocationCollection LocationCollection();
    }
}
