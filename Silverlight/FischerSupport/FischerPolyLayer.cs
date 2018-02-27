// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="FischerPolyLayer.cs">
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

namespace MilSym.FischerSupport
{
#if SILVERLIGHT
    using System.Windows.Media;
#elif WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using MapControl;
#else
    using System.Windows.Media;
    using MapControl;
#endif
    using MilGraph.Support;

    /// <summary>
    /// Provides map polyline layer services.
    /// </summary>
    public class FischerPolyLayer :  MapPanel, IPolyLayer
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
            base.Children.Add(new MapPolyline { Locations = lc as LocationCollection, Stroke = br });
        }
    }
}
