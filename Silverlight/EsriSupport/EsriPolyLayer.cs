// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="EsriPolyLayer.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Methods supporting IPolyLayer for use by the Esri maps.
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
    using System.Windows.Media;
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Geometry;
    using ESRI.ArcGIS.Client.Symbols;
    using MilGraph.Support;
    using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;
    using Transform = ESRI.ArcGIS.Client.Bing.Transform;

    /// <summary>
    /// Methods supporting IPolyLayer for use by the Esri maps.
    /// </summary>
    public class EsriPolyLayer : GraphicsLayer, IPolyLayer
    {
        /// <summary>
        /// Adds a polyline to this map layer.
        /// </summary>
        /// <param name="lc">
        /// The location collection that defines the polyline.
        /// </param>
        /// <param name="br">
        /// The brush used to render the polyline.
        /// </param>
        public void AddPolyline(ILocationCollection lc, Brush br)
        {
            var pc = new PointCollection();
            foreach (var p in lc)
            {
                pc.Add(Transform.GeographicToWebMercator(p as MapPoint));
            }

            var polyline = new Polyline();
            polyline.Paths.Add(pc);
            var graphic = new Graphic
            {
                Symbol = new SimpleLineSymbol { Color = br },
                Geometry = polyline
            };
            Graphics.Add(graphic);
        }
    }
}
