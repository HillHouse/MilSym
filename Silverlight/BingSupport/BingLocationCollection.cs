// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingLocationCollection.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Generate an ILocationCollection compatible with Bing maps.
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
    using System.Collections.Generic;
#if SILVERLIGHT
    using Microsoft.Maps.MapControl;
#else
    using Microsoft.Maps.MapControl.WPF;    // a big shout-out to Microsoft
#endif
    using MilGraph.Support;

    /// <summary>
    /// Generate an ILocationCollection compatible with Bing maps.
    /// </summary>
    public class BingLocationCollection : LocationCollection, ILocationCollection
    {
        /// <summary>
        /// Gets a value indicating whether the collection is IsReadOnly - which is always false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the location at the indicated index.
        /// </summary>
        /// <param name="index">
        /// The index to be gotten or set.
        /// </param>
        /// <returns>The Bing location as an ILocation.</returns>
        public new ILocation this[int index]
        {
            get
            {
                return base[index] as BingLocation;
            }

            set
            {
                base[index] = value as BingLocation;
            }
        }

        /// <summary>
        /// Check to see if the location collection contains a particular ILocation.
        /// </summary>
        /// <param name="location">
        /// The location to check.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the collection contains the location.
        /// </returns>
        public bool Contains(ILocation location)
        {
            var bing = location as BingLocation;
            if (bing == null)
            {
                return false;
            }

            return base.Contains(bing);
        }

        /// <summary>
        /// Copies to the target location starting at index.
        /// </summary>
        /// <param name="target">
        /// The target collection for copying.
        /// </param>
        /// <param name="index">
        /// The target index to start copying.
        /// </param>
        public void CopyTo(ILocation[] target, int index)
        {
            ILocation[] bing = target as BingLocation[];
            if (bing == null)
            {
                return;
            }

            this.CopyTo(bing, index);
        }

        /// <summary>
        /// Return the first index of a given location.
        /// </summary>
        /// <param name="location">
        /// The location for which to find the first index.
        /// </param>
        /// <returns>
        /// The index of the location in the collection, or -1.
        /// </returns>
        public int IndexOf(ILocation location)
        {
            return base.IndexOf(location as BingLocation);
        }

        /// <summary>
        /// Inserts the given location at the given index.
        /// </summary>
        /// <param name="index">
        /// The index at which to insert.
        /// </param>
        /// <param name="location">
        /// The location to be inserted.
        /// </param>
        public void Insert(int index, ILocation location)
        {
            var bing = location as BingLocation;
            if (bing == null)
            {
                return;
            }

            base.Insert(index, bing);
        }

        /// <summary>
        /// Remove a location from the collection.
        /// </summary>
        /// <param name="location">
        /// The location to be removed.
        /// </param>
        /// <returns>
        /// True if the operation is successful. 
        /// </returns>
        public bool Remove(ILocation location)
        {
            return base.Remove(location as BingLocation);
        }

        /// <summary>
        /// Adds a location to the collection.
        /// </summary>
        /// <param name="location">
        /// The location to add.
        /// </param>
        public void Add(ILocation location)
        {
            this.Add(location as Location);
        }

        /// <summary>
        /// Return an enumerator for the location collection.
        /// </summary>
        /// <returns>
        /// An enumerator for the location collection.
        /// </returns>
        public new IEnumerator<ILocation> GetEnumerator()
        {
            return new BingLocationEnumerator(this);
        }
    }
}
