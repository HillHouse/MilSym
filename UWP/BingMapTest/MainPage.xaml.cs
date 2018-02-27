using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;

using MilSym.BingSupport;
using MilSym.MilGraph;
using MilSym.MilGraph.Support;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BingMapTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// The factory for generating location and other objects.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// The contents for the map layers.
        /// </summary>
        private MapTest.TestMapDrawing tdm;

        public MainPage()
        {
            this.InitializeComponent();
            MilGraphic.MilSymFactory = this.milsymFactory = new BingMilSymFactory();
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            BingMap.Center = new Geopoint(new BasicGeoposition() { Latitude = 35.0, Longitude = -70.0, Altitude = 0.0 });
            BingMap.ZoomLevel = 4;
            BingMap.DesiredPitch = 45;
            BingMap.ZoomLevelChanged += this.UpdateLayout;

            var ml = this.milsymFactory.MilSymLayer();
            if (ml is UIElement)
            {
                BingMap.Children.Add(ml as UIElement);
            }

            var pl = this.milsymFactory.PolyLayer();
            if (pl is UIElement)
            {
                BingMap.Children.Add(pl as UIElement);
            }

            this.tdm = new MapTest.TestMapDrawing(this.milsymFactory, ml, pl);
            BingMap.PointerReleased += this.tdm.MsMouseLeftButtonUp;
            this.tdm.DrawStuff();
        }

        public void UpdateLayout(object sender, object e)
        {
            foreach (var child in BingMap.Children)
            {
                if (child is Border bdr)
                {
                    if (bdr.Child is MapMilSymbol mms)
                    {
                        mms.SymbolLayoutUpdated(sender, e);
                    }
                    else if (bdr.Child is MilGraphic mg)
                    {
                        mg.CanvasLayoutUpdated(sender, e);
                    }
                }
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
