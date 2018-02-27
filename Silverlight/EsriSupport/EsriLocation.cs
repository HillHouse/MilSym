// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriLocation.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support for ILocation when using the Esri maps.
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
    /// Support methods to getting and setting ILocation.
    /// </summary>
    public class EsriLocation : MapPoint, ILocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EsriLocation"/> class.
        /// </summary>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        public EsriLocation(double longitude, double latitude)
            : base(longitude, latitude)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EsriLocation"/> class.
        /// </summary>
        /// <param name="location">
        /// The location to clone.
        /// </param>
        public EsriLocation(ILocation location)
            : base(location.Longitude, location.Latitude)
        {
            this.Altitude = location.Altitude;
        }

        /// <summary>
        /// Gets or sets the latitude for an ILocation.
        /// </summary>
        public double Latitude
        {
            get
            {
                return this.Y;
            }
        }

        /// <summary>
        /// Gets or sets the longitude for an ILocation.
        /// </summary>
        public double Longitude
        {
            get
            {
                return this.X;
            }
        }

        /// <summary>
        /// Gets or sets the altitude for a location.
        /// </summary>
        public double Altitude { get; set; }
    }
}
