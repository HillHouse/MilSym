// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriLocationRect.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support for ILocationRect when using the Esri maps.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.EsriSupport
{
    using ESRI.ArcGIS.Client.Geometry;
    using MilGraph.Support;

    /// <summary>
    /// Support for ILocationRect when using the Esri maps.
    /// </summary>
    public class EsriLocationRect : Envelope, ILocationRect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EsriLocationRect"/> class.
        /// </summary>
        /// <param name="one">
        /// The southwest corner of the rectangle.
        /// </param>
        /// <param name="two">
        /// The northeast corner of the rectangle.
        /// </param>
        public EsriLocationRect(MapPoint one, MapPoint two) : 
            base(one.X, one.Y, two.X, two.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EsriLocationRect"/> class.
        /// </summary>
        /// <param name="e">
        /// The ESRI-specific Envelope containing extent coordinates.
        /// </param>
        public EsriLocationRect(Envelope e) : base(e.XMin, e.YMin, e.XMax, e.YMax)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EsriLocationRect"/> class.
        /// </summary>
        /// <param name="west">
        /// The west, leftmost, coordinate.
        /// </param>
        /// <param name="south">
        /// The south, bottom most, coordinate.
        /// </param>
        /// <param name="east">
        /// The east, rightmost, coordinate.
        /// </param>
        /// <param name="north">
        /// The north, top most, coordinate.
        /// </param>
        public EsriLocationRect(double west, double south, double east, double north) :
            base(west, south, east, north)
        {
        }

        /// <summary>
        /// Gets or sets the north coordinate.
        /// </summary>
        public double North
        {
            get
            {
                return this.YMax;
            }

            set
            {
                this.YMax = value;
            }
        }

        /// <summary>
        /// Gets or sets the west coordinate.
        /// </summary>
        public double West
        {
            get
            {
                return this.XMin;
            }

            set
            {
                this.XMin = value;
            }
        }

        /// <summary>
        /// Gets or sets the south coordinate.
        /// </summary>
        public double South
        {
            get
            {
                return this.YMin;
            }

            set
            {
                this.YMin = value;
            }
        }

        /// <summary>
        /// Gets or sets east coordinate.
        /// </summary>
        public double East
        {
            get
            {
                return this.XMax;
            }

            set
            {
                this.XMax = value;
            }
        }
 
        /// <summary>
        /// Grow the ILocationRect by the new ILocationRect. This is a poor man's - 
        /// it is stupid around international dateline but we're not really using it yet.
        /// </summary>
        /// <param name="lr">
        /// The ILocationRect by which to expand.
        /// </param>
        public void Expand(ILocationRect lr)
        {
            if (this.XMax < lr.East)
            {
                this.XMax = lr.East;
            }

            if (this.YMax < lr.North)
            {
                this.YMax = lr.North;
            }

            if (this.XMin > lr.West)
            {
                this.XMin = lr.West;
            }

            if (this.YMin > lr.South)
            {
                this.YMin = lr.South;
            }
        }

        /// <summary>
        /// Grow the ILocationRect by the new ILocation. This is a poor man's - 
        /// it is stupid around international dateline but we're not really using it yet.
        /// </summary>
        /// <param name="loc">
        /// The ILocation by which to expand.
        /// </param>
        public void Expand(ILocation loc)
        {
            if (this.XMax < loc.Longitude)
            {
                this.XMax = loc.Longitude;
            }

            if (this.YMax < loc.Latitude)
            {
                this.YMax = loc.Latitude;
            }

            if (this.XMin > loc.Longitude)
            {
                this.XMin = loc.Longitude;
            }

            if (this.YMin > loc.Latitude)
            {
                this.YMin = loc.Latitude;
            }
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
        /// Returns whether the passed in ILocationRect intersects this ILocationRect.
        /// </summary>
        /// <param name="lr">
        /// The ILocationRect to check.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the rectangles intersect.
        /// </returns>
        public bool Intersects(ILocationRect lr)
        {
            return this.Intersects(lr as Envelope);
        }
    }
}
