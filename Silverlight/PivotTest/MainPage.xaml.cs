// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MainPage.xaml.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Standard AssemblyInfo.cs file
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

#define DO_WRITES

// This program has to be installed on the desktop before it is run.

// After running this program, run the following from a command line in the users Pictures directory:
// Pauthor.exe /source cxml Symbols.cxml /target deepzoom Symbols_dz.cxml

// A sample item in the CXML file should look like the following:

////<Item Id="0" Name="EUIPCD----" Img="Symbols\EUIPCD----.jpg" Href="">
////  <Description>
////    A fire affecting a home or housing complex.
////  </Description>
////  <Facets>
////    <Facet Name="Affiliation">
////      <String Value="Unknown" />
////    </Facet>
////    <Facet Name="Battle Dimension">
////      <String Value="Incident" />
////    </Facet>
////    <Facet Name="Type">
////      <String Value="None" />
////    </Facet>
////    <Facet Name="Coding Scheme">
////      <String Value="Emergency Management Symbols" />
////    </Facet>
////    <Facet Name="Key Words">
////      <String Value="Residential Fire" />
////    </Facet>
////  </Facets>
////</Item>

namespace PivotTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using ImageTools;
    using MilSym.MilSymbol;
    using MilSym.MilSymbol.Schemas;
    using Path = System.IO.Path;

    /// <summary>
    /// MainPage class for generating some sample CXML files.
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// An incrementing index used for counting.
        /// </summary>
        private int index;                            // we need some sort of index that increments

        /// <summary>
        /// The color scheme to used for unframed symbols
        /// </summary>
        private ColorSchemeProperty schemeProperty;        // need for unframed symbols

        /// <summary>
        /// The CXML output file.
        /// </summary>
        private StreamWriter cxmlStream;            // CXML output file

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the symbology color to dark.
        /// </summary>
        public void SetFrame()
        {
            this.schemeProperty = MilBrush.ColorScheme;
            MilBrush.ColorScheme = ColorSchemeProperty.Dark;
        }

        /// <summary>
        /// Restores the symbology color.
        /// </summary>
        public void UnsetFrame()
        {
            MilBrush.ColorScheme = this.schemeProperty;
        }

        /// <summary>
        /// Create the header for the CXML file.
        /// </summary>
        /// <returns>
        /// The header for the CXML file.
        /// </returns>
        private static string GetHeader()
        {
            var headerText = new[] 
            {
                @"<?xml version=""1.0"" encoding=""utf-8""?>",
                @"<Collection xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:p=""http://schemas.microsoft.com/livelabs/pivot/collection/2009"" Name=""MIL-STD 2525C"" SchemaVersion=""1.0"" xmlns=""http://schemas.microsoft.com/collection/metadata/2009"">",
                @"    <FacetCategories>",
                @"        <FacetCategory Name=""Affiliation"" Type=""String"" p:IsFilterVisible=""true"" p:IsMetaDataVisible=""true"" p:IsWordWheelVisible=""true"">",
                @"            <Extension>",
                @"                <p:SortOrder Name=""Affiliation"">",
                @"                    <p:SortValue Value=""Friend"" />",
                @"                    <p:SortValue Value=""Hostile"" />",
                @"                    <p:SortValue Value=""Neutral"" />",
                @"                    <p:SortValue Value=""Unknown"" />",
                @"                </p:SortOrder>",
                @"            </Extension>",
                @"        </FacetCategory>",
                @"        <FacetCategory Name=""Coding Scheme"" Type=""String"" p:IsFilterVisible=""true"" p:IsMetaDataVisible=""true"" p:IsWordWheelVisible=""true"">",
                @"            <Extension>",
                @"                <p:SortOrder Name=""Coding Scheme"">",
                @"                    <p:SortValue Value=""Warfighting"" />",
                @"                    <p:SortValue Value=""Tactical Graphics"" />",
                @"                    <p:SortValue Value=""Weather (METOC)"" />",
                @"                    <p:SortValue Value=""Intelligence"" />",
                @"                    <p:SortValue Value=""Stability Operations"" />",
                @"                    <p:SortValue Value=""Emergency Management"" />",
                @"                </p:SortOrder>",
                @"            </Extension>",
                @"        </FacetCategory>",
                @"        <FacetCategory Name=""Key Words"" Type=""String"" p:IsFilterVisible=""true"" p:IsMetaDataVisible=""true"" p:IsWordWheelVisible=""true"" />",
                @"        <FacetCategory Name=""Battle Dimension"" Type=""String"" p:IsFilterVisible=""true"" p:IsMetaDataVisible=""true"" p:IsWordWheelVisible=""true"">",
                @"            <Extension>",
                @"                <p:SortOrder Name=""Battle Dimension"">",
                @"                    <p:SortValue Value=""Air"" />",
                @"                    <p:SortValue Value=""Atmospheric"" />",
                @"                    <p:SortValue Value=""Combat Service Support"" />",
                @"                    <p:SortValue Value=""Command/Control and General Maneuver"" />",
                @"                    <p:SortValue Value=""Fire Support"" />",
                @"                    <p:SortValue Value=""Ground"" />",
                @"                    <p:SortValue Value=""Incident"" />",
                @"                    <p:SortValue Value=""Individual"" />",
                @"                    <p:SortValue Value=""Infrastructure"" />",
                @"                    <p:SortValue Value=""Items"" />",
                @"                    <p:SortValue Value=""Locations"" />",
                @"                    <p:SortValue Value=""Mobility Survivablity"" />",
                @"                    <p:SortValue Value=""Natural Events"" />",
                @"                    <p:SortValue Value=""Non-Military Group Or Organization"" />",
                @"                    <p:SortValue Value=""Oceanic"" />",
                @"                    <p:SortValue Value=""Operations"" />",
                @"                    <p:SortValue Value=""Other"" />",
                @"                    <p:SortValue Value=""Rape"" />",
                @"                    <p:SortValue Value=""Sea Subsurface"" />",
                @"                    <p:SortValue Value=""Sea Surface"" />",
                @"                    <p:SortValue Value=""SOF"" />",
                @"                    <p:SortValue Value=""Space"" />",
                @"                    <p:SortValue Value=""Tasks"" />",
                @"                    <p:SortValue Value=""Unknown"" />",
                @"                    <p:SortValue Value=""ViolentActivities"" />",
                @"                </p:SortOrder>",
                @"            </Extension>",
                @"        </FacetCategory>",
                @"        <FacetCategory Name=""Key Phrases"" Type=""String"" p:IsFilterVisible=""true"" p:IsMetaDataVisible=""true"" p:IsWordWheelVisible=""true"" />",
                @"        <FacetCategory Name=""Type"" Type=""String"" p:IsFilterVisible=""true"" p:IsMetaDataVisible=""true"" p:IsWordWheelVisible=""true"">",
                @"            <Extension>",
                @"                <p:SortOrder Name=""Type"">",
                @"                    <p:SortValue Value=""Equipment"" />",
                @"                    <p:SortValue Value=""Facility"" />",
                @"                    <p:SortValue Value=""None"" />",
                @"                    <p:SortValue Value=""Unit"" />",
                @"                </p:SortOrder>",
                @"            </Extension>",
                @"        </FacetCategory>",
                @"    </FacetCategories>"
            };
            var sb = new StringBuilder();
            foreach (var s in headerText)
            {
                sb.AppendLine(s);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Write out the files to be processed by cxml.
        /// </summary>
        /// <param name="sender">
        /// This parameter is not used.
        /// </param>
        /// <param name="e">
        /// This parameter is also not used.
        /// </param>
        private void WriteFiles(object sender, RoutedEventArgs e)
        {
#if DO_WRITES
            var symbols = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Symbols");
            if (!Directory.Exists(symbols))
            {
                Directory.CreateDirectory(symbols);
            }

            string facetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Symbols.cxml");
            this.cxmlStream = new StreamWriter(facetPath, false);
            this.cxmlStream.Write(GetHeader());
            this.cxmlStream.WriteLine("<Items>");
#endif

            //// Appendix A - Space "^S.P" space symbols
            //// Appendix A - Air "^S.A" air symbols
            //// Appendix A - Equipment "^S.G.E" ground equipment
            //// Appendix A - Equipment (no frame) "^S.G.E" ToolTipService.ToolTip="Draws some Appendix A ground equipment without frames
            //// Appendix A - Installation "^S.G.I" ground installations
            //// Appendix A - Unit (pt 1) "^S.G.U[CU]" units
            //// Appendix A - Unit (pt 2) "^S.G.U[S]" units
            //// Appendix A - Water "^S.S" surface water symbols
            //// Appendix A - Water (no frame) "^S.S" ToolTipService.ToolTip="Draws some Appendix A surface water symbols without frames
            //// Appendix A - Subsurface "^S.U" subsurface symbols
            //// Appendix A - Subsurface (no frame) "^S.U" ToolTipService.ToolTip="Draws some Appendix A subsurface symbols without frames
            //// Appendix A - SOF "^S.F" SOF symbols
            //// Appendix B "^G" ToolTipService.ToolTip="Draws all single point tactical symbols"  />
            //// Appendix C "^W.........P" ToolTipService.ToolTip="Draws all single point Weather (METOC) symbols"  />
            //// Appendix D "^I" ToolTipService.ToolTip="Draws all Intelligence symbols"  />
            //// Appendix E "^O" ToolTipService.ToolTip="Draws all Stability Operations symbols"  />
            //// Appendix G "^E" ToolTipService.ToolTip="Draws all Emergency Operations symbols"  />

            ////this.ProcessSymbolCollection("^S.P", true);
            ////this.ProcessSymbolCollection("^S.A", true);
            ////this.ProcessSymbolCollection("^S.G.E", true);
            ////this.ProcessSymbolCollection("^S.G.I", true);
            ////this.ProcessSymbolCollection("^S.G.U[CU]", true);
            ////this.ProcessSymbolCollection("^S.G.U[S]", true);
            ////this.ProcessSymbolCollection("^S.S", true);
            ////this.ProcessSymbolCollection("^S.U", true);
            ////this.ProcessSymbolCollection("^S.F", true);
            ////this.ProcessSymbolCollection("^G", true);                // this will not work yet
            
            ////this.ProcessSymbolCollection("^W.........P", true);        
            
            ////this.ProcessSymbolCollection("^I", true);
            ////this.ProcessSymbolCollection("^O", true);
            
            this.ProcessSymbolCollection("^E", true);

            this.SetFrame();

            ////this.ProcessSymbolCollection("^S.G.E", false);
            ////this.ProcessSymbolCollection("^S.S", false);
            ////this.ProcessSymbolCollection("^S.U", false);
            
            this.UnsetFrame();

#if DO_WRITES
            this.cxmlStream.WriteLine("</Items>");
            this.cxmlStream.WriteLine("</Collection>");
            this.cxmlStream.Flush();
            this.cxmlStream.Close();
            this.cxmlStream.Dispose();
#endif
        }

        /// <summary>
        /// Process the given appendix.
        /// </summary>
        /// <param name="append">
        /// The name of the appendix.
        /// </param>
        /// <param name="framed">
        /// Whether or not the symbols are framed.
        /// </param>
        private void ProcessSymbolCollection(string append, bool framed)
        {
            const double Scale = 1.0;

            // Since some appendices cross reference symbols,
            // in other appendices, we'll check to make sure 
            // we haven't already drawn a particular symbol code.
            IList<string> symList = new List<string>();

            var affiliations = new[] { "U", "F", "N", "H" };
            var keys = MilAppendix.Keys(append);
            foreach (var ap in keys)
            {
                MilSymbol ms;
                var sc = ap;
                if (symList.Contains(sc))
                {
                    continue;
                }

                symList.Add(sc);

                // There is no affiliation for weather
                var schemeKey = CodingScheme.GetCode(sc);
                if (schemeKey == CodingScheme.Weather)
                {
                    ms = new MilSymbol(sc, Scale, "X=67.8");
                    this.ProcessSymbol(ms, sc);
                    continue;
                }

                if (schemeKey == CodingScheme.TacticalGraphics)
                {
                    sc = sc.Substring(0, 1) + "H" + sc.Substring(2, 1) + "A" + sc.Substring(4);
                    ms = new MilSymbol(sc, Scale, "H=HH;H1=H1;W=W;W1=W1;T=TT;N=N;X=XX;V=VV;C=CC;Y=YY;Q=-60.0");
                    this.ProcessSymbol(ms, sc);
                    continue;
                }

                if (!framed)
                {
                    sc = sc.Substring(0, 2) + "X" + sc.Substring(3);
                }

                foreach (var c in affiliations)
                {
                    sc = sc[0] + c + sc[2] + "P" + sc.Substring(4, 10) + "E";

                    ms = new MilSymbol(sc, Scale, null, null);
                    this.ProcessSymbol(ms, sc);
                }
            }
        }

        /// <summary>
        /// Generates the CXML for the symbol.
        /// </summary>
        /// <param name="ms">
        /// The symbol for which to generate the CXML.
        /// </param>
        /// <param name="name">
        /// The name to associate with the symbol.
        /// </param>
        private void ProcessSymbol(MilSymbol ms, string name)
        {
            if (ms.Empty)
            {
                return;
            }

            var rootName = name.Substring(0, 10);
            this.WriteCxml(ms, rootName);
            this.WriteImage(ms, rootName);
        }

        /// <summary>
        /// Writes out the CXML file.
        /// </summary>
        /// <param name="ms">
        /// The symbol for which to generate the CXML.
        /// </param>
        /// <param name="rootName">
        /// The name to use for the symbol.
        /// </param>
        private void WriteCxml(MilSymbol ms, string rootName)
        {
            var symbolCode = (CodingScheme.GetCode(ms.SymbolCode) != CodingScheme.Weather) ? rootName + "*****" : ms.SymbolCode;

            var description = MilAppendix.Description(symbolCode);
            var lines = description.Split(new[] { '\n' });
            var lineCount = lines.Length - 1; // the last line is empty

            var sb = new StringBuilder();
            sb.AppendFormat(@"<Item Id=""{0}"" Name=""{1}"" Img=""Symbols\{1}.png"" Href="""">", this.index++, rootName);
            sb.AppendLine();
            if (lineCount > 0)
            {
                sb.AppendFormat(@" <Description>""{0}""</Description>", lines[lineCount - 1].Trim(new[] { ' ', '\n', '\r' }));
                sb.AppendLine();
            }
            else
            {
                sb.AppendFormat(@" <Description>""""</Description>");
                sb.AppendLine();
            }

            sb.AppendLine(@" <Facets>");
            sb.AppendLine(@"  <Facet Name=""Affiliation"">");
            sb.AppendFormat(@"   <String Value=""{0}"" />", StandardIdentity.GetName(symbolCode));
            sb.AppendLine();
            sb.AppendLine(@"  </Facet>");
            sb.AppendLine(@"  <Facet Name=""Battle Dimension"">");
            sb.AppendFormat(@"   <String Value=""{0}"" />", CategoryBattleDimension.GetName(symbolCode));
            sb.AppendLine();
            sb.AppendLine(@"  </Facet>");
            if (lineCount > 2)
            {
                sb.AppendLine(@"  <Facet Name=""Type"">");
                sb.AppendFormat(@"   <String Value=""{0}"" />", lines[2].Trim(new[] { ' ', '\n', '\r' }));
                sb.AppendLine();
                sb.AppendLine(@"  </Facet>");
            }

            sb.AppendLine(@"  <Facet Name=""Coding Scheme"">");
            sb.AppendFormat(@"   <String Value=""{0}"" />", CodingScheme.GetName(symbolCode));
            sb.AppendLine();
            sb.AppendLine(@"  </Facet>");
            if (lineCount - 1 > 3)
            {
                sb.AppendLine(@"  <Facet Name=""Key Phrases"">");
                for (var i = 3; i < lineCount - 1; i++)
                {
                    sb.AppendFormat(@"   <String Value=""{0}"" />", lines[i].Trim(new[] { ' ', '\n', '\r' }));
                    sb.AppendLine();
                }

                sb.AppendLine(@"  </Facet>");
            }

            sb.AppendLine(@" </Facets>");
            sb.AppendLine(@"</Item>");

#if DO_WRITES
            this.cxmlStream.Write(sb.ToString());
#endif
        }

        /// <summary>
        /// Write out the image associated with the symbol.
        /// </summary>
        /// <param name="ms">
        /// The symbol for which to write out the image.
        /// </param>
        /// <param name="rootName">
        /// The name associated with the symbol.
        /// </param>
        private void WriteImage(MilSymbol ms, string rootName)
        {
            const double Scale = 0.333333;

            var ct = new CompositeTransform();
            ct.ScaleX = ct.ScaleY = Scale;
            ct.TranslateX = -ms.Bounds.Left * Scale;
            ct.TranslateY = -ms.Bounds.Top * Scale;
            ms.RenderTransform = ct;
            target.Width = Scale * ms.Bounds.Width;
            target.Height = Scale * ms.Bounds.Height;
            var path = new System.Windows.Shapes.Path
            {
                Fill = new SolidColorBrush(Colors.White),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 3.0,
                Data = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, target.Width - 1, target.Height - 1)
                }
            };

            target.Children.Add(path);
            target.Children.Add(ms);
#if DO_WRITES
            this.SaveImage(rootName);
#endif
            target.Children.Clear();
        }

        /// <summary>
        /// Code that actually saves the image.
        /// </summary>
        /// <param name="rootName">
        /// The name to associate with the image.
        /// </param>
        private void SaveImage(string rootName)
        {
            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), @"Symbols\" + rootName + ".png");

            // Check to see if the file exists and delete if it does
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            target.ToImage().WriteToStream(File.Create(filePath));
        }
    }
}
