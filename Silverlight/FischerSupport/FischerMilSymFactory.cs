// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="FischerMilSymFactory.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Creates a map factory for locations, layers, and other map related objects.
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
    using MapControl;
    using MilGraph.Support;

    /// <summary>
    /// Creates a map factory for locations, layers, and other map related objects.
    /// </summary>
    public class FischerMilSymFactory : IMilSymFactory
    {
        /// <summary>
        /// Returns an ILocationCollection compatible with the Fischer map.
        /// </summary>
        /// <returns>
        /// An ILocationCollection.
        /// </returns>
        public ILocationCollection LocationCollection()
        {
            return new FischerLocationCollection();
        }

        /// <summary>
        /// Returns an "IMilSymLayer" compatible with the Fischer map.
        /// </summary>
        /// <returns>
        /// An "IMilSymLayer"
        /// </returns>
        public IMilSymLayer MilSymLayer()
        {
            return new FischerMilSymLayer();
        }

        /// <summary>
        /// Returns an IPolyLayer compatible with the Fischer map.
        /// </summary>
        /// <returns>
        /// An IPolyLayer.
        /// </returns>
        public IPolyLayer PolyLayer()
        {
            return new FischerPolyLayer();
        }

        /// <summary>
        /// Create one location from another.
        /// </summary>
        /// <param name="location">
        /// The location to be cloned.
        /// </param>
        /// <returns>
        /// The new ILocation object.
        /// </returns>
        public ILocation Location(ILocation location)
        {
            return new FischerLocation(location);
        }

        /// <summary>
        /// Generate a location based on a latitude and longitude.
        /// </summary>
        /// <param name="order">
        /// The order of the latitude and longitude, either "LatLon" or "LonLat"
        /// </param>
        /// <param name="a">
        /// The first parameter.
        /// </param>
        /// <param name="b">
        /// The second parameter.
        /// </param>
        /// <returns>
        /// The new ILocation.
        /// </returns>
        public ILocation Location(Order order, double a, double b)
        {
            return (order == Order.LatLon) ? new FischerLocation(a, b) : new FischerLocation(b, a);
        }

        /// <summary>
        /// Create a location rectangle based on two ILocations.
        /// </summary>
        /// <param name="one">
        /// The southwest corner of the rectangle.
        /// </param>
        /// <param name="two">
        /// The northeast corner of the rectangle.
        /// </param>
        /// <returns>
        /// The "ILocationRect"
        /// </returns>
        public ILocationRect LocationRect(ILocation one, ILocation two)
        {
            return new FischerLocationRect(one, two);
        }
    }
}