// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="FischerLocation.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Generate an ILocation compatible with Fischer maps.
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
#if SILVERLIGHT
#else
    using MapControl;
#endif
    using MilGraph.Support;

    /// <summary>
    /// Generate an ILocation compatible with Fischer maps.
    /// </summary>
    public class FischerLocation : Location, ILocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FischerLocation"/> class.
        /// </summary>
        /// <param name="latitude">
        /// The latitude to set.
        /// </param>
        /// <param name="longitude">
        /// The longitude to set.
        /// </param>
        public FischerLocation(double latitude, double longitude)
            : base(latitude, longitude)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FischerLocation"/> class.
        /// </summary>
        /// <param name="location">
        /// The location to set.
        /// </param>
        public FischerLocation(ILocation location)
            : base(location.Latitude, location.Longitude)
        {
            this.Altitude = location.Altitude;
        }

        /// <summary>
        /// Gets or sets the altitude for a location.
        /// </summary>
        public double Altitude { get; set; }
    }
}
