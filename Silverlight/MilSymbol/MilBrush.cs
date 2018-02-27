// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilBrush.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Supports both standard and custom colored brushes, lines, fills, and styles.
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
#if WINDOWS_UWP
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif
    using Schemas;

    /// <summary>
    /// Enumeration of the MIL-STD 2525C standard color schemes
    /// </summary>
    public enum ColorSchemeProperty
    {
        /// <summary>
        /// Enumerated value representing the dark colors specified in MIL-STD 2525C
        /// </summary>
        Dark = 0,

        /// <summary>
        /// Enumerated value representing the medium dark colors specified in MIL-STD 2525C
        /// </summary>
        Medium = 1,

        /// <summary>
        /// Enumerated value representing the light colors specified in MIL-STD 2525C
        /// </summary>
        Light = 2
    }

    /// <summary>
    /// MilBrush maintains the colored brushes specified by the military standard.
    /// </summary>
    public static class MilBrush
    {
        /// <summary>
        /// A rust color used by some weather symbols
        /// </summary>
        public static readonly Brush Rust = new SolidColorBrush(Color.FromArgb(0xFF, 0xC6, 0x10, 0x21));

        /// <summary>
        /// A solid black line style. This is the same as the ResourceDictionary's BS10
        /// </summary>
        public static readonly Style BlackPresent = new Style(typeof(Shape));

        /// <summary>
        /// The offsets into the array of default brushes for yellows.
        /// </summary>
        private const int Yellows = 1;

        /// <summary>
        /// The offsets into the array of default brushes for purples.
        /// </summary>
        private const int Purples = 4;

        /// <summary>
        /// The offsets into the array of default brushes for greens.
        /// </summary>
        private const int Greens = 7;

        /// <summary>
        /// The offsets into the array of default brushes for reds.
        /// </summary>
        private const int Reds = 10;

        /// <summary>
        /// The offsets into the array of default brushes for blues.
        /// </summary>
        private const int Blues = 13;

        /// <summary>
        /// Array of all the colors, defined as brushes, as specified by MIL-STD 2525C
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed. Suppression is OK here.")]
        private static readonly SolidColorBrush[] Defaults = new[]
        {
            new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), // black

            // yellows
            new SolidColorBrush(Color.FromArgb(255, 225, 225, 0)),
            new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)),
            new SolidColorBrush(Color.FromArgb(255, 255, 255, 128)),
            
            // purples
            new SolidColorBrush(Color.FromArgb(255, 80, 0, 80)),
            new SolidColorBrush(Color.FromArgb(255, 128, 0, 128)),
            new SolidColorBrush(Color.FromArgb(255, 255, 161, 255)),
            
            // greens
            new SolidColorBrush(Color.FromArgb(255, 0, 160, 0)),
            new SolidColorBrush(Color.FromArgb(255, 0, 226, 0)),
            new SolidColorBrush(Color.FromArgb(255, 170, 255, 170)),
            
            // reds
            new SolidColorBrush(Color.FromArgb(255, 200, 0, 0)),
            new SolidColorBrush(Color.FromArgb(255, 255, 48, 49)),
            new SolidColorBrush(Color.FromArgb(255, 255, 128, 128)),
            
            // blues
            new SolidColorBrush(Color.FromArgb(255, 0, 107, 140)),
            new SolidColorBrush(Color.FromArgb(255, 0, 168, 220)),
            new SolidColorBrush(Color.FromArgb(255, 128, 224, 255))
        };

        /// <summary>
        /// A dictionary of solid line styles (present, not anticipated).
        /// </summary>
        private static readonly IDictionary<Brush, Style> LinePresents = new Dictionary<Brush, Style>();

        /// <summary>
        /// A dictionary of solid line-fill styles (present, not anticipated).
        /// </summary>
        private static readonly IDictionary<Brush, Style> LineFillPresents = new Dictionary<Brush, Style>();

        /// <summary>
        /// A dictionary of solid fill styles.
        /// </summary>
        private static readonly IDictionary<Brush, Style> Fills = new Dictionary<Brush, Style>();

        /// <summary>
        /// Initializes static members of the <see cref="MilBrush"/> class.
        /// </summary>
        static MilBrush()
        {
            ColorScheme = ColorSchemeProperty.Light;
            BlackPresent.Setters.Add(new Setter(Shape.StrokeProperty, Black));
            BlackPresent.Setters.Add(new Setter(Shape.StrokeMiterLimitProperty, 2.0));
            BlackPresent.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 10.0));
            foreach (var br in Defaults)
            {
                Add(true, br);
            }
        }

        /// <summary>
        /// Gets a black solid color brush.
        /// </summary>
        public static Brush Black
        {
            get { return Defaults[0]; }
        }

        /// <summary>
        /// Gets a dark blue solid color brush.
        /// </summary>
        public static Brush DarkBlue
        {
            get { return Defaults[13]; }
        }

        /// <summary>
        /// Gets a dark green solid color brush.
        /// </summary>
        public static Brush DarkGreen
        {
            get { return Defaults[7]; }
        }

        /// <summary>
        /// Gets a dark purple solid color brush.
        /// </summary>
        public static Brush DarkPurple
        {
            get { return Defaults[4]; }
        }

        /// <summary>
        /// Gets a dark red solid color brush.
        /// </summary>
        public static Brush DarkRed
        {
            get { return Defaults[10]; }
        }

        /// <summary>
        /// Gets a dark yellow solid color brush.
        /// </summary>
        public static Brush DarkYellow
        {
            get { return Defaults[1]; }
        }

        /// <summary>
        /// Gets a light blue solid color brush.
        /// </summary>
        public static Brush LightBlue
        {
            get { return Defaults[15]; }
        }

        /// <summary>
        /// Gets a light green solid color brush.
        /// </summary>
        public static Brush LightGreen
        {
            get { return Defaults[9]; }
        }

        /// <summary>
        /// Gets a light purple solid color brush.
        /// </summary>
        public static Brush LightPurple
        {
            get { return Defaults[6]; }
        }

        /// <summary>
        /// Gets a light red solid color brush.
        /// </summary>
        public static Brush LightRed
        {
            get { return Defaults[12]; }
        }

        /// <summary>
        /// Gets a light yellow solid color brush.
        /// </summary>
        public static Brush LightYellow
        {
            get { return Defaults[3]; }
        }

        /// <summary>
        /// Gets a medium blue solid color brush.
        /// </summary>
        public static Brush MediumBlue
        {
            get { return Defaults[14]; }
        }

        /// <summary>
        /// Gets a medium green solid color brush.
        /// </summary>
        public static Brush MediumGreen
        {
            get { return Defaults[8]; }
        }

        /// <summary>
        /// Gets a medium purple solid color brush.
        /// </summary>
        public static Brush MediumPurple
        {
            get { return Defaults[5]; }
        }

        /// <summary>
        /// Gets a medium red solid color brush.
        /// </summary>
        public static Brush MediumRed
        {
            get { return Defaults[11]; }
        }

        /// <summary>
        /// Gets a medium yellow solid color brush.
        /// </summary>
        public static Brush MediumYellow
        {
            get { return Defaults[2]; }
        }

        /// <summary>
        /// Gets or sets which of the three default color schemes to use: dark, medium, or light.
        /// The default is "Light."
        /// This is a static property so it is possible for one thread to overwrite
        /// the user by a different thread. This behavior may change in the future.
        /// </summary>
        public static ColorSchemeProperty ColorScheme { get; set; }

        /// <summary>
        /// This is the current background color for this particular symbol
        /// </summary>
        /// <param name="symbolCode">The symbol code</param>
        /// <returns>A brush representing the current background color for the symbol code</returns>
        public static Brush FindColorScheme(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            if (OrderOfBattle.GetCode(symbolCode) == OrderOfBattle.Civilian)
            {
                if (StandardIdentity.IsColorHostile(symbolCode))
                {
                    return GetDefault(Reds, ColorScheme);
                }

                return GetDefault(Purples, ColorScheme);
            }

            switch (StandardIdentity.GetCode(symbolCode))
            {
                case StandardIdentity.AssumedFriend:
                case StandardIdentity.ExerciseFriend:
                case StandardIdentity.Friend:
                case StandardIdentity.ExerciseAssumedFriend:
                    return GetDefault(Blues, ColorScheme);
                case StandardIdentity.Hostile:
                case StandardIdentity.Joker:
                case StandardIdentity.Faker:
                case StandardIdentity.Suspect:
                    return GetDefault(Reds, ColorScheme);
                case StandardIdentity.ExerciseNeutral:
                case StandardIdentity.Neutral:
                    return GetDefault(Greens, ColorScheme);
                case StandardIdentity.ExercisePending:
                case StandardIdentity.Pending:
                case StandardIdentity.Unknown:
                case StandardIdentity.ExerciseUnknown:
                    return GetDefault(Yellows, ColorScheme);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the background fill style for the specified brush. There is little or
        /// no reason for a user to call this method unless they wish to capture the
        /// style for other reasons.
        /// </summary>
        /// <param name="brush">The brush to use for background fills</param>
        /// <returns>The style corresponding to a fill using the specified brush</returns>
        public static Style GetFill(Brush brush)
        {
            if (brush == null)
            {
                return null;
            }

            return Fills.ContainsKey(brush) ? Fills[brush] : Add(true, brush);
        }

        /// <summary>
        /// Gets the "Present" line style associated with the specified brush,
        /// a style for drawing solid lines with a thickness of 10. There is little or
        /// no reason for a user to call this method unless they wish to capture the
        /// style for other reasons.
        /// </summary>
        /// <param name="brush">The brush to use for the solid lines, typically a solid black brush</param>
        /// <returns>The style for drawing solid lines with the specified brush</returns>
        public static Style GetLinePresent(Brush brush)
        {
            if (brush == null)
            {
                return null;
            }

            return LinePresents.ContainsKey(brush) ? LinePresents[brush] : Add(false, brush);
        }

        /// <summary>
        /// Returns the line, fill, and present Style for the given brush or adds a line, fill, and present style based on the brush.
        /// "Present" in this context refers to the object being present (solid) as opposed to anticipated (dashed).
        /// In this case, the Style has both a solid line and a solid fill.
        /// </summary>
        /// <param name="brush">
        /// The brush for which to get the Style.
        /// </param>
        /// <returns>
        /// The style corresponding to the requested brush.
        /// </returns>
        public static Style GetLineFillPresent(Brush brush)
        {
            if (brush == null)
            {
                return null;
            }

            return LineFillPresents.ContainsKey(brush) ? LineFillPresents[brush] : Add(true, brush);
        }

        /// <summary>
        /// Method to load up the various default line and fill styles
        /// </summary>
        /// <param name="isFill">
        /// True if the style includes a fill.
        /// </param>
        /// <param name="br">
        /// The brush to be used for the given style.
        /// </param>
        /// <returns>
        /// Returns the style while also adding it to the list of default styles.
        /// </returns>
        internal static Style Add(bool isFill, Brush br)
        {
            // Load up Fills
            var styleFill = new Style(typeof(Shape));
            styleFill.Setters.Add(new Setter(Shape.FillProperty, br));
            if (!Fills.ContainsKey(br))
            {
                Fills.Add(br, styleFill);
            }

            // Load up LinePresents and LineFillPresents
            var stylePresent = new Style(typeof(Shape)) { BasedOn = BlackPresent };
            if (isFill)
            {
                if (br != Black)
                {
                    stylePresent.Setters.Add(new Setter(Shape.FillProperty, br));
                }

                if (!LineFillPresents.ContainsKey(br))
                {
                    LineFillPresents.Add(br, stylePresent);
                }
            }
            else
            {
                if (br != Black)
                {
                    stylePresent.Setters.Add(new Setter(Shape.StrokeProperty, br));
                }

                if (!LinePresents.ContainsKey(br))
                {
                    LinePresents.Add(br, stylePresent);
                }
            }

            if (isFill)
            {
                return styleFill;
            }

            return stylePresent;
        }

        /// <summary>
        /// Returns the appropriate brush type according to the current color scheme
        /// </summary>
        /// <param name="colorOffset">
        /// The color offset in the array of Default colors.
        /// </param>
        /// <param name="csp">
        /// The particular color scheme property, Dark, Medium, or Light
        /// </param>
        /// <returns>
        /// The brush corresponding to the color scheme property
        /// </returns>
        private static Brush GetDefault(int colorOffset, ColorSchemeProperty csp)
        {
            return csp == ColorSchemeProperty.Dark
                       ? Defaults[colorOffset]
                       : csp == ColorSchemeProperty.Medium ? Defaults[colorOffset + 1] : Defaults[colorOffset + 2];
        }
    }
}