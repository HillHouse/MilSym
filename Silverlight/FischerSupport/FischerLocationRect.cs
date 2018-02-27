// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="FischerLocationRect.cs">
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

namespace MilSym.FischerSupport
{
#if SILVERLIGHT

#else
    using MapControl;
#endif
    using MilGraph.Support;
    using System;

    /// <summary>
    /// Defines a map rectangular region for unions and intersections.
    /// </summary>
    public class FischerLocationRect : BoundingBox, ILocationRect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FischerLocationRect"/> class.
        /// </summary>
        /// <param name="one">
        /// The southwest location.
        /// </param>
        /// <param name="two">
        /// The northeast location.
        /// </param>
        public FischerLocationRect(ILocation one, ILocation two) :
            base(Math.Min(one.Latitude, two.Latitude), 
                Math.Min(one.Longitude, two.Longitude),
                Math.Max(one.Latitude, two.Latitude),
                Math.Max(one.Longitude, two.Longitude))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FischerLocationRect"/> class.
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
        public FischerLocationRect(double west, double south, double east, double north) :
            base(Math.Min(south, north),
                Math.Min(west, east),
                Math.Max(south, north),
                Math.Max(west, east))
        {
        }

        public FischerLocationRect(BoundingBox rect) :
            base(rect.South, rect.West, rect.North, rect.East)
        {
        }

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
