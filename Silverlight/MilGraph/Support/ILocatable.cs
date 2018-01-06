// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ILocatable.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   An interface to support a canvas object having a map location
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
    /// Interface to ensure that a UIElement can be placed on the map
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Gets the latitude and longitude values of the map location
        /// </summary>
        ILocation Origin { get; }
    }
}
