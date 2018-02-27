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
    using System;
    using MilGraph.Support;

    /// <summary>
    /// Defines a map rectangular region for unions and intersections.
    /// </summary>
    public class BingLocationRect : ILocationRect
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
        public BingLocationRect(ILocation one, ILocation two)
        {
            West = Math.Min(one.Longitude, two.Longitude);
            East = Math.Max(one.Longitude, two.Longitude);
            South = Math.Min(one.Latitude, two.Latitude);
            North = Math.Max(one.Latitude, two.Latitude);
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
            West = Math.Min(east, west);
            East = Math.Max(east, west);
            South = Math.Min(north, south);
            North = Math.Max(north, south);
        }

        public double West { get; set; }
        public double East { get; set; }
        public double North { get; set; }
        public double South { get; set; }

        /// <summary>
        /// Returns whether or not this rectangle contains the passed in rectangle.
        /// </summary>
        /// <param name="lr">
        /// The location rectangle to compare against the current instance.
        /// </param>
        /// <returns>
        /// A boolean indicating if this rectangle contains the passed in rectangle.
        /// </returns>
        public bool Contains(ILocationRect lr)
        {
            if (lr.West >= West &&
                lr.East <= East &&
                lr.South >= South &&
                lr.North <= North)
            {
                return true;
            }

            return false;
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
            if (lr.East < West || lr.West > East)
            {
                return false;
            }
            if (lr.North < South || lr.South > North)
            {
                return false;
            }

            return true;
        }
    }
}
