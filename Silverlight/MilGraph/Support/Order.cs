// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Order.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   An enumerated type to control the order of latitude and longitude in ILocation constructors.
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
    /// An enumerated type to control the order of latitude and longitude in ILocation constructors.
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// Order the coordinates as latitude followed by longitude.
        /// </summary>
        LatLon = 0,
        
        /// <summary>
        /// Order the coordinates as longitude followed by latitude.
        /// </summary>
        LonLat = 1
    }
}
