// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingLocationRect.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Defines a map rectangular region for unions and intersections.
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
    /// Defines a map rectangular region for unions and intersections.
    /// </summary>
    public class BingLocationRect : LocationRect, ILocationRect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BingLocationRect"/> class.
        /// </summary>
        /// <param name="one">
        /// The southwest location.
        /// </param>
        /// <param name="two">
        /// The northeast location.
        /// </param>
        public BingLocationRect(ILocation one, ILocation two) : base(one as BingLocation, two as BingLocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BingLocationRect"/> class.
        /// </summary>
        /// <param name="west">
        /// The west, leftmost, location.
        /// </param>
        /// <param name="south">
        /// The south, bottom most, location.
        /// </param>
        /// <param name="east">
        /// The east, right most, location.
        /// </param>
        /// <param name="north">
        /// The north, top most, location.
        /// </param>
        public BingLocationRect(double west, double south, double east, double north)
        {
            this.West = west;
            this.South = south;
            this.East = east;
            this.North = north;
        }

        /// <summary>
        /// Returns whether or not two rectangles intersect one another.
        /// </summary>
        /// <param name="lr">
        /// The location rectangle to compare against the current instance.
        /// </param>
        /// <returns>
        /// A boolean indicating if the two rectangles intersect.
        /// </returns>
        public bool Intersects(ILocationRect lr)
        {
            return this.Intersects(lr as LocationRect);
        }
    }
}
