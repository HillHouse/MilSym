// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingLocation.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Generate an ILocation compatible with Microsoft Bing maps.
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
    using MilGraph.Support;

    /// <summary>
    /// Generate an ILocation compatible with Microsoft Bing maps.
    /// </summary>
    public class BingLocation : ILocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BingLocation"/> class.
        /// </summary>
        /// <param name="latitude">
        /// The latitude to set.
        /// </param>
        /// <param name="longitude">
        /// The longitude to set.
        /// </param>
        public BingLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BingLocation"/> class.
        /// </summary>
        /// <param name="location">
        /// The location to set.
        /// </param>
        public BingLocation(ILocation location)
        {
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            Altitude = location.Altitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
    }
}
