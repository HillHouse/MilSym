// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingLocation.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Generate an ILocation compatible with Bing maps.
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
#if SILVERLIGHT
    using Microsoft.Maps.MapControl;
#else
    using Microsoft.Maps.MapControl.WPF;    // a big shout-out to Microsoft
#endif
    using MilGraph.Support;

    /// <summary>
    /// Generate an ILocation compatible with Bing maps.
    /// </summary>
    public class BingLocation : Location, ILocation
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
            : base(latitude, longitude)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BingLocation"/> class.
        /// </summary>
        /// <param name="location">
        /// The location to set.
        /// </param>
        public BingLocation(ILocation location)
            : base(location as BingLocation)
        {
        }
    }
}
