// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ILocation.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Interface for a map-based location.
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
    /// Interface for a map-based location
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// Gets or sets the latitude value for the interface.
        /// </summary>
        double Latitude { get; }

        /// <summary>
        /// Gets or sets the longitude value for the interface.
        /// </summary>
        double Longitude { get; }
        
        /// <summary>
        /// Gets or sets the altitude value for the interface.
        /// </summary>
        double Altitude { get; }
    }
}