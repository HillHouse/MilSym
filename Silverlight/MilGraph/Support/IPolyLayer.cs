// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="IPolyLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Interface for a map-independent map polyline and polygon layer.
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
    using System.Windows.Media;

    /// <summary>
    /// Interface for a map-independent map polyline and polygon layer.
    /// </summary>
    public interface IPolyLayer
    {
        /// <summary>
        /// Add a polyline to the polyline and polygon layer.
        /// </summary>
        /// <param name="lc">
        /// The location collection representing the coordinates of the polyline.
        /// </param>
        /// <param name="br">
        /// The brush with which to draw the polyline.
        /// </param>
        void AddPolyline(ILocationCollection lc, Brush br);
    }
}
