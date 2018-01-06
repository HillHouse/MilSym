// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriLocationCollection.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support for ILocationCollection when using the Esri maps.
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
    using System.Collections.Generic;
    using ESRI.ArcGIS.Client.Geometry;
    using MilGraph.Support;

    /// <summary>
    /// Support for ILocationCollection when using the Esri maps.
    /// </summary>
    public class EsriLocationCollection : PointCollection, ILocationCollection
    {
        /// <summary>
        /// Gets a value indicating whether the collection is readonly - always false in this case.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the location at the indicated index.
        /// </summary>
        /// <param name="index">
        /// The index for setting or getting the location.
        /// </param>
        /// <returns>The location as an ILocation.</returns>
        public new ILocation this[int index]
        {
            get
            {
                return base[index] as EsriLocation;
            }

            set
            {
                base[index] = value as EsriLocation;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether a location is in this location collection.
        /// </summary>
        /// <param name="location">
        /// The location to check for inclusion.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the collection contains the location.
        /// </returns>
        public bool Contains(ILocation location)
        {
            var esri = location as EsriLocation;
            return esri != null && base.Contains(esri);
        }
        
        /// <summary>
        /// Copies the contents of the current ILocationCollection to the target ILocation array.
        /// </summary>
        /// <param name="target">
        /// The target ILocation array.
        /// </param>
        /// <param name="index">
        /// The starting index for copying.
        /// </param>
        public void CopyTo(ILocation[] target, int index)
        {
            ILocation[] esri = target as EsriLocation[];
            if (esri == null)
            {
                return;
            }

            this.CopyTo(esri, index);
        }

        /// <summary>
        /// Returns the first index of the location in the location collection, or -1 if not found.
        /// </summary>
        /// <param name="location">
        /// The location to find in the location collection.
        /// </param>
        /// <returns>
        /// The first index of the location in the location collection, or -1 if not found.
        /// </returns>
        public int IndexOf(ILocation location)
        {
            return base.IndexOf(location as EsriLocation);
        }

        /// <summary>
        /// Inserts the given location at the given index.
        /// </summary>
        /// <param name="index">
        /// The index for insertion.
        /// </param>
        /// <param name="location">
        /// The location to insert.
        /// </param>
        public void Insert(int index, ILocation location)
        {
            var esri = location as EsriLocation;
            if (esri == null)
            {
                return;
            }

            base.Insert(index, esri);
        }

        /// <summary>
        /// Removes the first instance of the indicated location.
        /// </summary>
        /// <param name="location">
        /// The location to be removed.
        /// </param>
        /// <returns>
        /// A boolean indicating if the location was removed.
        /// </returns>
        public bool Remove(ILocation location)
        {
            return base.Remove(location as EsriLocation);
        }

        /// <summary>
        /// Adds a location to a location collection.
        /// </summary>
        /// <param name="location">
        /// The location to add.
        /// </param>
        public void Add(ILocation location)
        {
            var esri = location as EsriLocation;
            if (esri == null)
            {
                return;
            }

            base.Add(esri);
        }

        /// <summary>
        /// Returns an enumerator for the collection.
        /// </summary>
        /// <returns>
        /// The requested enumerator.
        /// </returns>
        public new IEnumerator<ILocation> GetEnumerator()
        {
            return new EsriLocationEnumerator(this);
        }
    }
}
