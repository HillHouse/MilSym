// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MainPage.xaml.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Standard MainPage.xaml.cs file plus setup for Bing maps.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicUpdateTest
{
    using System.Windows;
    using Microsoft.Maps.MapControl;
    using MilSym.BingSupport;
    using MilSym.MilGraph;
    using MilSym.MilGraph.Support;

    /// <summary>
    /// Test for dynamic updates to tactical graphics.
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// The factory for generating location and other objects.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            MilGraphic.MilSymFactory = this.milsymFactory = new BingMilSymFactory();
        }

        /// <summary>
        /// Do the real work after the map is loaded.
        /// </summary>
        /// <param name="sender">
        /// This parameter is not used.
        /// </param>
        /// <param name="e">
        /// This parameter is also not used.
        /// </param>
        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            var ml = this.milsymFactory.MilSymLayer();
            if (ml is UIElement)
            {
                Bing.Children.Add(ml as UIElement);
            }

            var pl = this.milsymFactory.PolyLayer();
            if (pl is UIElement)
            {
                Bing.Children.Add(pl as UIElement);
            }

            Bing.ZoomLevel = 10;
            var center = this.milsymFactory.Location(Order.LatLon, 24.7, -70.0);
            Bing.Center = center as Location;

            var tdm = new TestDynamic(this.milsymFactory, ml, pl);
            tdm.DrawStuff();
        }

        /// <summary>
        /// Handle missing credentials by doing nothing.
        /// </summary>
        /// <param name="sender">
        /// This parameter is not used.
        /// </param>
        /// <param name="e">
        /// This parameter is also not used.
        /// </param>
        private void MapNotLoaded(object sender, LoadingErrorEventArgs e)
        {
            // do nothing
        }
    }
}
