// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="TestMapDrawing.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   The generic map test drawing code used for Bing and ESRI Silverlight and WPF.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace MapTest
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
#if WINDOWS_UWP
    using System.Diagnostics;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
#endif
    using MilSym.MilGraph;
    using MilSym.MilGraph.Support;
    using MilSym.MilSymbol;

    /// <summary>
    /// The generic map test drawing code used for Bing Silverlight and ESRI Silverlight and WPF.
    /// </summary>
    public class TestMapDrawing
    {
        /// <summary>
        /// A counter to assist in varying symbol types during the animation.
        /// </summary>
        private static int nextCodeCount;

        /// <summary>
        /// The character used to denote that a symbol is "present".
        /// </summary>
        private static char present = 'P';

        /// <summary>
        /// Starting value for a counter to be used to shift mobility codes through the point symbology set during the animation.
        /// </summary>
        private static int counterOffset;

        /// <summary>
        /// The map factory for generating locations and other map dependent items.
        /// </summary>
        private readonly IMilSymFactory milsymFactory;

        /// <summary>
        /// The map layer for displaying symbols.
        /// </summary>
        private readonly IMilSymLayer milsymLayer;

        /// <summary>
        /// The map layer for displaying polygons and polylines.
        /// </summary>
        private readonly IPolyLayer polyLayer;

        /// <summary>
        /// A list of mobility and echelon values to apply to a set of point symbols
        /// </summary>
        private readonly string[] mobilityEchelon = new[]
        {
            "H-", "HB", "**", "E-", "F-", "G-", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "NS", "NL", "DI", "EH",
            "FG", "CF", "BE", "AD"
        };

        /// <summary>
        /// A list of point symbols to be animated
        /// </summary>
        private readonly string[] singlePointSymbolCodes = new string[]
        {
            "SUPAS-----*****", "SFPPS-----*****", "SNPPS-----*****", "SHPPS-----*****",
            "SUAPMF----*****", "SFAPMF----*****", "SNAPMF----*****", "SHAPMF----*****",
            "SUGPUCVC--*****", "SFGPUCVC--*****", "SNGPUCVC--*****", "SHGPUCVC--*****",
            "SUGPUH----*****", "SFGPUH----*****", "SNGPUH----*****", "SHGPUH----*****",
            "SUGPEWDHS-*****", "SFGPEWDHS-*****", "SNGPEWDHS-*****", "SHGPEWDHS-*****",
            "SUGPIMFPW-H****", "SFGPIMFPW-H****", "SNGPIMFPW-H****", "SHGPIMFPW-H****",
            "SUSPCL----*****", "SFSPCL----*****", "SNSPCL----*****", "SHSPCL----*****",
            "SUUPSK----*****", "SFUPSK----*****", "SNUPSK----*****", "SHUPSK----*****",
            "SUUPWMMX--*****", "SFUPWMMX--*****", "SNUPWMMX--*****", "SHUPWMMX--*****",
            "SUFPNB----*****", "SFFPNB----*****", "SNFPNB----*****", "SHFPNB----*****",
            "IUPPSRE-----***", "IFPPSRE-----***", "INPPSRE-----***", "IHPPSRE-----***",
            "IUAPSRE-----***", "IFAPSRE-----***", "INAPSRE-----***", "IHAPSRE-----***",
            "IUGPSCP-----***", "IFGPSCP-----***", "INGPSCP-----***", "IHGPSCP-----***",
            "IUUPSCS-----***", "IFUPSCS-----***", "INUPSCS-----***", "IHUPSCS-----***",
            "OUGPA-----*****", "OFGPA-----*****", "ONGPA-----*****", "OHGPA-----*****",
            "EUOPAA----*****", "EFOPAA----*****", "ENOPAA----*****", "EHOPAA----*****",
            "EUOPAB----*****", "EFOPAB----*****", "ENOPAB----*****", "EHOPAB----*****",
            "EUOPAC----H****", "EFOPAC----H****", "ENOPAC----H****", "EHOPAC----H****",
            "SUXPEVCAM-*****", "SFXPEVCAM-*****", "SNXPEVCAM-*****", "SHXPEVCAM-*****",
            "SUFPGPA---*****", "SFFPGPA---*****", "SNFPGPA---*****", "SHFPGPA---*****",
            "EUFPMA----*****", "EFFPMA----*****", "ENFPMA----*****", "EHFPMA----*****",
            "OUVPA-----*****", "OFVPA-----*****", "ONVPA-----*****", "OHVPA-----*****",
            "SUUPSB----*****", "SFUPSB----*****", "SNUPSB----*****", "SHUPSB----*****",
            "SUXPEVUR--*****", "SFXPEVUR--*****", "SNXPEVUR--*****", "SHXPEVUR--*****"
        };

        /// <summary>
        /// List of MapMilSymbols to be used for the animation
        /// </summary>
        private readonly List<Tuple<MapMilSymbol, ILocation>> mapMilSymbols = new List<Tuple<MapMilSymbol, ILocation>>();

        /// <summary>
        /// Timer for the animation.
        /// </summary>
        private DispatcherTimer dt = new DispatcherTimer();

        /// <summary>
        /// Flag to indicate whether symbols 
        /// </summary>
        private bool isAnimated;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TestMapDrawing"/> class.
        /// </summary>
        /// <param name="milsymFactory">
        /// The instance of the map factory.
        /// </param>
        /// <param name="ml">
        /// The symbology layer.
        /// </param>
        /// <param name="pl">
        /// The polygon/polyline layer.
        /// </param>
        public TestMapDrawing(IMilSymFactory milsymFactory, IMilSymLayer ml, IPolyLayer pl)
        {
            this.milsymFactory = milsymFactory;
            this.milsymLayer = ml;
            this.polyLayer = pl;
        }

        /// <summary>
        /// This is the workhorse method that generates both tactical and point symbology.
        /// </summary>
        public void DrawStuff()
        {
            MilBrush.ColorScheme = ColorSchemeProperty.Dark;

            // Some multi-point locations for some tactical graphics
            var quadLatLons = new[]
            {
                new Point(60.02, -70.3), new Point(60.0, -70.2), new Point(60.05, -70.25), new Point(60.1, -70.15),
                new Point(60.11, -70.25), new Point(60.03, -70.285)
            };
            var quads = this.PointsToCollection(quadLatLons);
            this.RotateCollection(quads, new RotateTransform { Angle = 90, CenterX = 60.11, CenterY = -70.10 });

            // Some triple point locations for some tactical graphics
            var triadLatLons = new[] { new Point(24.65, -70.0), new Point(24.5, -70.0), new Point(24.575, -70.135) };
            var triads = this.PointsToCollection(triadLatLons);
            this.RotateCollection(triads, new RotateTransform { Angle = 90, CenterX = 24.45, CenterY = -70.14 });

            var qgs = new string[] // quadruple point symbols
            {
                "GUGPGLB---****X", "GUMPOAR---****X", "GUMPOADC--****X", "GUMPOADU--****X", "GUMPOGR---****X",
                "GUMPOGF---****X", "GUMPOGZ---****X", "GUMPOGL---****X", "GUMPOGB---****X", "GUGPOAA---****X",
                "GUGPOLP---****X", "GUGPOLF---****X", "GUGPOLL---****X", "GUGPOLT---****X", "GUGPOLC---****X",
                "GUGPOLAR--****X", "GUGPOLAV--****X", "GUGPOLAGS-****X", "GUGPOLAGM-****X", "GUGPPA----****X",
                "GHGPAAW---****X", "GNGPGAD---****X", "GFGPGAF---****X", "GFGPAAF---****X", "GFGPGLL---****X",
                "GFGPGLP---****X", "GFGPGLF---****X", "GHTPK-----****X", "GHTPKF----****X", "GHGPGAD---****X",
                "GHGPGAG---****X", "GHGPGAA---****X", "GHGPGAE---****X", "GHGPGAX---****X", "GHGPGAL---****X",
                "GHGPGAP---****X", "GHTPR-----****X", "GHTPUS----****X", "GHSPASB---****X", "GHSPASD---****X",
                "GHSPAD----****X", "GHSPAE----****X", "GHSPAH----****X", "GHSPAR----****X", "GHSPASR---****X"
            };
            var tgs = new string[] // triple point symbols
            {
                "GHTPW-----****X", "GHTPWP----****X", "GHTPM-----****X", "GHTPL-----****X", "GHTPT-----****X",
                "GHTPQ-----****X", "GHTPH-----****X", "GHTPX-----****X", "GHMPORP---****X", "GHMPORS---****X",
                "GHMPORS---****X", "GHMPORA---****X", "GHGPOLI---****X", "GHMPOEB---****X", "GHMPBDE---****X",
                "GHMPBDD---****X", "GHMPBDI---****X", "GHMPBCE---****X", "GHMPBCD---****X", "GHTPJ-----****X",
                "GHTPC-----****X", "GHTPY-----****X", "GHTPB-----****X", "GHTPP-----****X", "GHTPS-----****X",
                "GHTPO-----****X", "GHTPE-----****X", "GHTPF-----****X", "GHTPA-----****X", "GHTPAS----****X",
                "GHMPOADC--****X", "GHMPOADU--****X", "GHMPOAW---****X", "GHMPOMC---****X", "GHMPOEF---****X",
                "GHMPOET---****X", "GHMPOED---****X", "GHGPGAS---****X", "GHGPDLP---****X", "GHGPOAF---****X",
                "GHGPSLA---****X", "GHMPBDI---****X", /*"GHMPORC---****X",*/ 
                "GHMPOT----****X", "GHGPPF----****X", "GHGPDLF---****X", "GHGPOLKA--****X", "GHGPOLKGM-****X",
                "GHGPOLKGS-****X", "GHMPBCF---****X", "GHMPBCL---****X", "GHMPBCR---****X", "GHMPSW----****X",
                "GHFPLT----****X", "GHFPLTS---****X", "GHFPLTF---****X", "GHSPLCM---****X", "GHSPLCH---****X",
                "GHOPHN----****X", "GHOPB-----****X", "GHOPBE----****X", "GHOPBA----****X", "GHOPBT----****X",
                "GHOPBO----****X", "GHTPUS----****X"
            };

            int graphicCount = 0;
            for (double i = 8; i < 16; i += 0.5)
            {
                for (double j = 0; j < 4; j += 0.5)
                {
                    if (graphicCount < tgs.Length)
                    {
                        this.PlotSymbol(tgs[graphicCount++], triads, new Point(j, i));
                    }
                    else if (graphicCount < tgs.Length + qgs.Length)
                    {
                        this.PlotSymbol(qgs[graphicCount++ - tgs.Length], quads, new Point(j, i));
                    }
                }
            }

            // Test plotting some regular, nonsense, military symbols in a bounding box
            var baseLoc = this.milsymFactory.Location(Order.LatLon, 42.73, -73.68);

            var lat = baseLoc.Latitude;
            var lon = baseLoc.Longitude;
            foreach (string sim in this.singlePointSymbolCodes)
            {
                var origin = this.milsymFactory.Location(Order.LatLon, lat, lon);

                // The labels will work too
                var ms = new MapMilSymbol(
                    sim,
                    opacity: 0.9,
                    origin: origin //, 
                    //labelString:"W=Hello;Q=135;X=World;H=Testing 1 2 3"
                    );
                if (ms.Bounds.IsEmpty)
                {
                    continue;
                }

                this.milsymLayer.AddSymbol(ms);
                this.mapMilSymbols.Add(new Tuple<MapMilSymbol, ILocation>(ms, origin));

                lon += 0.1;
                if (lon - baseLoc.Longitude > 1.5)
                {
                    lat -= 0.1;
                    lon = baseLoc.Longitude;
                }
            }

            // Add in the mobilityEchelon markings
            this.UpdateMapMilSymbols(null, null);
        }

        /// <summary>
        /// Reports the symbol code under the mouse, if any, at the time of the button up event.
        /// </summary>
        /// <param name="sender">
        /// The object reporting the mouse up event.
        /// </param>
        /// <param name="ea">
        /// The event args associated with the mouse up event.
        /// </param>
#if WINDOWS_UWP
        public void MsMouseLeftButtonUp(object sender, PointerRoutedEventArgs ea)
#else
        public void MsMouseLeftButtonUp(object sender, MouseButtonEventArgs ea)
#endif
        {
            var mapMilSymbol = sender as MapMilSymbol;
            var milGraphic = sender as MilGraphic;
            if (mapMilSymbol == null && milGraphic == null)
            {
#if SILVERLIGHT
                var ui = Application.Current.RootVisual;
                var elements =
                    VisualTreeHelper.FindElementsInHostCoordinates(ea.GetPosition(ui), ui);
                foreach (MapMilSymbol ele in elements.OfType<MapMilSymbol>())
                {
                    mapMilSymbol = ele;
                }
#elif WINDOWS_UWP
                var set = new HashSet<string>();
                var layer = this.milsymLayer;
                var pos = layer.EventToPoint<PointerRoutedEventArgs>(ea);
                var loc = layer.PointToLocation(pos);

                // First we use the conventional approach to find point elements
                // This will also work when selecting MilGraphics near their Origin
                // but not elsewhere.
                var elements = layer.ElementsAtPoint(pos);
                if (elements == null)
                {
                    return;
                }
                foreach (var element in elements)
                {
                    if (element is MapMilSymbol mms)
                    {
                        set.Add(mms.SymbolCode);
                    }
                    else if (element is MilGraphic mg)
                    {
                        set.Add(mg.SymbolCode);
                    }
                    else if (element is FrameworkElement hit)
                    {
                        var ele = (hit.DataContext != null) ? hit.DataContext as FrameworkElement : hit;
                        if (ele != null)
                        {
                            if (ele.Parent is MapMilSymbol mls)
                            {
                                set.Add(mls.SymbolCode);
                            }
                            else if (ele.Parent is MilGraphic mlg)
                            {
                                set.Add(mlg.SymbolCode);
                            }
                        }
                    }
                }

                foreach (var code in set)
                {
                    Debug.Write(code + " ");
                }
                Debug.WriteLine(" ");
#else
                var p = this.milsymLayer.EventToPoint<MouseButtonEventArgs>(ea);
                var elements = this.milsymLayer.ElementsAtPoint(p);
                if (elements == null)
                {
                    return;
                }
                foreach (var uie in elements)
                {
                    if (uie is FrameworkElement fe)
                    {
                        var ele = (fe.DataContext != null) ? fe.DataContext as FrameworkElement : fe;
                        if (ele != null)
                        {
                            if (ele.Parent is MapMilSymbol mms)
                            {
                                Console.WriteLine(mms.SymbolCode);
                            }
                            else if (ele.Parent is MilGraphic mg)
                            {
                                Console.WriteLine(mg.SymbolCode);
                            }
                        }
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Plots a 3+ point tactical graphic
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code to plot.
        /// </param>
        /// <param name="triads">
        /// The map locations for the graphic.
        /// </param>
        /// <param name="offset">
        /// An offset used to displace the graphic.
        /// </param>
        public void PlotSymbol(string symbolCode, ILocationCollection[] triads, Point offset)
        {
            var sc = symbolCode;
            foreach (var old in triads)
            {
                sc = NextCode(sc);

                var lc = this.milsymFactory.LocationCollection();
                foreach (var loc in old)
                {
                    lc.Add(this.milsymFactory.Location(Order.LatLon, loc.Latitude - offset.Y, loc.Longitude + offset.X));
                }

                var saveLoc = this.milsymFactory.LocationCollection();

                // Test to make sure presentation is still consistent with points re-ordered
                if (lc.Count == 3)
                {
                    saveLoc.Add(lc[2]);
                    saveLoc.Add(lc[1]);
                    saveLoc.Add(lc[0]);
                }
                else
                {
                    foreach (var loc in lc)
                    {
                        saveLoc.Add(loc); // can't re-order with four points
                    }
                }

                var spline = sc.Contains("GFGPGLL");
                var pg = new MilGraphic(
                    sc,
                    saveLoc,
                    labelOffset: new Offset(-0.25, 0.25),
                    labelString: "T=Hello;T1=Denver;X=2000' AGL;X1=4000' AGL",
                    labelW: new DateTime(2010, 12, 18).ToString(CultureInfo.InvariantCulture),
                    labelW1: new DateTime(2010, 12, 19).ToString(CultureInfo.InvariantCulture),
                    isSpline: spline);

                ToolTipService.SetToolTip(pg, sc);
                this.milsymLayer.AddSymbol(pg);

                // Draw the base and transformed base vectors
                //this.polyLayer.AddPolyline(saveLoc, new SolidColorBrush(Colors.Orange));
                //this.polyLayer.AddPolyline(lc, new SolidColorBrush(Colors.Purple));
            }
        }

        /// <summary>
        /// Changes the mobility/echelon/etc. of a symbol to show some animation features
        /// </summary>
        /// <param name="sender">
        /// The DispatcherTimer that caused the event.
        /// </param>
        /// <param name="ea">
        /// This parameter is not used.
        /// </param>
        /// 
#if WINDOWS_UWP
        public void UpdateMapMilSymbols(object sender, object ea)
#else
        public void UpdateMapMilSymbols(object sender, EventArgs ea)
#endif
        {
            var dtick = sender as DispatcherTimer;
            if (dtick != null)
            {
                dtick.Stop();
            }

            int total = this.mapMilSymbols.Count;
            var count = counterOffset++;
            for (int i = 0; i < total; i++)
            {
                Tuple<MapMilSymbol, ILocation> tuple = this.mapMilSymbols[i];
                var s = tuple.Item1;

                // if (counterOffset == 1)      // uncomment this line if you only want to see symbols move
                {
                    // Randomly change the symbol codes
                    var sim = this.singlePointSymbolCodes[i]; 
                    s.SymbolCode = (sim[10] == 'H' || sim[5] == 'H')
                                       ? sim
                                       : sim.Substring(0, 10) +
                                         this.mobilityEchelon[count++ % this.mobilityEchelon.Length] + "***";
                    s.Wrap();
                }

                // Randomly move the symbols around
                ILocation loc = tuple.Item2;
                var angle = counterOffset * Math.PI / 30.0;
                s.Origin = this.milsymFactory.Location(
                    Order.LonLat, 
                    loc.Longitude + (0.03 * Math.Sin(angle)), 
                    loc.Latitude + (0.03 * Math.Cos(angle)));
            }

            if (dtick != null && this.isAnimated)
            {
                dtick.Start();
            }
        }

        /// <summary>
        /// Method to turn animation of point symbols on and off
        /// </summary>
        public void ToggleAnimation()
        {
            if (this.isAnimated == false)
            {
                this.isAnimated = true;
                this.dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500) };
                this.dt.Tick += this.UpdateMapMilSymbols;
                this.dt.Start();
            }
            else
            {
                this.isAnimated = false;
                this.dt.Stop();
            }
        }

        /// <summary>
        /// Cycle through some affiliation/present/anticipated codes
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code to modify.
        /// </param>
        /// <returns>
        /// The modified symbol code.
        /// </returns>
        private static string NextCode(string symbolCode)
        {
            char newChar = ' ';
            switch (symbolCode[1])
            {
                case 'F':
                    newChar = 'H';
                    break;
                case 'H':
                    newChar = 'N';
                    break;
                case 'N':
                    newChar = 'U';
                    break;
                case 'U':
                    newChar = 'F';
                    break;
            }

            // For now, let's stick with Present only since Anticipated puts quite a load on my GPU
            if (nextCodeCount++ % 4 == 0)
            {
                present = /*(_present == 'P') ? 'A' :*/ 'P';
            }

            return symbolCode.Substring(0, 1) + newChar +
            symbolCode.Substring(2, 1) + present +
            symbolCode.Substring(4, 6) + "-J" +
            symbolCode.Substring(12);
        }

        /// <summary>
        /// Convert a list of Points into a collection of Locations using the factory.
        /// </summary>
        /// <param name="pts">
        /// The list of Points.
        /// </param>
        /// <returns>
        /// The collection of Locations.
        /// </returns>
        private ILocationCollection[] PointsToCollection(IEnumerable<Point> pts)
        {
            var locs = new ILocationCollection[4];
            locs[0] = this.milsymFactory.LocationCollection();
            foreach (var p in pts)
            {
                locs[0].Add(this.milsymFactory.Location(Order.LatLon, p.X, p.Y));
            }

            return locs;
        }

        /// <summary>
        /// Rotates a polyline locations by 90 degrees, three times, and adds the results to the collection.
        /// </summary>
        /// <param name="quads">
        /// The location collection to transform.
        /// </param>
        /// <param name="rt">
        /// The rendering transform, a rotation.
        /// </param>
        private void RotateCollection(ILocationCollection[] quads, RotateTransform rt)
        {
            for (int i = 0; i < 3; i++)        
            {
                // Rotate around three times.
                var locOut = this.milsymFactory.LocationCollection();
                var locIn = quads[i];
#if WINDOWS_UWP
                foreach (Point pointOut in locIn.Select(q => new Point(q.Latitude, q.Longitude)).Select(rt.TransformPoint))
#else
                foreach (Point pointOut in locIn.Select(q => new Point(q.Latitude, q.Longitude)).Select(rt.Transform))
#endif
                {
                    locOut.Add(this.milsymFactory.Location(Order.LatLon, pointOut.X, pointOut.Y));
                }

                quads[i + 1] = locOut;
            }
        }
    }
}