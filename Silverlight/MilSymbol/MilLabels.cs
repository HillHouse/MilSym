// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilLabels.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Supports the generation of all labels for the standard military symbol.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilSymbol
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif

    using MilSym.LoadResources;
    using Schemas;

    /// <summary>
    /// Supports the generation of all labels for the standard military symbol.
    /// </summary>
    public class MilLabels
    {
        /// <summary>
        /// An enumerated value to indicate that the labels above the symbol are under consideration.
        /// </summary>
        internal const int TopLabels = 0;

        /// <summary>
        /// An enumerated value to indicate that the big labels northeast of the symbol are under consideration.
        /// </summary>
        internal const int BigLabels = 1;

        /// <summary>
        /// An enumerated value to indicate that the labels to the left of the symbol are under consideration.
        /// </summary>
        internal const int LeftLabels = 2;

        /// <summary>
        /// An enumerated value to indicate that the labels to the right of the symbol are under consideration.
        /// </summary>
        internal const int RightLabels = 3;

        /// <summary>
        /// An enumerated value to indicate that tactical graphics labels are under consideration.
        /// </summary>
        internal const int TacticalGraphicsLabels = 4;

        /// <summary>
        /// The number of label styles affected by changes to the symbol's label style.
        /// </summary>
        private const int LabelCount = TacticalGraphicsLabels + 1;

        /// <summary>
        /// The header for parsing some XAML code strings.
        /// </summary>
        private const string XmlnsOpen =
            "<Run xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
                " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'";

        /// <summary>
        /// The amount to offset the direction arrow when it originates from the center.
        /// </summary>
        private const double PlusMinus = 150.0; // amount to offset direction arrow

        /// <summary>
        /// Separator between strings on the same line. The standard says this should be a space.
        /// </summary>
        private const string Divider = " ";

        /// <summary>
        /// The culture to use for string operations 
        /// </summary>
        private static readonly CultureInfo Culture = new CultureInfo("en-US");

        /// <summary>
        /// The message logger
        /// </summary>
        private static readonly ILogger Log = LoggerFactory<MilLabels>.GetLogger();

        ////    C    quantity
        ////    F    +=>reinforced, -=>reduced, ±=>reinforced & reduced
        ////    G    staff comments
        ////    H    additional informmation
        ////    J    evaluation (reliability plus credibility) 
        ////         A-completely reliable           B-usually reliable  C-fairly reliable
        ////         D-not usually reliable          E-unreliable        F-reliability cannot be judged
        ////         1-confirmed by other sources    2-probably true     3-possibly true
        ////         4-doubtfully true               5-improbable        6-truth cannot be judged
        ////    K    combat effectiveness
        ////    L    signature equipment (! indicates detectable electronic signatures)
        ////    M    higher formation (number/title of higher commannd, Roman numerals designate corps
        ////    N    enemy (ENY) for equipment
        ////    P    IFF/SIF indentification modes and codes
        ////    Q    degrees measured clockwise from true north 
        ////    R2   SIGINT mobility (M=>mobile, S=>static, U=>uncertain)
        ////    T    unique designation (acquisition nummber when used in SIGINT)
        ////    V    equipment type
        ////    W    date/time as DDHHMMSSZMONYYYY or O/O
        ////    X    altitude or depth
        ////    Y    location (DMS, UTM, or MGRS)
        ////    Z    speed
        ////    AA   name of special C2 headquarters
        ////    AD   platform type (ELNOT or CENOT)
        ////    AE   equipment teardown time
        ////    AF   common identifier (e.g., Hawk for Hawk SAM)
        ////    AG   auxillary equipment indicator
        ////    AM   distance
        ////    AN    azimuth
        ////    AO    engagement bar

        /// <summary>
        /// The legal labels
        /// </summary>
        private static readonly List<string> LegalLabels = new List<string>
        {
            "C", "F", "G", "H", "H1", "J", "K", "L", "M", "N", "P", "Q", "T", "T1", "V", "W", "W1", "X", "X1", "Y", "Z", "AA"
        };

        /// <summary>
        /// A placeholder key for the default collection of label styles
        /// </summary>
        private static readonly Style DefaultStyle = new Style();

        /// <summary>
        /// Cached list of styles - currently this is not ever pruned and could be abused
        /// </summary>
        private static readonly IDictionary<Style, Style[]> LabelStyles = new Dictionary<Style, Style[]>();

        /// <summary>
        /// A dictionary just for the "W" label
        /// </summary>
        private static readonly IDictionary<string, string> DashLabels = new Dictionary<string, string> { { "W", string.Empty } };

        /// <summary>
        /// Initializes static members of the <see cref="MilLabels"/> class.
        /// </summary>
        static MilLabels()
        {
            var styles = new Style[LabelCount];
            styles[TopLabels] = SymbolData.GetStyle("TopLabels");
            styles[BigLabels] = SymbolData.GetStyle("BigLabels");
            styles[LeftLabels] = SymbolData.GetStyle("LeftLabels");
            styles[RightLabels] = SymbolData.GetStyle("RightLabels");
            styles[TacticalGraphicsLabels] = SymbolData.GetStyle("TacticalGraphicsLabels");
            LabelStyles.Add(DefaultStyle, styles);
        }

        /// <summary>
        /// A delegate for adding a string to a child element.
        /// </summary>
        /// <param name="name">
        /// The string to be added to the child element.
        /// </param>
        /// <param name="ue">
        /// The child element to contain the string.
        /// </param>
        public delegate void AddChild(string name, UIElement ue);

        /// <summary>
        /// Parses the LabelString as dictionary of labels
        /// </summary>
        /// <param name="labelsIn">
        /// The label string coming in.
        /// </param>
        /// <param name="labelsOut">
        /// The dictionary of labels with the name of the label as the key.
        /// </param>
        /// <returns>
        /// Also returns the dictionary of labels.
        /// </returns>
        public static IDictionary<string, string> Generate(
            string labelsIn, IDictionary<string, string> labelsOut)
        {
            // There are a couple of parsing capabilities:
            // A=SOMETHING,B=SOMETHING - i.e., two different, but arbitrary, separators
            // A:SOMETHING:B:SOMETHING - i.e., one, arbitrary, separator

            // Look for the first non-alphanumeric character - that will be a separator
            int i;
            int len = labelsIn.Length;
            for (i = 0; i < len; i++)
            {
                if (!char.IsLetterOrDigit(labelsIn[i]))
                {
                    break;
                }
            }

            if (i == len)
            {
                return labelsOut;
            }

            // We got a separator so split the string up
            string[] bits = labelsIn.Split(labelsIn[i]);

            // Verify that all of the label names are valid
            if (!CheckValidity(bits)) 
            {
                int last = bits[1].Length - 1;  // we have a second separator
                for (int j = last; j > 0; j--)
                {
                    if (!char.IsLetterOrDigit(bits[1][j]))
                    {
                        bits = labelsIn.Split(new[] { labelsIn[i], bits[1][j] });
                        if (!CheckValidity(bits))
                        {
                            return labelsOut;
                        }

                        break;
                    }
                }
            }

            int count = bits.Length;

            // Check for an even number of entries
            if ((count & 1) != 0)
            {
                return labelsOut;
            }

            // Only create labels array if there are labels
            if (labelsOut == null)
            {
                labelsOut = new Dictionary<string, string>(count / 2);
            }

            // Process the pairs
            for (int j = 0; j < count; j += 2)
            {
                if (!labelsOut.ContainsKey(bits[j]))
                {
                    labelsOut.Add(bits[j], bits[j + 1]);
                }
                else
                {
                    labelsOut[bits[j]] = bits[j + 1];
                }
            }

            return labelsOut;
        }

        /// <summary>
        /// Return the cache of styles for the particular symbol.
        /// </summary>
        /// <param name="style">
        /// The style for which to retrieve the array of styles.
        /// </param>
        /// <returns>
        /// An array of styles, such as left labels, right labels, etc. that match the passed in style.
        /// </returns>
        internal static Style[] GetStyles(Style style)
        {
            if (style == null)
            {
                return LabelStyles[DefaultStyle];
            }

            if (LabelStyles.ContainsKey(style))
            {
                return LabelStyles[style];
            }

            var styles = new Style[LabelCount];
            styles[TopLabels] = new Style(typeof(TextBlock)) { BasedOn = SymbolData.GetStyle("TopLabels") };
            styles[BigLabels] = new Style(typeof(TextBlock)) { BasedOn = SymbolData.GetStyle("BigLabels") };
            styles[LeftLabels] = new Style(typeof(TextBlock)) { BasedOn = SymbolData.GetStyle("LeftLabels") };
            styles[RightLabels] = new Style(typeof(TextBlock)) { BasedOn = SymbolData.GetStyle("RightLabels") };
            styles[TacticalGraphicsLabels] = new Style(typeof(TextBlock)) { BasedOn = SymbolData.GetStyle("TacticalGraphicsLabels") };

            var tb = new TextBlock { Style = style };
            SetSingleProperty(TextBlock.ForegroundProperty, tb.Foreground, ref styles);
            SetSingleProperty(TextBlock.FontSizeProperty, tb.FontSize, ref styles);
            SetSingleProperty(TextBlock.FontFamilyProperty, tb.FontFamily, ref styles);
            SetSingleProperty(TextBlock.FontWeightProperty, tb.FontWeight, ref styles);
            LabelStyles.Add(style, styles);
            return styles;
        }

        /// <summary>
        /// Returns the arrow representing a direction of travel for a mil symbol
        /// </summary>
        /// <param name="symbolCode">Code for the mil symbol</param>
        /// <param name="labels">The labels for the symbol, we're looking for "Q"</param>
        /// <param name="extraOffset">An extra offset for tactical graphics</param>
        /// <returns>The arrow shape representing the direction of travel</returns>
        internal static Shape GenerateQ(
            string symbolCode,
            IDictionary<string, string> labels,
            double extraOffset)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            string q;
            if (!labels.TryGetValue("Q", out q))
            {
                return null;
            }

            double angle;
            if (!double.TryParse(q, out angle))
            {
                return null;
            }

            double off = 0.0;
            if (CategoryBattleDimension.GetCode(symbolCode) == CategoryBattleDimension.BdGround)
            {
                off = SymbolData.GetBounds(symbolCode).Bottom;
            }

            off += extraOffset;
            return new Path
            {
                Style = SymbolData.GetStyle("BS10"),
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        GenerateArrowPoints(off, angle)
                    }
                }
            };
        }

        /// <summary>
        /// Generate the Joker, Faker, or "eXercise" character.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for which to generate the large character string.
        /// </param>
        /// <param name="labels">
        /// A dictionary of the labels to be drawn.
        /// </param>
        /// <param name="style">
        /// The style to apply to the labels.
        /// </param>
        /// <returns>
        /// The TextBlock that incorporates the labels.
        /// </returns>
        internal static TextBlock GenerateJokerFakerExercise(string symbolCode, IDictionary<string, string> labels, Style style)
        {
            // Check for matching code
            char theChar = StandardIdentity.GetExerciseAmplifyingDescriptor(symbolCode);
            if (theChar == (char)0)
            {
                return null;
            }
#if WINDOWS_UWP
            string theString = theChar.ToString();
#else
            string theString = theChar.ToString(Culture);
#endif
            if (labels != null && labels.ContainsKey("F"))
            {
                theString += " " + labels["F"];
            }

            Rect r = SymbolData.GetBounds(symbolCode);
            var tb = new TextBlock
            {
                Style = style, // BigLabels,
                Text = theString
            };
            tb.FindTextExtent();
            tb.SetValue(Canvas.TopProperty, r.Top - tb.Height);
            tb.SetValue(Canvas.LeftProperty, r.Right);
            return tb;
        }

        /// <summary>
        /// Generate the left labels for the symbol.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for which to generate the labels.
        /// </param>
        /// <param name="labels">
        /// The dictionary of labels to generate.
        /// </param>
        /// <param name="style">
        /// The style with which to render the labels.
        /// </param>
        /// <returns>
        /// A TextBlock that represents the rendered labels.
        /// </returns>
        internal static TextBlock GenerateLeft(string symbolCode, IDictionary<string, string> labels, Style style)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            // Pick off the left-oriented strings and write them out
            if (labels == null)
            {
                return null;
            }

            var left = new TextBlock { Style = style /*LeftLabels*/ };

            bool gotLine = false;
            ProcessLabels(labels, false, new[] { "W" }, left, ref gotLine);
            var height = gotLine ? (double)left.GetValue(TextBlock.LineHeightProperty) : 0.0;

            // At this point we need to process the remaining labels as if each row has a label.
            // Otherwise some label lines may appear in the wrong position 
            // if other label lines are empty.
            // Setting "gotLine" to true forces a new line for each of the label lines.
            // More complicated logic might consider the height of the text block, prior to
            // the elimination of the empty lines.
            gotLine = true;

            ProcessLabels(labels, false, new[] { "X", "Y" }, left, ref gotLine);
            ProcessLabels(labels, false, new[] { "V" }, left, ref gotLine);
            ProcessLabels(labels, false, new[] { "T" }, left, ref gotLine);
            ProcessLabels(labels, true, new[] { "Z" }, left, ref gotLine);
            TruncateNewLines(left);

            Rect b = SymbolData.GetBounds(symbolCode);
            left.FindTextExtent();
            SetTopLeft(left, b.Left - left.Width - 10, b.Top - height);
            return left; // we're only doing this to support unit test
        }

        /// <summary>
        /// Generate the right labels for the symbol.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for which to generate the labels.
        /// </param>
        /// <param name="labels">
        /// The dictionary of labels to generate.
        /// </param>
        /// <param name="style">
        /// The style with which to render the labels.
        /// </param>
        /// <returns>
        /// A TextBlock that represents the rendered labels.
        /// </returns>
        internal static TextBlock GenerateRight(string symbolCode, IDictionary<string, string> labels, Style style)
        {
            // Pick off the left-oriented strings and write them out
            if (labels == null)
            {
                return null;
            }

            var right = new TextBlock { Style = style /*RightLabels*/ };
            bool gotLine = false;

            // If we have an X, J, or K modifier character, skip label F because it is already written out.
            if (StandardIdentity.GetExerciseAmplifyingDescriptor(symbolCode) != (char)0)
            {
                ProcessLabels(labels, false, new[] { string.Empty }, right, ref gotLine);
            }
            else
            {
                ProcessLabels(labels, false, new[] { "F" }, right, ref gotLine);
            }

            var height = gotLine ? (double)right.GetValue(TextBlock.LineHeightProperty) : 0.0;

            // At this point we need to process the remaining labels as if each row has a label.
            // Otherwise some label lines may appear in the wrong position 
            // if other label lines are empty.
            // Setting "gotLine" to true forces a new line for each of the label lines.
            // More complicated logic might consider the height of the text block, prior to
            // the elimination of the empty lines.
            gotLine = true;

            ProcessLabels(labels, false, new[] { "G" }, right, ref gotLine);
            ProcessLabels(labels, false, new[] { "H" }, right, ref gotLine);
            ProcessLabels(labels, false, new[] { "M" }, right, ref gotLine);
            ProcessLabels(labels, true, new[] { "J", "K", "L", "N", "P" }, right, ref gotLine);
            TruncateNewLines(right);

            Rect b = SymbolData.GetBounds(symbolCode);
            SetTopLeft(right, b.Right + (height / 5.0), b.Top - height);
            return right;
        }

        /// <summary>
        /// Generate the top labels for the symbol.
        /// </summary>
        /// <param name="high">
        /// The height at which to generate the labels.
        /// </param>
        /// <param name="symbolCode">
        /// The symbol code for which to generate the labels.
        /// </param>
        /// <param name="labels">
        /// The dictionary of labels to generate.
        /// </param>
        /// <param name="style">
        /// The style with which to render the labels.
        /// </param>
        /// <returns>
        /// A TextBlock that represents the rendered labels.
        /// </returns>
        internal static TextBlock GenerateTop(double high, string symbolCode, IDictionary<string, string> labels, Style style)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            if (labels == null)
            {
                return null;
            }

            var middle = new TextBlock { Style = style /*TopLabels*/ };

            bool gotLine = false;
            ProcessLabels(labels, true, new[] { "C" }, middle, ref gotLine);
            var height = gotLine ? (double)middle.GetValue(TextBlock.LineHeightProperty) : 0.0;

            middle.FindTextExtent();
            
            // This positions the top line just above the symbol, I think
            SetTopLeft(middle, -middle.Width / 2.0, high - height);
            return middle;
        }

        /// <summary>
        /// Generate the middle label for the symbol.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for which to generate the labels.
        /// </param>
        /// <param name="labels">
        /// The dictionary of labels to generate.
        /// </param>
        /// <returns>
        /// A TextBlock that represents the rendered labels.
        /// </returns>
        internal static TextBlock GenerateMiddle(string symbolCode, IDictionary<string, string> labels)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            if (labels == null)
            {
                return null;
            }

            var middle = new TextBlock { Style = SymbolData.GetStyle("MiddleLabels") };

            bool gotLine = false;
            ProcessLabels(labels, true, new[] { "AA" }, middle, ref gotLine);
            middle.FindTextExtent();
            SetTopLeft(middle, -middle.Width / 2, -middle.Height / 2);
            return middle;
        }

        /// <summary>
        /// Generate a TextBlock label based on an integer value.
        /// </summary>
        /// <param name="x">
        /// The x coordinate for the TextBlock.
        /// </param>
        /// <param name="y">
        /// The y coordinate for the TextBlock.
        /// </param>
        /// <param name="integerLabel">
        /// The integer label to be displayed.
        /// </param>
        /// <param name="style">
        /// The style for the label.
        /// </param>
        /// <returns>
        /// The TextBlock containing the integer as a label.
        /// </returns>
        internal static TextBlock IntegerLabel(
            double x,                // a reference point for the horizontal location
            double y,                // the top of the label - for now
            string integerLabel,    // the label
            string style)            // the label's style
        {
            var label = new TextBlock
            {
                Style = SymbolData.GetStyle(style)
            };

            double result;
            if (!double.TryParse(integerLabel, out result))
            {
                return null;
            }

            if (result < 0.0 || result > 99999.0)    
            {
                return null; // negative or too many digits
            }

            integerLabel = Math.Round(result).ToString(Culture);
            label.Text = integerLabel;
            label.FindTextExtent();
            var multiplier =
                (label.TextAlignment == TextAlignment.Center) ? 0.5 :
                (label.TextAlignment == TextAlignment.Right) ? 1.0 : 0.0;
            SetTopLeft(label, x - (multiplier * label.Width), y);
            return label;
        }

        /// <summary>
        /// Generate the labels for the single point weather symbology.
        /// </summary>
        /// <param name="symbolCode">
        /// The weather symbol code.
        /// </param>
        /// <param name="labels">
        /// The dictionary of labels to be displayed with the weather symbology.
        /// </param>
        /// <param name="addChild">
        /// The delegate that will actually add the child to the UIElement.
        /// </param>
        internal static void GenerateWeather(string symbolCode, IDictionary<string, string> labels, AddChild addChild)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return;
            }

            if (labels == null)
            {
                return;
            }

            // There are very few weather codes that require labels - so we'll handle them via a case statement
            string label;
            switch (symbolCode)
            {
                case "WOS-HDS---P----":            // X - altitude
                {
                    double depth = 0.0;
                    var ok = labels.ContainsKey("X") && double.TryParse(labels["X"], out depth);
                    var splits = (ok ? depth.ToString(Culture) : "6.3").Split('.');
                    var meters = IntegerLabel(0, -100, splits[0], "MeterDepthLabel");
                    addChild("Meters", meters);
                    
                    // Plot the first fractional digit offset 
                    if (splits.Length > 1)
                    {
                        meters.FindTextExtent();
                        addChild(
                            "Decimeters",
                            IntegerLabel(meters.Width / 2, -25, splits[1].Substring(0, 1), "DecimeterDepthLabel"));
                    }

                    return;
                }

                case "WAS-WSF-LVP----":            // X - altitude
                {
                    label = labels.ContainsKey("X") ? labels["X"] : "100";
                    addChild("Altitude", IntegerLabel(63, -55, label, "FreezeLabel"));
                    return;
                }

                case "WAS-WST-LVP----":            // X - altitude
                {
                    label = labels.ContainsKey("X") ? labels["X"] : "380";
                    addChild("Altitude", IntegerLabel(0, -61, label, "TropoLabel"));
                    return;
                }

                case "WAS-PLT---P----":            // low pressure
                {
                    label = labels.ContainsKey("X") ? labels["X"] : "270";
                    addChild("Altitude", IntegerLabel(0, -136, label, "PressureLabel"));
                    return;
                }

                case "WAS-PHT---P----":            // high pressure
                {
                    label = labels.ContainsKey("X") ? labels["X"] : "460";
                    addChild("Altitude", IntegerLabel(0, 2, label, "PressureLabel"));
                    return;
                }

                case "WAS-WC----P----":
                {
                    int cloudCover; // 0 through 9}
                    if (labels.ContainsKey("AA") &&
                        int.TryParse(labels["AA"], out cloudCover))
                    {
                        addChild("CloudCover", WindBarb.GenerateCloudCover(2, cloudCover));
                    }

                    return;
                }

                case "WAS-WP----P----":        // wind barb
                {
                    int cloudCover; // 0 through 9}
                    if (labels.ContainsKey("AA") &&
                        int.TryParse(labels["AA"], out cloudCover))
                    {
                        addChild("CloudCover", WindBarb.GenerateCloudCover(1, cloudCover));
                    }

                    double speed;        // in knots
                    double? speedIn = null;
                    if (labels.ContainsKey("Z") &&
                        double.TryParse(labels["Z"], out speed))
                    {
                        speedIn = speed;
                    }

                    bool southernHemisphere = labels.ContainsKey("Y") && labels["Y"].ToUpper() == "S";

                    double direction;    // in degrees
                    if (labels.ContainsKey("Q") &&
                        double.TryParse(labels["Q"], out direction))
                    {
                        addChild("Wind", WindBarb.GenerateWind(speedIn, direction, southernHemisphere));
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Adds a label to a UIElement at a given offset based on the text width and height.
        /// </summary>
        /// <param name="labels">
        /// The dictionary of labels containing the label to render.
        /// </param>
        /// <param name="match">
        /// The key to the label to render.
        /// </param>
        /// <param name="locationX">
        /// The x coordinate of where to place the label.
        /// </param>
        /// <param name="locationY">
        /// The y coordinate of where to place the label.
        /// </param>
        /// <param name="multiplierX">
        /// The x multiplier to apply to the label's width to further offset the label.
        /// </param>
        /// <param name="multiplierY">
        /// The y multiplier to apply to the label's height to further offset the label.
        /// </param>
        /// <param name="style">
        /// The style with which to render the label.
        /// </param>
        /// <param name="addChild">
        /// The delegate that will actually render the label into the UIElement.
        /// </param>
        /// <returns>
        /// The TextBlock containing the rendered label.
        /// </returns>
        internal static TextBlock GenerateSingleLabel(
            IDictionary<string, string> labels,
            string match,
            double locationX,
            double locationY,
            double multiplierX,
            double multiplierY,
            Style style,
            AddChild addChild)
        {
            if (labels == null)
            {
                return null;
            }

            if (!labels.ContainsKey(match))
            {
                return null;
            }

            var tb = new TextBlock
            {
                Style = style, /*TacticalGraphicsLabels*/
                Text = labels[match]
            };
            tb.FindTextExtent();
            SetTopLeft(tb, locationX - (multiplierX * tb.Width), locationY - (multiplierY * tb.Height));
            addChild(match, tb);
            return tb;
        }

        /// <summary>
        /// Helper method to set the x and y coordinates for a TextBlock
        /// </summary>
        /// <param name="tb">
        /// The TextBlock on which to set the x and y coordinates.
        /// </param>
        /// <param name="x">
        /// The x coordinate.
        /// </param>
        /// <param name="y">
        /// The y coordinate.
        /// </param>
        internal static void SetTopLeft(TextBlock tb, double x, double y)
        {
            tb.FindTextExtent();
            tb.SetValue(Canvas.TopProperty, y);
            tb.SetValue(Canvas.LeftProperty, x);
        }

        /// <summary>
        /// Generate the labels for single point technical graphics.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for the technical graphic.
        /// </param>
        /// <param name="labels">
        /// The dictionary of labels for the technical graphic.
        /// </param>
        /// <param name="style">
        /// The style for the labels.
        /// </param>
        /// <param name="addChild">
        /// The delegate that actually adds the label to the UIElement.
        /// </param>
        internal static void GenerateTacticalGraphics(
            string symbolCode, IDictionary<string, string> labels, Style style, AddChild addChild)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return;
            }

            if (labels == null)
            {
                return;
            }

            string stencil = MilSymbolBase.CodeToStencil(symbolCode);

            double extraOffset;
            TextBlock tb;

            // These are special case codes that require labels
            switch (stencil)
            {
                case "G_MPNDP":
                case "G_MPNDA":
                case "G_MPNDT":
                case "G_MPNDE":
                case "G_MPNDB":
                case "G_MPNDO":
                case "G_MPNDD":
                case "G_FPPCS":
                case "G_FPPCB":
                case "G_FPPCR":
                case "G_FPPCH":
                case "G_FPPCL":
                    TacticalGraphicsPointLabels(labels, addChild, new[] { "H", "N", "T", "W" }, style);
                    break;
                case "G_GPGPP":
                    TacticalGraphicsPointLabels(labels, addChild, new[] { "H", "H1", "N", "T", "W-" }, style);
                    break;
                case "G_GPGPPK":
                case "G_GPGPPL":
                case "G_GPGPPP":
                case "G_GPGPPR":
                case "G_GPGPPE":
                case "G_GPGPPS":
                case "G_GPGPPA":
                case "G_GPOPP":
                case "G_MPBCP":
                case "G_SPPX":
                case "G_SPPC":
                case "G_SPPY":
                case "G_SPPT":
                case "G_SPPD":
                case "G_SPPE":
                case "G_SPPL":
                case "G_SPPM":
                case "G_SPPR":
                case "G_SPPU":
                case "G_SPPO":
                case "G_SPPI":
                case "G_SPPN":
                case "G_SPPSZ":
                case "G_SPPSA":
                case "G_SPPSB":
                case "G_SPPSC":
                case "G_SPPSD":
                case "G_SPPSE":
                case "G_SPPSF":
                case "G_SPPSG":
                case "G_SPPSH":
                case "G_SPPSI":
                case "G_SPPSJ":
                case "G_SPPAS":
                case "G_SPPAT":
                    TacticalGraphicsPointLabels(labels, addChild, new[] { "H", "N", "T", "W-" }, style);
                    break;
                case "G_GPGPRI":
                    GenerateSingleLabel(labels, "T", 0, -246, 0.5, 0.5, style, addChild);
                    break;
                case "G_GPGPH":
                    GenerateSingleLabel(labels, "H", 0, 0, 0.5, 0.5, style, addChild);
                    break;
                case "G_GPGPPC":
                    GenerateSingleLabel(labels, "T", 0, -175, 0.5, 0.5, style, addChild);
                    break;
                case "G_GPGPPD":
                    GenerateSingleLabel(labels, "T", 0, 0, 0.5, 0.25, style, addChild);
                    break;
                case "G_GPGPPW":
                    GenerateSingleLabel(labels, "T", 50, 0, 0.0, 0.5, style, addChild);
                    break;
                case "G_GPAPP":
                case "G_GPAPC":
                    GenerateSingleLabel(labels, "T", 0, 0, 0.5, 0.25, style, addChild);
                    break;
                case "G_GPDPT":
                    GenerateSingleLabel(labels, "T", 87, -87, 0.5, 0.5, style, addChild);
                    break;
                case "G_MPOHTL":
                    GenerateSingleLabel(labels, "X", 50, -250, 0.0, 0.0, style, addChild);
                    break;
                case "G_MPOHTH":
                    GenerateSingleLabel(labels, "X", 50, -230, 0.0, 0.0, style, addChild);
                    break;
                case "G_MPNZ":
                    GenerateSingleLabel(labels, "W", -110, -300, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "V", -110, -200, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "T", -110, -100, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "H", 110, -300, 0.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "N", 110, -100, 0.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "C", 0, -300, 0.5, 1.0, style, addChild);
                    tb = GenerateSingleLabel(labels, "Y", 0, 0, 0.5, 0.0, style, addChild);
                    tb.FindTextExtent();
                    extraOffset = tb != null ? tb.Height : 0.1;
                    addChild("Q", GenerateQ(symbolCode, labels, extraOffset));
                    break;
                case "G_MPNEB":
                    GenerateSingleLabel(labels, "W", -110, -300, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "T", -110, -100, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "H", 110, -300, 0.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "N", 110, -100, 0.0, 0.0, style, addChild);
                    tb = GenerateSingleLabel(labels, "Y", 0, 0, 0.5, 0.0, style, addChild);
                    tb.FindTextExtent();
                    extraOffset = tb != null ? tb.Height : 0.1;
                    addChild("Q", GenerateQ(symbolCode, labels, extraOffset));
                    break;
                case "G_MPNEC":
                    GenerateSingleLabel(labels, "W", -94, -300, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "T", -94, -100, 1.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "H", 94, -300, 0.0, 0.0, style, addChild);
                    GenerateSingleLabel(labels, "N", 94, -100, 0.0, 0.0, style, addChild);
                    tb = GenerateSingleLabel(labels, "Y", 0, 0, 0.5, 0.0, style, addChild);
                    tb.FindTextExtent();
                    extraOffset = tb != null ? tb.Height : 0.1;
                    addChild("Q", GenerateQ(symbolCode, labels, extraOffset));
                    break;
                case "G_FPPTS":
                    GenerateSingleLabel(labels, "H", 50, 75, 0.0, 0.5, style, addChild);
                    GenerateSingleLabel(labels, "H1", -50, 75, 1.0, 0.5, style, addChild);
                    GenerateSingleLabel(labels, "T", 50, -75, 0.0, 0.5, style, addChild);
                    break;
                case "G_FPPTN":
                    GenerateSingleLabel(labels, "T", 50, -75, 0.0, 0.5, style, addChild);
                    break;
                case "G_FPPCF":
                    GenerateSingleLabel(labels, "T", 50, 0, 0.0, 0.5, style, addChild);
                    break;
            }
        }

        /// <summary>
        /// Set one attribute value for the labels affected by the symbols label style.
        /// </summary>
        /// <param name="dp">
        /// The dependency property to set.
        /// </param>
        /// <param name="o">
        /// The value to set for the dependency property.
        /// </param>
        /// <param name="styles">
        /// The styles for which to set the dependency property.
        /// </param>
        private static void SetSingleProperty(DependencyProperty dp, object o, ref Style[] styles)
        {
            styles[LeftLabels].Setters.Add(new Setter(dp, o));
            styles[RightLabels].Setters.Add(new Setter(dp, o));
            styles[TopLabels].Setters.Add(new Setter(dp, o));
            styles[TacticalGraphicsLabels].Setters.Add(new Setter(dp, o));
            if (dp == TextBlock.ForegroundProperty)
            {
                styles[BigLabels].Setters.Add(new Setter(dp, o));
            }
        }

        /// <summary>
        /// Check to make sure that all label names are valid - as listed in MIL-STD 2525C.
        /// </summary>
        /// <param name="bits">
        /// The array of label names to check.
        /// </param>
        /// <returns>
        /// A boolean to indicate whether the passed in labels are valid.
        /// </returns>
        private static bool CheckValidity(string[] bits)
        {
            for (int i = 0; i < bits.Length; i += 2)
            {
                if (!LegalLabels.Contains(bits[i]))
                {
                    return false;
                }
            }

            return true;
        }

        ////     ^
        ////    / \                
        ////   /_|_\    
        ////     |
        ////     |
        //// First point is up inside middle of arrow    (0)
        //// Then comes bottom of arrow                  (1)
        //// Then comes right base of arrow              (2)
        //// Then comes tip of arrow                     (3)
        //// Then comes left base of arrow               (4)
        //// Then comes center of arrow                  (5)
        //// Then comes shaft of arrow                   (6)
        //// Then, optionally, comes line to join arrow to symbol

        /// <summary>
        /// Generates points for an arrow assuming a 10-pixel wide line (un-scaled value).
        /// </summary>
        /// <param name="off">The pixel distance down from the center of the symbol to the start of the arrow</param>
        /// <param name="angle">The arrow angle measured clockwise from true north</param>
        /// <returns>An array of points that make up the arrow</returns>
        private static PathFigure GenerateArrowPoints(double off, double angle)
        {
            // Convert angle to radians measured
            // counterclockwise from the x-axis
            angle = (90.0 - angle) * Math.PI / 180.0;
            double sa = -Math.Sin(angle); // use sign change to minimize sign changes below
            double ca = Math.Cos(angle);

            // Need shorter arrow if not from center
            double arrow = (Math.Abs(off) > double.Epsilon) ? 200 : 300;
            if (Math.Abs(off) > double.Epsilon)
            {
                off += PlusMinus; // additional offset to support non-center arrows
            }

            // Just some trig to avoid transformation matrices
            // Some arrows are from the symbol center (7 points), others from the base (8 points)
            double px = arrow * ca;
            double py = (arrow * sa) + off;

            var psc = new PathSegmentCollection
            {
                new LineSegment { Point = new Point(px, py) },
                new LineSegment { Point = new Point(px - (10 * sa), py + (10 * ca)) },
                new LineSegment { Point = new Point(px + (40 * ca), py + (40 * sa)) },
                new LineSegment { Point = new Point(px + (10 * sa), py - (10 * ca)) },
                new LineSegment { Point = new Point(px, py) },
                new LineSegment { Point = new Point(0, off) }
            };
            if (Math.Abs(off) > double.Epsilon)
            {
                psc.Add(new LineSegment { Point = new Point(0, off - PlusMinus) }); // point to join non-center arrows to symbol}
            }

            var pf = new PathFigure
            {
                StartPoint = new Point(px + (20 * ca), py + (20 * sa)),
                Segments = psc
            };
            return pf;
        }

        /// <summary>
        /// Process a set of labels into a TextBlock.
        /// </summary>
        /// <param name="labels">
        /// A dictionary of the labels to be processed.
        /// </param>
        /// <param name="skip">
        /// Whether or not to return after generating the first line.
        /// </param>
        /// <param name="choices">
        /// The choice of keys for labels are to be generated.
        /// </param>
        /// <param name="tb">
        /// The TextBlock to which the labels are to be added.
        /// </param>
        /// <param name="gotLine">
        /// Whether or not an entire line was generated, including a line break.
        /// </param>
        private static void ProcessLabels(
            IDictionary<string, string> labels,
            bool skip,
            IEnumerable<string> choices,
            TextBlock tb,
            ref bool gotLine)
        {
            // boolean flag to manage dividers after a "<Run"
            bool needDivider = false;

            string finalLabel = string.Empty;
            foreach (string choice in choices)
            {
                if (!labels.ContainsKey(choice))
                {
                    continue;
                }

                gotLine = true;

                string label = labels[choice];
                
                // What to do if the current label is its own "Run"?
                // 1. If we alrady have something make it into its own "Run".
                // 2. Add this new "Run".
                // 3. Reset the finalLabel.
                // 4. Continue.
                if (label.StartsWith("<Run"))
                {
                    if (!string.IsNullOrEmpty(finalLabel))
                    {
                        // Need separator before next run
                        finalLabel += Divider;
                        tb.Inlines.Add(new Run { Text = finalLabel });  // #1
                        finalLabel = string.Empty;  // #3
                    }

                    try
                    {
#if SILVERLIGHT
                        var run = XamlReader.Load(XmlnsOpen + label.Substring(4)) as Run;   // #2
#elif WINDOWS_UWP
                        var run = XamlReader.Load(XmlnsOpen + label.Substring(4)) as Run;   // #2
#else
                        var run = XamlReader.Parse(XmlnsOpen + label.Substring(4)) as Run;  // #2
#endif
                        if (run != null)
                        {
                            tb.Inlines.Add(run);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteMessage(LogLevel.Warn, "Error parsing 'Run' command in label", ex);
                    }

                    needDivider = true;
                    continue;   // #4
                }

                if (!string.IsNullOrEmpty(finalLabel) || needDivider)
                {
                    finalLabel += Divider;
                    needDivider = false;
                }

                finalLabel += label;
            }

            // If we have a string, add it to the output
            if (!string.IsNullOrEmpty(finalLabel))
            {
                tb.Inlines.Add(new Run { Text = finalLabel });
            }

            if (skip)
            {
                return; // don't need another line break
            }

            if (gotLine)
            {
                tb.Inlines.Add(new LineBreak());
            }
        }

        /// <summary>
        /// Clean the line breaks off the end of the InLines.
        /// </summary>
        /// <param name="tb">
        /// The TextBlock from which the line breaks should be removed.
        /// </param>
        private static void TruncateNewLines(TextBlock tb)
        {
            var count = tb.Inlines.Count;
            for (var i = count - 1; i > 0; i--)
            {
#if WINDOWS_UWP
                if (((InlineCollection)tb.Inlines)[i] is LineBreak)
                {
                    ((InlineCollection)tb.Inlines).RemoveAt(i);
                }
#else
                if (((IList)tb.Inlines)[i] is LineBreak)
                {
                    ((IList)tb.Inlines).RemoveAt(i);
                }
#endif
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Generate the labels for a tactical graphic point symbol.
        /// </summary>
        /// <param name="labels">
        /// The dictionary containing the labels to be generated for the point symbol.
        /// </param>
        /// <param name="addChild">
        /// The delegate to actually add the labels.
        /// </param>
        /// <param name="matches">
        /// The list of keys for the labels to be generated.
        /// </param>
        /// <param name="style">
        /// The style for generating the labels.
        /// </param>
        private static void TacticalGraphicsPointLabels(
            IDictionary<string, string> labels,
            AddChild addChild,
            IEnumerable<string> matches,
            Style style)
        {
            foreach (var match in matches)
            {
                TextBlock tb;
                switch (match)
                {
                    case "H1":
                        GenerateSingleLabel(labels, "H1", 0, -265, 0.5, 0.0, style, addChild);
                        break;
                    case "H":
                        GenerateSingleLabel(labels, "H", 0, -285, 0.5, 1.0, style, addChild);
                        break;
                    case "T":
                        GenerateSingleLabel(labels, "T", 80, -275, 0.0, 0.0, style, addChild);
                        break;
                    case "N":
                        GenerateSingleLabel(labels, "N", 80, -95, 0.0, 1.0, style, addChild);
                        break;
                    case "W-":
                        // Need '-' on end of label here
                        if (labels.ContainsKey("W"))
                        {
                            DashLabels["W"] = labels["W"] + "-";
                            tb = GenerateSingleLabel(DashLabels, "W", -80, -275, 1.0, 0.0, style, addChild);
                            if (tb != null)
                            {
                                tb.FindTextExtent();
                                GenerateSingleLabel(labels, "W1", -80, -275 + tb.Height, 1.0, 0.0, style, addChild);
                            }
                        }

                        break;
                    case "W":
                        tb = GenerateSingleLabel(labels, "W", -80, -275, 1.0, 0.0, style, addChild);
                        if (tb != null)
                        {
                            tb.FindTextExtent();
                            GenerateSingleLabel(labels, "W1", -80, -275 + tb.Height, 1.0, 0.0, style, addChild);
                        }

                        break;
                }
            }
        }
    }
}