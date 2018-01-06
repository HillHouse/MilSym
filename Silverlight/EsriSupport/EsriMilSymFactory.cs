// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriMilSymFactory.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   This factory returns basic layer and location objects for use by the Esri maps.
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
    using ESRI.ArcGIS.Client.Geometry;
    using MilGraph.Support;

    /// <summary>
    /// This factory returns basic layer and location objects for use by the Esri maps.
    /// </summary>
    public class EsriMilSymFactory : IMilSymFactory
    {
        /// <summary>
        /// Returns an instance of an ILocationCollection suitable for use by an Esri map.
        /// </summary>
        /// <returns>
        /// An ILocationCollection.
        /// </returns>
        public ILocationCollection LocationCollection()
        {
            return new EsriLocationCollection();
        }

        /// <summary>
        /// Returns an instance of an IMilSymLayer suitable for use by an Esri map.
        /// </summary>
        /// <returns>
        /// An IMilSymLayer.
        /// </returns>
        public IMilSymLayer MilSymLayer()
        {
            return new EsriMilSymLayer();
        }

        /// <summary>
        /// Returns an instance of an IPolyLayer suitable for use by an Esri map.
        /// </summary>
        /// <returns>
        /// An IPolyLayer.
        /// </returns>
        public IPolyLayer PolyLayer()
        {
            return new EsriPolyLayer();
        }

        /// <summary>
        /// Returns an instance of an ILocation suitable for use by an Esri map.
        /// </summary>
        /// <param name="location">
        /// The location to clone.
        /// </param>
        /// <returns>
        /// An ILocation.
        /// </returns>
        public ILocation Location(ILocation location)
        {
            return new EsriLocation(location);
        }

        /// <summary>
        /// Returns an instance of an ILocation suitable for use by an Esri map.
        /// </summary>
        /// <param name="order">
        /// The order of the latitude and longitude coordinates, either LatLon or LonLat.
        /// </param>
        /// <param name="a">
        /// The value of the first coordinate.
        /// </param>
        /// <param name="b">
        /// The value of the second coordinate.
        /// </param>
        /// <returns>
        /// An ILocation.
        /// </returns>
        public ILocation Location(Order order, double a, double b)
        {
            return order == Order.LonLat ? new EsriLocation(a, b) : new EsriLocation(b, a);
        }

        /// <summary>
        /// Returns an instance of an ILocationRect suitable for use by an Esri map.
        /// </summary>
        /// <param name="one">
        /// The southwest corner of the rectangle.
        /// </param>
        /// <param name="two">
        /// The northeast corner of the rectangle.
        /// </param>
        /// <returns>
        /// An ILocationRect.
        /// </returns>
        public ILocationRect LocationRect(ILocation one, ILocation two)
        {
            return new EsriLocationRect(one as MapPoint, two as MapPoint);
        }
    }
}