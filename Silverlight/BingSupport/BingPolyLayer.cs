// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BingPolyLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Provides map polyline layer services.
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
    using System.Windows.Media;
#if SILVERLIGHT
    using Microsoft.Maps.MapControl;
#else
    using Microsoft.Maps.MapControl.WPF;    // a big shout-out to Microsoft
#endif
    using MilGraph.Support;

    /// <summary>
    /// Provides map polyline layer services.
    /// </summary>
    public class BingPolyLayer : MapLayer, IPolyLayer
    {
        /// <summary>
        /// Adds a polyline to this map layer.
        /// </summary>
        /// <param name="lc">
        /// The collection of map locations.
        /// </param>
        /// <param name="br">
        /// The brush to use when rendering the polyline.
        /// </param>
        public void AddPolyline(ILocationCollection lc, Brush br)
        {
            Children.Add(new MapPolyline { Locations = lc as LocationCollection, Stroke = br });
        }
    }
}
