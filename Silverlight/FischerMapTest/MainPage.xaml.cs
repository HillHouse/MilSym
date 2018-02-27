// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MainPage.xaml.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Standard MainPage.xaml.cs file plus setup for Fischer maps.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FischerMapTest
{
#if SILVERLIGHT
    using System.Windows;
#elif WINDOWS_UWP
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif
    using MapTest;
    using MilSym.FischerSupport;
    using MilSym.MilGraph;
    using MilSym.MilGraph.Support;

    /// <summary>
    /// Covered in TestMapDrawing.cs.
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// The ViewModel for the application - doesn't seem to work in UWP's XAML.
        /// </summary>
        public MapViewModel ViewModel { get; } = new MapViewModel();

        /// <summary>
        /// The factory for generating location and other objects.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// The contents for the map layers.
        /// </summary>
        private TestMapDrawing tdm;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            MilGraphic.MilSymFactory = this.milsymFactory = new FischerMilSymFactory();
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
                Fischer.Children.Add(ml as UIElement);
            }

            var pl = this.milsymFactory.PolyLayer();
            if (pl is UIElement)
            {
                Fischer.Children.Add(pl as UIElement);
            }

            this.tdm = new TestMapDrawing(this.milsymFactory, ml, pl);
#if WINDOWS_UWP
            Fischer.PointerReleased += this.tdm.MsMouseLeftButtonUp;
#else
            Fischer.MouseLeftButtonUp += this.tdm.MsMouseLeftButtonUp;
#endif
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
        private void MapNotLoaded(object sender /*, LoadingErrorEventArgs e*/)
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
