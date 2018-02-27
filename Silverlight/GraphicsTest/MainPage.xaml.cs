// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MainPage.xaml.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Main page and test code for drawing all single point symbology.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace GraphicsTest
{
#if SILVERLIGHT
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Windows.Threading;
#elif WINDOWS_UWP
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Security.Cryptography.Core;
    using Windows.Foundation;
    using Windows.Storage;
    using Windows.UI;
    using Windows.UI.ViewManagement;
    using Windows.UI.Popups;
    using Windows.UI.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
#else
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Windows.Threading;
#endif
    using MilSym.LoadResources;
    using MilSym.MilSymbol;
    using MilSym.MilSymbol.Schemas;

    /// <summary>
    /// This is a simple test to illustrate some of the features of the military symbols in Appendix D.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// A parameter value used for placing the symbols on the page.
        /// </summary>
        private const int Edge = 30;

        /// <summary>
        /// A parameter value used for placing the symbols on the page.
        /// </summary>
        private const int Loose = 75;

        /// <summary>
        /// A parameter value used for placing the symbols on the page.
        /// </summary>
        private const int Tight = 50;

        /// <summary>
        /// A parameter value used for placing the symbols on the page.
        /// </summary>
        private const int Wide = 100;

        /// <summary>
        /// The symbol size for the pages.
        /// </summary>
        private const double Scale = 0.14; // EqualAreas 36, EqualHeights 40, NativeSize 0.14

        /// <summary>
        /// The scale type that matches the Scale value.
        /// </summary>
        private const ScaleTypeValues ScaleType = ScaleTypeValues.NativeSize;

        /// <summary>
        /// The message logger
        /// </summary>
        private static readonly ILogger Log = LoggerFactory<MainPage>.GetLogger();

        /// <summary>
        /// The four basic affiliations used in the display of the point symbols.
        /// </summary>
        private static readonly string[] CommonAffiliation = { "U", "F", "H", "N" };

#if SILVERLIGHT
        /// <summary>
        /// The configuration settings for the application. Not currently used.
        /// </summary>
        private static readonly IsolatedStorageSettings AppSettings =
            IsolatedStorageSettings.ApplicationSettings;
#elif WINDOWS_UWP
        /// <summary>
        /// The configuration settings for the application. Not currently used.
        /// </summary>
        private static readonly ApplicationDataContainer AppSettings =
            ApplicationData.Current.LocalSettings;
#else
        /// <summary>
        /// The configuration settings for the application. Not currently used.
        /// </summary>
        private static readonly KeyValueConfigurationCollection AppSettings =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
#endif

        /// <summary>
        /// A dictionary of previously recorded hash values to compare images against. Not currently used.
        /// </summary>
        private static readonly IDictionary<string, string> Hashes = new Dictionary<string, string>();

        /// <summary>
        /// A string representative of the last plot that was displayed.
        /// </summary>
        private static string lastPlot;

        /// <summary>
        /// The rotating symbol that is displayed at the start of the program.
        /// </summary>
        private MilSymbol milSymbol;

        /// <summary>
        /// The color scheme, Dark, Medium, or Light to use when rendering.
        /// </summary>
        private ColorSchemeProperty colorSchemeProperty;

        /// <summary>
        /// A timer used to drive the initially rotating symbol.
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
#if WINDOWS_UWP
            ApplicationView.PreferredLaunchViewSize = new Size(640, 607);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
#endif
        }

        /// <summary>
        /// A helper function for creating a gradient brush.
        /// </summary>
        /// <param name="col">The base color.</param>
        /// <param name="stop">The gradient stop.</param>
        /// <returns>A gradient stop to use in the construction of a gradient brush.</returns>
        private static GradientStop CreateGradientStop(Color col, double stop)
        {
            return new GradientStop { Color = col, Offset = stop };
        }

        /// <summary>
        /// Compares the current image with a list of hashes.
        /// Allows user to update the hash in the case of a mismatch.
        /// </summary>
        /// <param name="lastKnownPlot">the hash to compare</param>
        /// <param name="source">the Canvas (or UIElement) to compare</param>
