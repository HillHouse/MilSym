// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MainScreen.xaml.cs">
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

namespace BingMapTest
{
    using System.Windows;
    using MapTest;
#if SILVERLIGHT
    using Microsoft.Maps.MapControl;
#else
    using Microsoft.Maps.MapControl.WPF;
#endif
    using MilSym.BingSupport;
    using MilSym.MilGraph;
    using MilSym.MilGraph.Support;

    /// <summary>
    /// Covered in TestMapDrawing.cs
    /// </summary>
    public partial class MainScreen
    {
        /// <summary>
        /// The factory for generating location and other objects.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// The contents for the map layers.
        /// </summary>
        private TestMapDrawing tdm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainScreen"/> class.
        /// </summary>
        public MainScreen()
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

            this.tdm = new TestMapDrawing(this.milsymFactory, ml, pl);
            Bing.MouseLeftButtonUp += this.tdm.MsMouseLeftButtonUp; 
            this.tdm.DrawStuff();
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
            // Do nothing
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
