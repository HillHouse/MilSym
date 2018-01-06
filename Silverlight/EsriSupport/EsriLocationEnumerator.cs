// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriLocationEnumerator.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Enumerate an ILocationCollection compatible with Esri maps.
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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using MilGraph.Support;

    /// <summary>
    /// Standard class for enumerating the EsriLocationCollection.
    /// </summary>
    public class EsriLocationEnumerator : IEnumerator<ILocation>
    {
        /// <summary>
        /// The collection to enumerate.
        /// </summary>
        private readonly EsriLocationCollection collection;

        /// <summary>
        /// The current index into the collection.
        /// </summary>
        private int curIndex;

        /// <summary>
        /// The current value from the collection.
        /// </summary>
        private EsriLocation curLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsriLocationEnumerator"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection for enumeration support.
        /// </param>
        public EsriLocationEnumerator(EsriLocationCollection collection)
        {
            this.collection = collection;
            this.curIndex = -1;
            this.curLocation = default(EsriLocation);
        }

        /// <summary>
        /// Gets the current location from the collection.
        /// </summary>
        public ILocation Current
        {
            get { return this.curLocation; }
        }

        /// <summary>
        /// Gets the current location from the collection.
        /// </summary>
        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Move to the next location in the collection.
        /// </summary>
        /// <returns>
        /// A boolean indicating whether the current location has moved.
        /// </returns>
        public bool MoveNext()
        {
            // Avoid going beyond the end of the collection.
            if (++this.curIndex >= this.collection.Count)
            {
                return false;
            }

            // Set current location to next item in collection.
            this.curLocation = this.collection[this.curIndex] as EsriLocation;
            return true;
        }

        /// <summary>
        /// Reset the enumerator.
        /// </summary>
        public void Reset()
        {
            this.curIndex = -1;
        }

        /// <summary>
        /// Dispose as necessary.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}