#if WINDOWS_UWP
        private async static void CheckHash(string lastKnownPlot, UIElement source)
#else
        private static void CheckHash(string lastKnownPlot, UIElement source)
#endif
        {
            if (lastKnownPlot == null || lastKnownPlot == "Check")
            {
#if WINDOWS_UWP
                                    var dialog = new MessageDialog("Pick a test before comparing its checksum");
                                    await dialog.ShowAsync();
#else
                MessageBox.Show("Pick a test before comparing its checksum");
#endif
                return;
            }

#if WINDOWS_UWP
            var imageArray = await GetByteArray(source);
#else
            var imageArray = GetByteArray(source);
#endif
            string hash = ComputeHash(imageArray);

            if (hash == Hashes[lastKnownPlot])
            {
#if WINDOWS_UWP
                                    var dialog = new MessageDialog("Checksum is OK", "Match");
                                    dialog.Commands.Clear();
                                    dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                                    await dialog.ShowAsync();
#else
                MessageBox.Show("Checksum is OK", "Match", MessageBoxButton.OK);
#endif
            }
            else
            {
#if WINDOWS_UWP
                                    var mb = new MessageDialog("Checksum doesn't match, which is normal for the first invocation only. Make this the new checksum?", "Mismatch");
                                    mb.Commands.Clear();
                                    mb.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                                    mb.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                                    var res = await mb.ShowAsync();
                                    if ((int)res.Id == 0)
                                    {
                                        AppSettings.Values[lastPlot] = Hashes[lastPlot] = hash;
                                    }
#else
                var mb =
                    MessageBox.Show(
                        "Checksum doesn't match, which is normal for the first invocation only. Make this the new checksum?",
                        "Mismatch",
                        MessageBoxButton.OKCancel);
                if (mb == MessageBoxResult.OK)
                {
#if SILVERLIGHT
                                        AppSettings[lastKnownPlot] = Hashes[lastKnownPlot] = hash;
#else
                    AppSettings[lastPlot].Value = Hashes[lastPlot] = hash;
#endif
                }
#endif
            }
        }

        /// <summary>
        /// Computes a hash that can be used as a checksum to make sure an image
        /// hasn't changed as a result of editing changes to the MilSym library.
        /// Unfortunately the hash is render dependent. Haven't checked to see if
        /// using WritableBitmap.Render would produce a computer independent hash.
        /// </summary>
        /// <param name="message">the non-white bytes in the screen image</param>
        /// <returns>the hash string</returns>
        private static string ComputeHash(byte[] message)
        {
#if WINDOWS_UWP
            var hap = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hashValue = hap.HashData(message.AsBuffer());
            return hashValue.ToArray().Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
#else
            var hashString = new SHA256Managed();

            byte[] hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
#endif
        }

        /// <summary>
        /// Gets the current canvas - not really necessary but illustrates FindName
        /// </summary>
        /// <returns>The current canvas.</returns>
        private static Canvas GetCanvas()
        {
#if SILVERLIGHT
            var uc = Application.Current.RootVisual as UserControl;
            if (uc == null)
            {
                return null;
            }

            return uc.FindName("Symbols") as Canvas;
#elif WINDOWS_UWP
            return Window.Current.Content
                .GetVisuals().OfType<Canvas>()
                .Where<Canvas>(c => c.Name == "Symbols")
                .FirstOrDefault<Canvas>();
#else
            return LogicalTreeHelper.FindLogicalNode(Application.Current.MainWindow, "Symbols") as Canvas;
#endif
        }

        /// <summary>
        /// Generates a static tooltip for a given symbol code. 
        /// </summary>
        /// <param name="symbolCode">The symbol code which needs the tooltip.</param>
        /// <returns>A string representing a tooltip.</returns>
        private static string GenerateTooltip(string symbolCode)
        {
            var desc = new StringBuilder(MilAppendix.Description(symbolCode));
            desc.AppendLine("Affiliation: " + StandardIdentity.GetName(symbolCode));
            desc.AppendLine("Condition: " + StatusOperationalCapacity.GetName(symbolCode));
            desc.AppendLine("Order of battle: " + OrderOfBattle.GetName(symbolCode));
            desc.AppendLine("Country: " + Countries.GetName(symbolCode));
            desc.Append("Modifier: " + CombinedModifierCode.GetName(symbolCode));
            return desc.ToString(0, desc.Length);
        }

        /// <summary>
        /// Plots a symbol to the canvas.
        /// </summary>
        /// <param name="cv">The canvas.</param>
        /// <param name="ms">The military symbol.</param>
        /// <param name="x">The x location on the canvas.</param>
        /// <param name="y">The y location on the canvas.</param>
        private static void DrawSymbol(Panel cv, MilSymbol ms, double x, double y)
        {
            if (!ms.Empty)
            {
                ms.SetValue(Canvas.TopProperty, y);
                ms.SetValue(Canvas.LeftProperty, x);

                // Not necessarily a good idea, in general, to pre-generate a bunch of tooltips
                ToolTipService.SetToolTip(ms, ms.SymbolCode + "\n" + GenerateTooltip(ms.SymbolCode));
                cv.Children.Add(ms);
            }
        }

        /// <summary>
        /// Plot a Miscellaneous collection of symbols to extend code coverage.
        /// </summary>
        private static void PlotMiscellany()
        {
            // Create a diagonal linear gradient with four stops.   
            var linBrush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            linBrush.GradientStops.Add(CreateGradientStop(Color.FromArgb(255, 255, 255, 160), 0.0));
            linBrush.GradientStops.Add(CreateGradientStop(Color.FromArgb(255, 255, 160, 160), 0.4));
            linBrush.GradientStops.Add(CreateGradientStop(Color.FromArgb(255, 160, 160, 255), 0.6));
            linBrush.GradientStops.Add(CreateGradientStop(Color.FromArgb(255, 160, 255, 160), 1.0));

#if WINDOWS_UWP
            var uri = new Uri("ms-appx:///Assets/IcelandFlag64.png", UriKind.Absolute);
            var bmi = new BitmapImage(uri);
#else
            StreamResourceInfo sr = Application.GetResourceStream(
                new Uri("Resources/IcelandFlag64.png", UriKind.Relative));
            if (sr == null)
            {
                Log.WriteMessage(
                    LogLevel.Error,
                    "Cannot find the IcelandFlag64.png resource. Should be in GraphicsTest/Resources.");
                return;
            }
            var bmi = new BitmapImage();
#endif

#if SILVERLIGHT
            bmi.SetSource(sr.Stream);
#elif !WINDOWS_UWP
            bmi.BeginInit();
            bmi.StreamSource = sr.Stream;
            bmi.EndInit();
#endif
            var imageBrush = new ImageBrush { ImageSource = bmi, Opacity = 0.5, Stretch = Stretch.UniformToFill };

            Canvas cv = GetCanvas();
            if (cv == null)
            {
                return;
            }

            double x = 50;
            double y = 50;

            var affiliations = new[] { "U", "A", "N", "H" };
            foreach (string ee in affiliations)
            {
                string sc = "I" + ee + "GPSRU----F***";

                var ms = new MilSymbol(sc, Scale, null, new SolidColorBrush(Colors.Cyan), linBrush);
                DrawSymbol(cv, ms, x, y);

                MilBrush.ColorScheme = ColorSchemeProperty.Dark;
                ms = new MilSymbol(sc, Scale, null, null);
                DrawSymbol(cv, ms, x += 50, y);

                MilBrush.ColorScheme = ColorSchemeProperty.Medium;
                ms = new MilSymbol(sc, Scale, null, new SolidColorBrush(Colors.Orange));
                DrawSymbol(cv, ms, x += 50, y);

                MilBrush.ColorScheme = ColorSchemeProperty.Light;
                ms = new MilSymbol(sc, Scale, null, null);
                DrawSymbol(cv, ms, x += 50, y);

                sc = "I" + ee + "GDSRU----J***";
                ms = new MilSymbol(sc, Scale, null, null, linBrush);
                DrawSymbol(cv, ms, x += 50, y);

                sc = "I" + ee + "GXSRU----N***";
                ms = new MilSymbol(sc, Scale, null, null, linBrush);
                DrawSymbol(cv, ms, x += 50, y);

                sc = "I" + ee + "ZPSRU----D***";
                ms = new MilSymbol(sc, Scale, null, null, imageBrush);
                DrawSymbol(cv, ms, x += 50, y);

                sc = "I" + ee + "ZASRU----B**C";
                ms = new MilSymbol(sc, Scale, null, new SolidColorBrush(Colors.Green));

                // ReSharper disable RedundantAssignment
                DrawSymbol(cv, ms, x += 50, y);

                // ReSharper restore RedundantAssignment
                x = 50;
                y += 75;
            }

            // Cloud cover
            x = 0;
            for (int i = 0; i < 10; i++)
            {
                var ms = new MilSymbol("WAS-WC----P----", Scale, "AA=" + i);
                DrawSymbol(cv, ms, x += 50, y);
            }

            y += 75;

            // Wind barbs with no speeds
            x = 0;
            for (int i = 0; i < 10; i++)
            {
                var ms = new MilSymbol("WAS-WP----P----", 2.0 * Scale, "AA=" + i + ";Q=" + (i * 36));
                DrawSymbol(cv, ms, x += 50, y);
            }

            y += 75;

            // Wind barbs with speeds
            x = 0;
            for (int i = 0; i < 10; i++)
            {
                string labels = "AA=" + i + ";Q=135;" + "Z=" + (i * 12.6);
                if (i > 5)
                {
                    labels += ";Y=S";
                }

                var ms = new MilSymbol("WAS-WP----P----", 2.0 * Scale, labels);
                DrawSymbol(cv, ms, x += 50, y);
            }

            y += 75;

            var brushes = new[] { new SolidColorBrush(Colors.Black), null, new SolidColorBrush(Colors.Magenta) };
            x = 0;
            for (int i = 0; i < 3; i++)
            {
                var newScale = (i == 0) ? Scale : -Scale;
                var ms = new MilSymbol("WAS-WSTSD-P----", newScale, null, brushes[i], brushes[i]);
                DrawSymbol(cv, ms, x += 50, y);
                ms = new MilSymbol("WAS-WSTSS-P----", newScale, null, brushes[i], brushes[i]);
                DrawSymbol(cv, ms, x += 50, y);
                ms = new MilSymbol("WAS-WSTSH-P----", newScale, null, brushes[i], brushes[i]);
                DrawSymbol(cv, ms, x += 50, y);
            }

            var dms = new MilSymbol("WOS-HDS---P----", Scale, "X=67.8");

            // ReSharper disable RedundantAssignment
            DrawSymbol(cv, dms, x += 50, y);

            // ReSharper restore RedundantAssignment
        }

        /// <summary>
        /// A test to make sure that all of the supported labels go into the right
        /// places and do the right things.
        /// </summary>
        private static void PlotAllLabels()
        {
            // Example using Run: <Run Foreground='Purple'>KK</Run> 
            var labels = new[]
             {
                                     "C=12345;F=±", "G=GG;H=HH", "J=JJ;K=KK", "L=LL;M=MM", "N=NN;P=PP",
                                     "Q=30.6;T=TT", "V=VV;W=WW", "X=XX;Y=YY", "Z=ZZ;AA=ABCDEF",

                                     // Can't use = as separator in this string because of Foreground='Green', etc.
                                     "C:12345;F:±;G:Staff comments;H:H;J:J;" +
                                     "K:<Run Foreground='Green' FontWeight='Bold' FontStyle='Italic'>KK</Run>;" +
                                     "L:L;M:M;N:N;P:P;Q:30.6;" +
                                     "T:T;V:VvV;W:WwW;X:X;Y:Y;Z:ZzZzZ;AA:ABCDEF"
                                 };
            Canvas cv = GetCanvas();
            if (cv == null)
            {
                return;
            }

            var saveScheme = MilBrush.ColorScheme;
            MilBrush.ColorScheme = ColorSchemeProperty.Medium;

            double x = Tight;
            double y = Loose;

            var baseCode = new[] { "IUAASRU---", "SUGAEVAL--", "IUUASRU---" };
            var affiliations = new[] { "U", "F", "N", "H" };

            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily("Times New Roman")));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 84.0));
            style.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Brown)));

            int count = 0;
            var unknowns = new[] { "U", "W", "P", "G" };
            var friendlies = new[] { "F", "A", "D", "M", "J", "K" };
            var hostiles = new[] { "H", "S" };
            var neutrals = new[] { "N", "L" };
            foreach (string bd in baseCode)
            {
                foreach (string ee in affiliations)
                {
                    foreach (string m in labels)
                    {
                        string af = string.Empty;
                        switch (ee)
                        {
                            case "U":
                                af = unknowns[count % 4];
                                break;
                            case "F":
                                af = friendlies[count % 6];
                                break;
                            case "H":
                                af = hostiles[count % 2];
                                break;
                            case "N":
                                af = neutrals[count % 2];
                                break;
                        }

                        count++;
                        string sc = bd.Substring(0, 1) + af + bd.Substring(2, 8) + "--" + "***";
                        var ms = new MilSymbol(sc, Scale, m, null, null, style);
                        DrawSymbol(cv, ms, x, y);
                        if ((x += Wide) > 5 * Wide)
                        {
                            x = Tight;
                            y += Loose;
                        }
                    }
                }

                style = null;
            }

            MilBrush.ColorScheme = saveScheme;
        }

        /// <summary>
        /// A test to check out the various mobility settings and some echelon markings
        /// </summary>
        private static void PlotAllMobility()
        {
            Canvas cv = GetCanvas();
            if (cv == null)
            {
                return;
            }

            double x = Edge;
            double y = Tight;

            var affiliations = new[] { "U", "F", "N", "H" };
            var categoryBattleDimension = new[] { "A", "G", "U", "F" };
            var mob = new[]
              {
                                      "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY",
                                      "NS", "NL",
                                      "H-", "HB", "AA", "BC", "CE", "DG", "EI", "FK", "GM"
                                  };

            foreach (string bd in categoryBattleDimension)
            {
                foreach (string ee in affiliations)
                {
                    foreach (string m in mob)
                    {
                        string sc;
                        if (bd == "F")
                        {
                            sc = "S" + ee + bd + "AN-----" + m + "AF*";
                        }
                        else
                        {
                            sc = "I" + ee + bd + "ASRU---" + m + "AF*";
                        }

                        var ms = new MilSymbol(sc, Scale, null, null);
                        DrawSymbol(cv, ms, x, y);
                        if ((x += Tight) > 11 * Tight)
                        {
                            x = Edge;
                            y += Loose;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Plots all of the core symbols that are in Appendix D.
        /// Since not all symbol codes are non-blank, there are some blanks in the output.
        /// Refer to MIL-STD 2525C for comparison.
        /// </summary>
        /// <param name="append">
        /// The appendix from which to get the symbols.
        /// </param>
        /// <param name="framed">
        /// Whether or not the symbols are framed.
        /// </param>
        private static void PlotAllSymbols(string append, bool framed)
        {
            Canvas cv = GetCanvas();
            if (cv == null)
            {
                return;
            }

            // Since some appendices cross reference symbols,
            // in other appendices, we'll check to make sure 
            // we haven't already drawn a particular symbol code.
            IList<string> symList = new List<string>();

            double x = Edge;
            double y = Edge;

            var affiliations = new[] { "U", "F", "N", "H" };
            bool first = true;

            var keys = MilAppendix.Keys(append);
            foreach (string ap in keys)
            {
                string sc = ap;
                if (symList.Contains(sc))
                {
                    continue;
                }

                symList.Add(sc);

                // There is no affiliation for weather
                int schemeKey = CodingScheme.GetCode(sc);
                if (schemeKey == CodingScheme.Weather)
                {
                    // Check centering
                    // var ls1 = new Line { X1 = x + 5, Y1 = y + 5, X2 = x - 5, Y2 = y - 5, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1 };
                    // var ls2 = new Line { X1 = x - 5, Y1 = y + 5, X2 = x + 5, Y2 = y - 5, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1 };
                    // cv.Children.Add(ls1);
                    // cv.Children.Add(ls2);
                    var ms = new MilSymbol(sc, Scale, "X=67.8");
                    DrawSymbol(cv, ms, x, y);
                    if ((x += Tight) > 12 * Tight)
                    {
                        x = Edge;
                        y += Tight;
                    }

                    continue;
                }

                if (schemeKey == CodingScheme.TacticalGraphics)
                {
                    // Check centering
                    // var ls1 = new Line { X1 = x + 5, Y1 = y + 5, X2 = x - 5, Y2 = y - 5, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1 };
                    // var ls2 = new Line { X1 = x - 5, Y1 = y + 5, X2 = x + 5, Y2 = y - 5, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1 };
                    // cv.Children.Add(ls1);
                    // cv.Children.Add(ls2);
                    sc = sc.Substring(0, 1) + "H" + sc.Substring(2, 1) + "A" + sc.Substring(4);
                    var ms = new MilSymbol(sc, Scale, "H=HH;H1=H1;W=W;W1=W1;T=TT;N=N;X=XX;V=VV;C=CC;Y=YY;Q=-60.0");
                    DrawSymbol(cv, ms, x, y);
                    if ((x += Tight) > 12 * Tight)
                    {
                        x = Edge;
                        y += Tight + 25;
                    }

                    continue;
                }

                if (!framed)
                {
                    sc = sc.Substring(0, 2) + "X" + sc.Substring(3);
                }

                foreach (string c in affiliations)
                {
                    sc = sc[0] + c + sc[2] + "C" + sc.Substring(4, 10) + "E";

                    var ms = new MilSymbol(sc, Scale, null, null);
                    if (first && ms.Empty)
                    {
                        continue;
                    }

                    first = false;
                    DrawSymbol(cv, ms, x, y);

                    if ((x += Tight) > 12 * Tight)
                    {
                        x = Edge;
                        y += Tight;
                    }
                }
            }
        }

        /// <summary>
        /// Helper to read one of the hash codes into memory.
        /// </summary>
        /// <param name="setting">which hash code to read</param>
        /// <returns>the hash code</returns>
        private static string GetSetting(string setting)
        {
#if SILVERLIGHT
            if (AppSettings.Contains(setting))
            {
                return (string)AppSettings[setting];
            }
            AppSettings.Add(setting, null);
            return null;
#elif WINDOWS_UWP
            //if (AppSettings.Values.ContainsKey(setting))
            //{
            //    return (string)AppSettings.Values[setting];
            //}
            //AppSettings.Values[setting] = null;
            return null;
#else
            int len = AppSettings.AllKeys.GetLength(0);
            for (int i = 0; i < len; i++)
            {
                if (AppSettings.AllKeys[i] == setting)
                {
                    return AppSettings[setting].Value;
                }
            }
            AppSettings.Add(setting, null);
            return null;
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Returns a byte array representative of the image associated with the UIElement.
        /// </summary>
        /// <param name="source">The UIElement whose image is to be returned.</param>
        /// <returns>the hash stringThe byte array representing the image.</returns>
        private static byte[] GetByteArray(UIElement source)
        {
            var bm = new WriteableBitmap(source, null);
            int[] pix = bm.Pixels;
            int len = pix.Length;
            var imageArray = new byte[len * sizeof(int)];
            int count = 0;
            for (int i = 0; i < len; i++)
            {
                if (pix[i] != -1)
                {
                    imageArray[count++] = (byte)((pix[i] >> 24) & 0xff); // alpha
                    imageArray[count++] = (byte)((pix[i] >> 16) & 0xff); // red
                    imageArray[count++] = (byte)((pix[i] >> 8) & 0xff); // green
                    imageArray[count++] = (byte)(pix[i] & 0xff); // blue
                }
            }
            return imageArray;
        }
#elif WINDOWS_UWP
        /// <summary>
        /// Returns a byte array representative of the image associated with the UIElement.
        /// </summary>
        /// <param name="source">The UIElement whose image is to be returned.</param>
        /// <returns>the hash stringThe byte array representing the image.</returns>
        private async static Task<byte[]> GetByteArray(UIElement source)
        {
            var height = (int)source.RenderSize.Height;
            var width = (int)source.RenderSize.Width;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(source, width, height);
            var buffer = await renderTargetBitmap.GetPixelsAsync();
            return buffer.ToArray();
        }
#else
        /// <summary>
        /// Returns a byte array representative of the image associated with the UIElement.
        /// </summary>
        /// <param name="source">The UIElement whose image is to be returned.</param>
        /// <returns>the hash stringThe byte array representing the image.</returns>
        private static byte[] GetByteArray(UIElement source)
        {
            var height = (int)source.RenderSize.Height;
            var width = (int)source.RenderSize.Width;

            var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var sourceBrush = new VisualBrush(source);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(width, height)));
            }

            renderTarget.Render(drawingVisual);

            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            byte[] imageArray;
            using (var outputStream = new MemoryStream())
            {
                pngEncoder.Save(outputStream);
                imageArray = outputStream.ToArray();
            }

            return imageArray;
        }
#endif

        /// <summary>
        /// Reads in the hash codes and starts a timer to do a simple update.
        /// </summary>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is also not used.</param>
        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            // Set the scale type for the symbols
            MilSymbol.ScaleType = ScaleType;

            // Read any stored validation hash codes.
            // If the first time, there won't be any.
            foreach (object item in Combo.Items)
            {
                var cbi = item as ComboBoxItem;
                if (cbi != null)
                {
                    var content = cbi.Content as string;
                    if (content != null)
                    {
                        Hashes.Add(content, GetSetting(content));
                    }
                }
            }

            // There are also other buttons with potential validation hash codes. 
            Hashes.Add("Appendix", GetSetting("Appendix"));
            Hashes.Add("Labels", GetSetting("Labels"));
            Hashes.Add("Miscellany", GetSetting("Miscellany"));
            Hashes.Add("Mobility", GetSetting("Mobility"));

            this.timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50),
            };
            this.timer.Tick += this.TimerTick;
            this.timer.Start();
        }

        /// <summary>
        /// Simple test showing that changing some values causes symbol to update.
        /// </summary>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is also not used.</param>
        private void TimerTick(object sender, object e)
        {
            if (this.milSymbol == null)
            {
                this.milSymbol = new MilSymbol(
                    "IUGPSRU---DKCAE", labelString: "X=The default;V= label font;T=is Arial;H=World", scale: 2 * Scale)
                {
                    LabelG = "Hello"
                };
                DrawSymbol(GetCanvas(), this.milSymbol, 300, 300);
                return;
            }

            this.milSymbol.Angle += 2;
            int count = (int)(this.milSymbol.Angle % 360) / 36;
            this.milSymbol.SymbolCode = "I" + CommonAffiliation[count % 4] + "GPSRU---DKCAE";
            this.milSymbol.LabelString = "F=" + this.milSymbol.Angle + "°";

            // Should be changing tooltip here too
            this.milSymbol.SetValue(Canvas.TopProperty, 300 + (100 * Math.Sin(this.milSymbol.Angle * Math.PI / 180.0)));
            this.milSymbol.SetValue(Canvas.LeftProperty, 300 + (100 * Math.Cos(this.milSymbol.Angle * Math.PI / 180.0)));
        }

        /// <summary>
        /// The color scheme the example uses for unframed symbols.
        /// </summary>
        private void SetFrame()
        {
            this.colorSchemeProperty = MilBrush.ColorScheme;
            MilBrush.ColorScheme = ColorSchemeProperty.Dark;
        }

        /// <summary>
        /// Returns to the default color scheme property.
        /// </summary>
        private void UnsetFrame()
        {
            MilBrush.ColorScheme = this.colorSchemeProperty;
        }

        /// <summary>
        /// Capture and act upon the user's button click.
        /// </summary>
        /// <param name="choice">
        /// The button that was clicked.
        /// </param>
        /// <param name="match">
        /// The string (selection) that was matched.
        /// </param>
        private void MakeChoice(string choice, string match)
        {
            // Stop the rotating symbol
            this.timer.Stop();
            this.timer.Tick -= this.TimerTick;

            Canvas cv = GetCanvas();
            if (cv == null)
            {
                return;
            }

            cv.Height = 4400;
            cv.Children.Clear();
#if WINDOWS_UWP
                                sv.ChangeView(0.0d, 0.0d, 1.0f);
#else
            sv.ScrollToVerticalOffset(0.0);
#endif

            switch (choice)
            {
                case "Labels":
                    PlotAllLabels();
                    break;
                case "Appendix":
                    PlotAllSymbols(match, true);
                    break;
                case "NoFrame":
                    this.SetFrame();
                    PlotAllSymbols(match, false);
                    this.UnsetFrame();
                    break;
                case "Miscellany":
                    PlotMiscellany();
                    break;
                case "Mobility":
                    PlotAllMobility();
                    break;
            }
        }

        /// <summary>
        /// User wants to plot something.
        /// </summary>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is also not used.</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            if (b == null)
            {
                return;
            }

            this.MakeChoice((string)b.Content, string.Empty);
            lastPlot = (string)b.Content;
        }

        /// <summary>
        /// User clicked on Check button, so let's compute and compare a hash.
        /// </summary>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is also not used.</param>
        private void CheckClick(object sender, RoutedEventArgs e)
        {
            Canvas cv = GetCanvas();
            if (cv == null)
            {
                return;
            }

            CheckHash(lastPlot, cv);
        }

        /// <summary>
        /// Action to take when the user picks a different combo box.
        /// </summary>
        /// <param name="sender">
        /// The combobox.
        /// </param>
        /// <param name="e">
        /// This parameter is not used.
        /// </param>
        private void ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb == null ||
                cb.SelectedIndex == 0 ||
                !(cb.SelectedItem is ComboBoxItem))
            {
                return;
            }

            var content = ((ComboBoxItem)cb.SelectedItem).Content as string;
            var match = ((ComboBoxItem)cb.SelectedItem).Tag as string;
            if (content == null || match == null)
            {
                return;
            }

            if (content.Contains("no frame"))
            {
                this.MakeChoice("NoFrame", match);
                lastPlot = content;
            }
            else
            {
                this.MakeChoice("Appendix", match);
                lastPlot = content;
            }

            cb.SelectedIndex = 0;
        }

        /// <summary>
        /// Attach a callback to a loaded combobox.
        /// </summary>
        /// <param name="sender">
        /// The combobox.
        /// </param>
        /// <param name="e">
        /// This parameter is not used.
        /// </param>
        private void ComboLoaded(object sender, RoutedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb == null)
            {
                return;
            }

            cb.SelectedIndex = 0;
            cb.SelectionChanged += this.ComboSelectionChanged;
        }
    }
}