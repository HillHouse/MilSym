// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ILocationRect.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Interface for a map-based location rectangle.
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
    /// Interface for a map-based location rectangle.
    /// </summary>
    public interface ILocationRect
    {
        /// <summary>
        /// Gets or sets the northernmost (top) latitude value.
        /// </summary>
        double North { get; set; }

        /// <summary>
        /// Gets or sets the westernmost (left) longitude value.
        /// </summary>
        double West { get; set; }

        /// <summary>
        /// Gets or sets the southernmost (bottom) latitude value.
        /// </summary>        
        double South { get; set; }

        /// <summary>
        /// Gets or sets easternmost (right) longitude value.
        /// </summary>
        double East { get; set; }

        /// <summary>
        /// This is a simple comparison of latitude and longitude values of "rectangles"
        /// whose sides are parallel to lines of latitude and longitude.
        /// </summary>
        /// <param name="rect">
        /// The rectangle to be compared against the current rectangle.
        /// </param>
        /// <returns>
        /// A boolean indicating whether or not the passed in rectangle intersects the current rectangle.
        /// </returns>
        bool Intersects(ILocationRect rect);

        /// <summary>
        /// This is a simple comparison of latitude and longitude values of "rectangles"
        /// whose sides are parallel to lines of latitude and longitude.
        /// </summary>
        /// <param name="rect">
        /// The rectangle to be compared against the current rectangle.
        /// </param>
        /// <returns>
        /// A boolean indicating whether or not the passed in rectangle is contained in the current rectangle.
        /// </returns>
        bool Contains(ILocationRect rect);
    }
}