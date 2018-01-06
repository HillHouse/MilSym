// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MainWindow.xaml.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Provides the main page for the Esri map test.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace EsriMapTest
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Bing;
    
    using MapTest;
    
    using MilSym.EsriSupport;
    using MilSym.MilGraph;
    using MilSym.MilGraph.Support;
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The map factory for generating layers and locations.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// The Bing map token string required to support the Bing map layer backgrounds.
        /// </summary>
        private const string TokenString = "AtkLTSqCB4ENRmBqIL4UGVv_scxX0Bl6JNA4c8VXpHSaguSEw2UyfQRNsSNfZolk";

        /// <summary>
        /// The contents for the map layers.
        /// </summary>
        private TestMapDrawing tdm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MilGraphic.MilSymFactory = this.milsymFactory = new EsriMilSymFactory();
        }

        /// <summary>
        /// Add the map layers for the symbology test.
        /// </summary>
        /// <param name="sender">
        /// This parameter is not used.
        /// </param>
        /// <param name="e">
        /// This parameter is also not used.
        /// </param>
        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load a background layer
                var bingLayer = new TileLayer
                {
                    LayerStyle = TileLayer.LayerType.Road,
                    ServerType = ServerType.Production,
                    Visible = true,
                    Token = TokenString
                };
                if (!string.IsNullOrEmpty(TokenString))
                {
                    Esri.Layers.Add(bingLayer);
                }

                var ml = this.milsymFactory.MilSymLayer();
                if (ml is ElementLayer)
                {
                    Esri.Layers.Add(ml as ElementLayer);
                }

                var pl = this.milsymFactory.PolyLayer();
                if (pl is GraphicsLayer)
                {
                    Esri.Layers.Add(pl as GraphicsLayer);
                }

                this.tdm = new TestMapDrawing(this.milsymFactory, ml, pl);
                this.tdm.DrawStuff();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Responds to the Animate button to toggle animation on the map.
        /// </summary>
        /// <param name="sender">
        /// This parameter is not used.
        /// </param>
        /// <param name="e">
        /// This parameter is also not used.
        /// </param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            this.tdm.ToggleAnimation();
        }
    }
}
