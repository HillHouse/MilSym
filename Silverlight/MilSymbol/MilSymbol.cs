// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilSymbol.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   The main interface for single point symbology as defined in MIL STD-2525C
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
    using System.Collections.Generic;
    using System.ComponentModel;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif

    using MilSym.LoadResources;
    using MilSym.MilSymbol.Schemas;

    /// <summary>
    /// ScaleTypeValues describes how the symbol's Scale
    /// is to be interpreted. The default is that the
    /// Scale is applied as a fraction of the native
    /// height for that symbol.
    /// </summary>
    public enum ScaleTypeValues
    {
        /// <summary>
        /// The size of the symbol is that fraction of the symbol's native size.
        /// A typical number is 0.13 for a height of about 40 pixels assuming
        /// each symbol is (very) roughly 300 x 300 pixels in size.
        /// </summary>
        NativeSize,

        /// <summary>
        /// The symbol heights are equal.
        /// A typical number is 40 for a height of about 40 pixels.
        /// Each symbol will have the same height on the display.
        /// </summary>
        EqualHeights,

        /// <summary>
        /// The symbol areas are equal.
        /// A typical number is 40 for a height of about 40 pixels.
        /// Each symbol will have the same area on the display.
        /// </summary>
        EqualAreas
    }

    /// <summary>
    /// The MilSymbol class is the only class of interest for
    /// most developers who wish to plot military symbology.
    /// Its various methods support any valid, and some invalid,
    /// symbol codes (once fully implemented). Labels
    /// can be specified as well as symbol scales and brushes to use
    /// for rendering the symbol's frame. Since MilSymbol extends
    /// Canvas it can be used anywhere that a Canvas can be used.
    /// </summary>
    public class MilSymbol : Canvas, INotifyPropertyChanged
    {
        /// <summary>
        /// The message logger
        /// </summary>
        private static readonly ILogger Log = LoggerFactory<MilSymbol>.GetLogger();

        /// <summary>
        /// The AA label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelAAProperty = DependencyProperty.Register(
            "LabelAA", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The C label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelCProperty = DependencyProperty.Register(
            "LabelC", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The F label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelFProperty = DependencyProperty.Register(
            "LabelF", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The G label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelGProperty = DependencyProperty.Register(
            "LabelG", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The H label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelHProperty = DependencyProperty.Register(
            "LabelH", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The H1 label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelH1Property = DependencyProperty.Register(
            "LabelH1", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The J label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelJProperty = DependencyProperty.Register(
            "LabelJ", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The K label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelKProperty = DependencyProperty.Register(
            "LabelK", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The L label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelLProperty = DependencyProperty.Register(
            "LabelL", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The M label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelMProperty = DependencyProperty.Register(
            "LabelM", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The N label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelNProperty = DependencyProperty.Register(
            "LabelN", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The P label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelPProperty = DependencyProperty.Register(
            "LabelP", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The Q label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelQProperty = DependencyProperty.Register(
            "LabelQ", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The T label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelTProperty = DependencyProperty.Register(
            "LabelT", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The V label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelVProperty = DependencyProperty.Register(
            "LabelV", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The W label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelWProperty = DependencyProperty.Register(
            "LabelW", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The W1 label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelW1Property = DependencyProperty.Register(
            "LabelW1", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The X label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelXProperty = DependencyProperty.Register(
            "LabelX", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The Y label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelYProperty = DependencyProperty.Register(
            "LabelY", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// The Z label as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty LabelZProperty = DependencyProperty.Register(
            "LabelZ", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelsChanged));

        /// <summary>
        /// A dictionary that maps the graphical element for a label to the MIL-STD 2525C designation for that label.
        /// </summary>
        private static readonly IDictionary<DependencyProperty, string> Dependencies =
            new Dictionary<DependencyProperty, string>
                {
                    { LabelCProperty, "C" },
                    { LabelFProperty, "F" },
                    { LabelGProperty, "G" },
                    { LabelHProperty, "H" },
                    { LabelH1Property, "H1" },
                    { LabelJProperty, "J" },
                    { LabelKProperty, "K" },
                    { LabelLProperty, "L" },
                    { LabelMProperty, "M" },
                    { LabelNProperty, "N" },
                    { LabelPProperty, "P" },
                    { LabelQProperty, "Q" },
                    { LabelTProperty, "T" },
                    { LabelVProperty, "V" },
                    { LabelWProperty, "W" },
                    { LabelW1Property, "W1" },
                    { LabelXProperty, "X" },
                    { LabelYProperty, "Y" },
                    { LabelZProperty, "Z" },
                    { LabelAAProperty, "AA" }
                };

        /// <summary>
        /// The rotation angle (in degrees measured clockwise from true north) at which to display the symbol. 
        /// </summary>
        private static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(MilSymbol), new PropertyMetadata(0.0, OnTransformChanged));

        /// <summary>
        /// The brush used to fill the symbol's background.
        /// </summary>
        private static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register(
            "FillBrush", typeof(Brush), typeof(MilSymbol), new PropertyMetadata(null, OnSymbolChanged));

        /// <summary>
        /// A single string potentially representing multiple label properties.
        /// </summary>
        private static readonly DependencyProperty LabelStringProperty = DependencyProperty.Register(
            "LabelString", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnLabelStringChanged));

        /// <summary>
        /// The style to apply to the labels for the symbol.
        /// </summary>
        private static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register(
            "LabelStyle", typeof(Style), typeof(MilSymbol), new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        /// The brush to be used when rendering the outline of the symbol.
        /// </summary>
        private static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register(
            "LineBrush", typeof(Brush), typeof(MilSymbol), new PropertyMetadata(null, OnSymbolChanged));

        /// <summary>
        /// The relative size of the symbol.
        /// </summary>
        private static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale", typeof(double), typeof(MilSymbol), new PropertyMetadata(0.0, OnTransformChanged));

        /// <summary>
        /// The code for the symbol as defined in MIL-STD 2525C.
        /// </summary>
        private static readonly DependencyProperty SymbolCodeProperty = DependencyProperty.Register(
            "SymbolCode", typeof(string), typeof(MilSymbol), new PropertyMetadata(null, OnSymbolChanged));

        /// <summary>
        /// The backing store for TextVisiblity, defaults to Visiblity.Visible
        /// </summary>
        private static readonly DependencyProperty TextVisibilityProperty = DependencyProperty.Register(
            "TextVisibility", typeof(Visibility), typeof(MilSymbol), new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        /// <summary>
        /// A convenience dictionary that tracks elements with a label that can be used to replace that
        /// labeled portion of the symbol following a change.
        /// </summary>
        private readonly IDictionary<string, UIElement> elements = new Dictionary<string, UIElement>();

        /// <summary>
        /// Flag used to indicate whether or not it is acceptable to redraw the symbol.
        /// </summary>
        private readonly bool suppressRefresh;

        /// <summary>
        /// A variable for saving the scale factor, whose value depends on the scale type.
        /// The scale factor is used to set the element's scaling matrix.
        /// </summary>
        private double scaleFactor = 1.0;

        /// <summary>
        /// The backing store variable for Bounds
        /// </summary>
        private Rect bounds = Rect.Empty;

        /// <summary>
        /// How high is the top of the symbol after various decorations?
        /// </summary>
        private double high;

        /// <summary>
        /// The dictionary that maps the label name (as defined in MIL-STD 2525C) to the label contents.
        /// </summary>
        private IDictionary<string, string> labels; // A-Z, AA-AZ, A1-Z1, etc. - null array unless user sets a label

        /// <summary>
        /// Initializes a new instance of the <see cref="MilSymbol"/> class.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public MilSymbol()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MilSymbol class, a canvas representing a military symbol code
        /// </summary>
        /// <param name="symbolCode">15 character code from the 2525C symbology manual</param>
        /// <param name="scale">the scale of the entity as a fraction of the default size (about 300x300 pixels)</param>
        /// <param name="labelsIn">a string representing an array of labels to be displayed near the symbol, e.g., "X=100;H=a string"</param>
        public MilSymbol(string symbolCode, double scale, string labelsIn)
            : this(symbolCode, scale, labelString: labelsIn)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MilSymbol class, a canvas representing a military symbol code
        /// </summary>
        /// <param name="symbolCode">15 character code from the 2525C symbology manual</param>
        /// <param name="scaleIn">the scale of the entity as a fraction of the default size (about 300x300 pixels)</param>
        public MilSymbol(string symbolCode, double scaleIn)
            : this(symbolCode, scale: scaleIn)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MilSymbol class, a canvas representing a military symbol code
        /// </summary>
        /// <param name="symbolCode">The 15 character code from the 2525C symbology manual</param>
        /// <param name="scale">The scale of the entity as a fraction of the default size (about 300x300 pixels)</param>
        /// <param name="labelString">A string representing an array of labels to be displayed near the symbol, e.g., "X=100;H=a string"</param>
        /// <param name="lineBrush">A brush to color the base symbol outline </param>
        /// <param name="fillBrush">A brush to color the base symbol background</param>
        /// <param name="labelStyle">A style to use for labels</param>
        /// <param name="angle">The clockwise rotation angle for the symbol, in degrees</param>
        public MilSymbol(
            string symbolCode,
            double scale = 0.14,
            string labelString = "",
            Brush lineBrush = null,
            Brush fillBrush = null,
            Style labelStyle = null,
            double angle = 0.0)
        {
            try
            {
                this.suppressRefresh = true; 
                
                // As soon as we set the Scale we're 
                // going to need the scaleFactor
                // so we're going to set it now.
                // The only thing that should change
                // scaleFactor is a change in the symbol
                // code.
                this.SetScaleFactor(symbolCode);

                this.Scale = scale;
                this.LineBrush = lineBrush;
                this.FillBrush = fillBrush;
                this.LabelString = labelString;
                this.LabelStyle = labelStyle;
                this.Angle = angle;
                this.suppressRefresh = false;
                this.SymbolCode = symbolCode;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(LogLevel.Error, "Unable to construct symbol", ex);
            }
        }

        /// <summary>
        /// The PropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the ScaleType. The ScaleType enumeration
        /// determines how MilSymbol Scale values are interpreted.
        /// As a static value, it is applied to each subsequent
        /// symbol until changed.
        /// </summary>
        public static ScaleTypeValues ScaleType { private get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the symbol IsDirty.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets Bounds which defines the bounding rectangle.
        /// </summary>
        public Rect Bounds
        {
            get
            {
                return this.bounds;
            }
        }

        /// <summary>
        /// Gets or sets the scale with which to render the symbol. The default
        /// Scale of 1.0 is rather large (approximately 300x300).
        /// In practice a Scale of 0.1 to 0.2 is effective. When
        /// symbols are rendered in a browser that supports magnification
        /// it is possible to change the zoom factor to see more detail.
        /// </summary>
        public double Scale
        {
            get
            {
                return (double)this.GetValue(ScaleProperty);
            }

            set
            {
                this.SetValue(ScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle for the current symbol. The standard 
        /// severely limits the occasions when rotating a symbol is
        /// allowed so this option should be used with caution.
        /// </summary>
        public double Angle
        {
            get
            {
                return (double)this.GetValue(AngleProperty);
            }

            set
            {
                this.SetValue(AngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label C - quantity 
        /// </summary>
        public string LabelC
        {
            get
            {
                return (string)this.GetValue(LabelCProperty);
            }

            set
            {
                this.SetValue(LabelCProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label F where + => reinforced, - => reduced, ± => reinforced and reduced
        /// </summary>
        public string LabelF
        {
            get
            {
                return (string)this.GetValue(LabelFProperty);
            }

            set
            {
                this.SetValue(LabelFProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label G - staff comments
        /// </summary>
        public string LabelG
        {
            get
            {
                return (string)this.GetValue(LabelGProperty);
            }

            set
            {
                this.SetValue(LabelGProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label H - additional information
        /// </summary>
        public string LabelH
        {
            get
            {
                return (string)this.GetValue(LabelHProperty);
            }

            set
            {
                this.SetValue(LabelHProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label H1 - additional information
        /// </summary>
        public string LabelH1
        {
            get
            {
                return (string)this.GetValue(LabelH1Property);
            }

            set
            {
                this.SetValue(LabelH1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets label J - evaluation (reliability plus credibility) 
        ///   A-completely reliable           B-usually reliable    C-fairly reliable
        ///   D-not usually reliable          E-unreliable          F-reliability cannot be judged
        ///   1-confirmed by other sources    2-probably true       3-possibly true
        ///   4-doubtfully true               5-improbable          6-truth cannot be judged
        /// </summary>
        public string LabelJ
        {
            get
            {
                return (string)this.GetValue(LabelJProperty);
            }

            set
            {
                this.SetValue(LabelJProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label K - combat effectiveness
        /// </summary>
        public string LabelK
        {
            get
            {
                return (string)this.GetValue(LabelKProperty);
            }

            set
            {
                this.SetValue(LabelKProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label L - signature equipment (! indicates detectable electronic signatures)
        /// </summary>
        public string LabelL
        {
            get
            {
                return (string)this.GetValue(LabelLProperty);
            }

            set
            {
                this.SetValue(LabelLProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label M - higher formation (number/title of higher command, Roman numerals designate corps)
        /// </summary>
        public string LabelM
        {
            get
            {
                return (string)this.GetValue(LabelMProperty);
            }

            set
            {
                this.SetValue(LabelMProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label N - enemy (ENY) for equipment
        /// </summary>
        public string LabelN
        {
            get
            {
                return (string)this.GetValue(LabelNProperty);
            }

            set
            {
                this.SetValue(LabelNProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label P - IFF/SIF identification modes and codes
        /// </summary>
        public string LabelP
        {
            get
            {
                return (string)this.GetValue(LabelPProperty);
            }

            set
            {
                this.SetValue(LabelPProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label Q - degrees measured clockwise from true north 
        /// </summary>
        public string LabelQ
        {
            get
            {
                return (string)this.GetValue(LabelQProperty);
            }

            set
            {
                this.SetValue(LabelQProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label T - unique designation (acquisition number when used in SIGINT)
        /// </summary>
        public string LabelT
        {
            get
            {
                return (string)this.GetValue(LabelTProperty);
            }

            set
            {
                this.SetValue(LabelTProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label V - equipment type
        /// </summary>
        public string LabelV
        {
            get
            {
                return (string)this.GetValue(LabelVProperty);
            }

            set
            {
                this.SetValue(LabelVProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label W - date/time as DDHHMMSSZMONYYYY or O/O
        /// </summary>
        public string LabelW
        {
            get
            {
                return (string)this.GetValue(LabelWProperty);
            }

            set
            {
                this.SetValue(LabelWProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label W1 - date/time as DDHHMMSSZMONYYYY or O/O
        /// </summary>
        public string LabelW1
        {
            get
            {
                return (string)this.GetValue(LabelW1Property);
            }

            set
            {
                this.SetValue(LabelW1Property, value);
            }
        }

        /// <summary>
        ///  Gets or sets label X - altitude or depth
        /// </summary>
        public string LabelX
        {
            get
            {
                return (string)this.GetValue(LabelXProperty);
            }

            set
            {
                this.SetValue(LabelXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label Y - location (DMS, UTM, or MGRS)
        /// </summary>
        public string LabelY
        {
            get
            {
                return (string)this.GetValue(LabelYProperty);
            }

            set
            {
                this.SetValue(LabelYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label Z - speed
        /// </summary>
        public string LabelZ
        {
            get
            {
                return (string)this.GetValue(LabelZProperty);
            }

            set
            {
                this.SetValue(LabelZProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets label AA - name of special C2 headquarters
        /// </summary>
        public string LabelAA
        {
            get
            {
                return (string)this.GetValue(LabelAAProperty);
            }

            set
            {
                this.SetValue(LabelAAProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the symbol code property for the current military symbol.
        /// Valid symbol codes are fully defined by the MIL-STD 2525C specification.
        /// As of this writing that specification is the first reference at the Wikipedia
        /// link http://en.wikipedia.org/wiki/Military_symbology.
        /// </summary>
        public string SymbolCode
        {
            get
            {
                return (string)this.GetValue(SymbolCodeProperty);
            }

            set
            {
                // A side effect of changing the symbol code is that we might need
                // a new scale factor.
                if (!string.IsNullOrEmpty(this.SymbolCode))
                {
                    this.SetScaleFactor(value);
                }

                this.SetValue(SymbolCodeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label style property for the current military symbol.
        /// Currently on the FontFamily, FontSize, FontWeight, and Foreground properties are used.
        /// </summary>
        public Style LabelStyle
        {
            get
            {
                return (Style)this.GetValue(LabelStyleProperty);
            }

            set
            {
                this.SetValue(LabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets this write-only string property to provide an easy way to set
        /// multiple labels at one time. The syntax is
        /// "label=value;label=value;..." where '=' and ';' can be
        /// any two characters that are ONLY used as separators in the
        /// string. It is also possible to use the same character for both
        /// '=' and ';' with a subsequent loss of readability.
        /// </summary>
        public string LabelString
        {
            private get
            {
                return (string)this.GetValue(LabelStringProperty);
            }

            set
            {
                this.SetValue(LabelStringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush for drawing the outline of a symbol, typically black and either solid or dashed.
        /// </summary>
        public Brush LineBrush
        {
            get
            {
                return (Brush)this.GetValue(LineBrushProperty);
            }

            set
            {
                this.SetValue(LineBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush for drawing the background of a symbol, typically dependent on the symbol's standard identity (aka affiliation).
        /// </summary>
        public Brush FillBrush
        {
            get
            {
                return (Brush)this.GetValue(FillBrushProperty);
            }

            set
            {
                this.SetValue(FillBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text visibility for a symbol
        /// </summary>
        public Visibility TextVisibility
        {
            get
            {
                return (Visibility)this.GetValue(TextVisibilityProperty);
            }

            set
            {
                this.SetValue(TextVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the requested military symbol is
        /// not available for whatever reason.
        /// If no symbol is generated, the Empty flag will be set to
        /// true and an error message will potentially be
        /// generated on the console log.
        /// </summary>
        /// <returns>true if there is no symbol yet</returns>
        public bool Empty
        {
            get
            {
                return Children.Count == 0;
            }
        }

        /// <summary>
        /// Gets "BaseRect" - the base rectangle for the code symbol - gets used a lot for symbol decorations.
        /// </summary>
        internal Rect BaseRect { get; private set; }

        /// <summary>
        /// Notify property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name that changed.
        /// </param>
        public void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets MilSymbol elements by name.
        /// </summary>
        /// <param name="name">The name of the element, e.g., "Base."</param>
        /// <returns>The UIElement corresponding to the name.</returns>
        public UIElement GetChild(string name)
        {
            if (this.elements.ContainsKey(name))
            {
                return this.elements[name];
            }
            return null;
        }

        /// <summary>
        /// Tracks which elements have been plotted vis a vis a military symbol.
        /// In the future, we'll be smart about which ones to update when
        /// bound data changes.
        /// </summary>
        /// <param name="name">The label to apply to the UIElement.</param>
        /// <param name="ue">The symbol's UIElement to add/replace/update.</param>
        internal void AddChild(string name, UIElement ue)
        {
            if (ue == null)
            {
                if (this.elements.ContainsKey(name))
                {
                    UIElement oldU = this.elements[name]; // remove any existing references
                    this.elements.Remove(name);
                    Children.Remove(oldU); // shouldn't hurt if not there
                }

                return;
            }

            Rect rect;
            if (ue is Path)
            {
                rect = ((Path)ue).Data.Bounds; // most graphical elements are now paths
            }
            else if (ue is ContentControl)
            {
                var height = (double)ue.GetValue(HeightProperty);
                var width = (double)ue.GetValue(WidthProperty);
                if (double.IsNaN(height) || double.IsNaN(width))
                {
                    rect = Rect.Empty;
                }
                else
                {
                    rect = new Rect(-width / 2.0, -height / 2.0, width, height);
                }

                this.BaseRect = rect; // we need these dimensions in a number of places
            }
            else
            {
                rect = new Rect(
                    (double)ue.GetValue(LeftProperty),
                    (double)ue.GetValue(TopProperty),
                    (double)ue.GetValue(WidthProperty),
                    (double)ue.GetValue(HeightProperty));
            }

            if (name != "Base")
            {
                ue.IsHitTestVisible = false;
            }

            // Make the bounding box bigger
            if (!rect.IsEmpty)
            {
                this.bounds.Union(rect);
                this.Width = this.bounds.Width;
                this.Height = this.bounds.Height;
            }

            if (this.elements.ContainsKey(name))
            {
                UIElement oldU = this.elements[name];
                this.elements[name] = ue;
                int index = Children.IndexOf(oldU); // preserve rendering order
                Children.Remove(oldU);              // shouldn't hurt if not there
                if (index == -1)
                {
                    Children.Add(ue);               // some evil person deleted our child!
                }
                else
                {
                    Children.Insert(index, ue);     // insert new element where old one was
                }
            }
            else
            {
                this.elements.Add(name, ue);        // just add the element
                Children.Add(ue);
            }
        }

        /// <summary>
        /// Brute force event handler for when the symbol's text visibility needs changing.
        /// </summary>
        /// <param name="dp">
        /// The symbol whose text visibility needs changing.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnVisibilityChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            if (dp is MilSymbol ms)
            {
                var vis = ms.TextVisibility;

                // Enumerate all of the children and adjust the text blocks
                foreach (var element in ms.Children)
                {
                    if (element is TextBlock textBlock)
                    {
                        textBlock.Visibility = vis;
                    }
                    else if (element is MilSymbolBase msb &&
                        VisualTreeHelper.GetChild(msb, 0) is Canvas canvas)
                    {
                        foreach (var ele in canvas.Children)
                        {
                            if (ele is TextBlock tb)
                            {
                                tb.Visibility = vis;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the symbols transform matrix needs changing.
        /// </summary>
        /// <param name="dp">
        /// The symbol whose transform needs changing.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnTransformChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            if (dp is MilSymbol ms)
            {
                ms.SetTransform();
            }
        }

        /// <summary>
        /// Event handler for when the symbols labels need changing.
        /// </summary>
        /// <param name="dp">
        /// The symbol whose labels need changing.
        /// </param>
        /// <param name="ea">
        /// The "ea" property value is the key into the Dependencies for which the label changed.
        /// </param>
        private static void OnLabelsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            if (!Dependencies.ContainsKey(ea.Property))
            {
                return;
            }

            if (dp is MilSymbol ms)
            {
                string key = Dependencies[ea.Property];
                if (ms.labels == null)
                {
                    ms.labels = new Dictionary<string, string>();
                }

                var label = ea.NewValue as string;
                if (ms.labels.ContainsKey(key))
                {
                    ms.labels[key] = label;
                }
                else if (label != null)
                {
                    ms.labels.Add(key, label);
                }

                ms.GenerateLabels(null);
            }
        }

        /// <summary>
        /// Event handler for when the symbol has changed.
        /// </summary>
        /// <param name="dp">
        /// The symbol that has changed.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnSymbolChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            if (dp is MilSymbol ms)
            {
                ms.GenerateSymbol();
            }
        }

        /// <summary>
        /// Event handler for when the label style has changed.
        /// </summary>
        /// <param name="dp">
        /// The symbol that has changed.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnLabelStyleChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            if (dp is MilSymbol ms)
            {
                ms.GenerateLabels(null);
            }
        }

        /// <summary>
        /// Event handler for when the label string has changed.
        /// </summary>
        /// <param name="dp">
        /// The symbol whose label string has changed.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnLabelStringChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            if (dp is MilSymbol ms)
            {
                ms.labels = MilLabels.Generate(ms.LabelString, ms.labels);
                ms.GenerateLabels(null);
            }
        }

        /// <summary>
        /// Computes the scale factor used when scaling the symbol.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code whose scale factor we need.
        /// </param>
        private void SetScaleFactor(string symbolCode)
        {
            this.scaleFactor = 1.0;
            switch (ScaleType)
            {
                case ScaleTypeValues.EqualAreas:
                    this.scaleFactor = 1.0 / SymbolData.GetScaling(symbolCode).AreaFactor;
                    break;
                case ScaleTypeValues.EqualHeights:
                    this.scaleFactor = 1.0 / SymbolData.GetScaling(symbolCode).LinearFactor;
                    break;
            }
        }

        /// <summary>
        /// Generates the rotate and scale transforms upon demand, adding them to the symbol.
        /// </summary>
        private void SetTransform()
        {
            var rt = new RotateTransform { Angle = this.Angle };
            var size = this.Scale * this.scaleFactor;
            var st = new ScaleTransform { ScaleX = size, ScaleY = Math.Abs(size) };
            var tg = new TransformGroup();
            tg.Children.Add(rt);
            tg.Children.Add(st);
            this.RenderTransform = tg;
        }

        /// <summary>
        /// Selectively (re)generates labels.
        /// </summary>
        /// <param name="styles">
        /// An optional list of styles to apply to the labels.
        /// </param>
        private void GenerateLabels(IList<Style> styles)
        {
            try
            {
                if (this.suppressRefresh)
                {
                    return;
                }

                string symbolCode = this.SymbolCode;
                if (symbolCode == null)
                {
                    return;
                }

                if (this.labels == null)
                {
                    return;
                }

                if (styles == null)
                {
                    styles = MilLabels.GetStyles(this.LabelStyle);
                }

                this.IsDirty = true;

                int schemeKey = CodingScheme.GetCode(symbolCode);
                if (schemeKey == CodingScheme.Weather)
                {
                    // All extraneous information for weather is in the labels.
                    MilLabels.GenerateWeather(symbolCode, this.labels, this.AddChild);
                    return;
                }

                if (schemeKey == CodingScheme.TacticalGraphics)
                {
                    MilLabels.GenerateTacticalGraphics(
                        symbolCode, this.labels, styles[MilLabels.TacticalGraphicsLabels], this.AddChild);
                    return;
                }

                // Generate labels to the left of the symbol
                this.AddChild("Left", MilLabels.GenerateLeft(symbolCode, this.labels, styles[MilLabels.LeftLabels]));

                // Generate labels to the right of the symbol
                this.AddChild("Right", MilLabels.GenerateRight(symbolCode, this.labels, styles[MilLabels.RightLabels]));

                // This is just the quantity, if specified
                this.AddChild("Top", MilLabels.GenerateTop(this.high, symbolCode, this.labels, styles[MilLabels.TopLabels]));

                // This is just the C2 headquarters label
                this.AddChild("Middle", MilLabels.GenerateMiddle(symbolCode, this.labels));

                // This is the direction of movement (which apparently requires a label value)
                this.AddChild("Q", MilLabels.GenerateQ(symbolCode, this.labels, 0.0));
            }
            catch (Exception ex)
            {
                Log.WriteMessage(LogLevel.Error, "Unable to add labels to symbol", ex);
            }
        }

        /// <summary>
        /// Always generates the entire symbol.
        /// The symbol code should not change very often.
        /// </summary>
        private void GenerateSymbol()
        {
            try
            {
                // Only draw after we have the symbol code, if drawing via method call
                if (this.suppressRefresh)
                {
                    return;
                }

                string symbolCode = this.SymbolCode;

                // Reinitialize 
                this.IsDirty = true;        // set a dirty flag to indicate the symbol needs updating
                this.bounds = Rect.Empty;
                this.BaseRect = Rect.Empty;
                this.Children.Clear();      // tried to cheap this one out - but that doesn't work well
                this.elements.Clear();

                // Get the base symbol from "the" resource dictionary 
                var baseSymbol = new MilSymbolBase(symbolCode, this.LineBrush, this.FillBrush);
                if (baseSymbol.Empty)
                {
                    return;
                }

                this.AddChild("Base", baseSymbol);

                var styles = MilLabels.GetStyles(this.LabelStyle);

                // There is only label decoration for weather codes
                int schemeKey = CodingScheme.GetCode(symbolCode);
                if (schemeKey == CodingScheme.Weather || schemeKey == CodingScheme.TacticalGraphics)
                {
                    this.GenerateLabels(styles);
                    return;
                }

                // Add in the echelon marking.
                // "high" is the maximum height from the decoration
                this.high = Echelon.Generate(this, symbolCode);

                // Draw headquarters, feint dummy, task force, and installation.
                // "high" is the maximum height from the decoration
                this.high = MilHats.Generate(this, symbolCode);

                // Take care of any Joker, Faker, or Exercise character
                this.AddChild(
                    "JFE", MilLabels.GenerateJokerFakerExercise(symbolCode, this.labels, styles[MilLabels.BigLabels]));

                // Add the black ribbon on the base symbol for space
                this.AddChild("Space", MilSymbolBase.GenerateSpace(symbolCode));

                // Indicate whether the entity is damaged or destroyed
                this.AddChild("OC", StatusOperationalCapacity.Generate(symbolCode));

                // Add the mobility to the base of the symbol
                this.AddChild("Mobility", DrawMobility.GenerateMobility(symbolCode));

                // We have to (re)generate the labels because the symbol code extent may be different
                this.GenerateLabels(styles);
            }
            catch (Exception ex)
            {
                Log.WriteMessage(LogLevel.Error, "Unable to construct military symbol", ex);
            }
        }
    }
}