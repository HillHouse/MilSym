// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="MilGraphic.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   The main file for rendering the multipoint graphics in Appendix B of MIL STD-2525C
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilGraph
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using MilSym.LoadResources;
    using MilSym.MilGraph.Support;
    using MilSym.MilSymbol;

    /// <summary>
    /// An enumerated list of possible actions to take on rendering text
    /// depending on the orientation of the MilGraphic.
    /// </summary>
    internal enum Flip
    {
        /// <summary>
        /// Neither the x or the y coordinate need to be flipped to make the text readable
        /// </summary>
        NoFlip,

        /// <summary>
        /// The x coordinate needs to be flipped to make the text readable
        /// </summary>
        XFlip,

        /// <summary>
        /// The y coordinate needs to be flipped to make the text readable
        /// </summary>
        YFlip,

        /// <summary>
        /// The x and the y coordinate need to be flipped to make the text readable
        /// </summary>
        BothFlip
    }

    /// <summary>
    /// The primary class for rendering the multipoint graphics in Appendix B of MIL STD-2525C
    /// </summary>
    public class MilGraphic : Canvas, INotifyPropertyChanged, ILocatable
    {
        /// <summary>
        /// An arbitrary convenience scale factor to help with too many zeros when debugging.
        /// </summary>
        private const double ConvenienceScaleFactor = 100000.0;

        /// <summary>
        /// Used to extend the bounding rectangle for the object to account for decoration.
        /// The rectangle is used for visibility tests, so a too large rectangle is not a problem.
        /// </summary>
        private const double BufferScale = 0.25;

        /// <summary>
        /// A convenient distance value that is helpful in computing text angles.
        /// </summary>
        private const double ArbitraryDistance = 10.0;

        /// <summary>
        /// The message logger
        /// </summary>
        private static readonly ILogger Log = LoggerFactory<MilGraphic>.GetLogger();

        /// <summary>
        /// The DependencyProperty for LabelT.
        /// </summary>
        private static readonly DependencyProperty LabelTProperty = DependencyProperty.Register(
            "LabelT", typeof(string), typeof(MilGraphic), new PropertyMetadata(string.Empty, OnLabelsChanged));

        /// <summary>
        /// The DependencyProperty for LabelT1.
        /// </summary>
        private static readonly DependencyProperty LabelT1Property = DependencyProperty.Register(
            "LabelT1", typeof(string), typeof(MilGraphic), new PropertyMetadata(string.Empty, OnLabelsChanged));

        /// <summary>
        /// The DependencyProperty for LabelW.
        /// </summary>
        private static readonly DependencyProperty LabelWProperty = DependencyProperty.Register(
            "LabelW", typeof(string), typeof(MilGraphic), new PropertyMetadata(string.Empty, OnLabelsChanged));

        /// <summary>
        /// The DependencyProperty for LabelW1.
        /// </summary>
        private static readonly DependencyProperty LabelW1Property = DependencyProperty.Register(
            "LabelW1", typeof(string), typeof(MilGraphic), new PropertyMetadata(string.Empty, OnLabelsChanged));

        /// <summary>
        /// The DependencyProperty for LabelX.
        /// </summary>
        private static readonly DependencyProperty LabelXProperty = DependencyProperty.Register(
            "LabelX", typeof(string), typeof(MilGraphic), new PropertyMetadata(string.Empty, OnLabelsChanged));

        /// <summary>
        /// The DependencyProperty for LabelX1.
        /// </summary>
        private static readonly DependencyProperty LabelX1Property = DependencyProperty.Register(
            "LabelX1", typeof(string), typeof(MilGraphic), new PropertyMetadata(string.Empty, OnLabelsChanged));

        /// <summary>
        /// For the landing zone, etc. the user can specify the location of the T label - via this property
        /// The values represent a fraction of the symbol's extent, so the value is approximate
        /// </summary>
        private static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register(
            "LabelOffset", typeof(Offset), typeof(MilGraphic), new PropertyMetadata(new Offset(), OnLabelsChanged));

        /// <summary>
        /// The DependencyProperty for LabelString.
        /// </summary>
        private static readonly DependencyProperty LabelStringProperty = DependencyProperty.Register(
            "LabelString", typeof(string), typeof(MilGraphic), new PropertyMetadata(null, OnLabelStringChanged));

        /// <summary>
        /// The DependencyProperty for Anchors.
        /// </summary>
        private static readonly DependencyProperty AnchorsProperty = DependencyProperty.Register(
            "Anchors", typeof(ILocationCollection), typeof(MilGraphic), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for IsSpline.
        /// </summary>
        private static readonly DependencyProperty IsSplineProperty = DependencyProperty.Register(
            "IsSpline", typeof(bool), typeof(MilGraphic), new PropertyMetadata(true, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for SymbolCode.
        /// </summary>
        private static readonly DependencyProperty SymbolCodeProperty = DependencyProperty.Register(
            "SymbolCode", typeof(string), typeof(MilGraphic), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for MilSymFactory.
        /// </summary>
        private static IMilSymFactory milsymFactory;

        /// <summary>
        /// A dictionary of elements making up a symbol so we can delete
        /// them if we need to "update" them.
        /// </summary>
        private readonly IDictionary<string, UIElement> elements = new Dictionary<string, UIElement>();

        /// <summary>
        /// Suppress refresh while we set one or more values.
        /// </summary>
        private readonly bool suppressRefresh;

        /// <summary>
        /// Flag to force a complete traversal of CanvasLayoutUpdated when some attribute value has changed
        /// </summary>
        private bool isDirty;

        /// <summary>
        /// Part of scale for the graphic as we zoom in and out.
        /// </summary>
        private double scaleFactor = 1.0;

        /// <summary>
        /// The last processed zoom. Zooms start at 1.0.
        /// </summary>
        private double lastZoomLevel; 

        /// <summary>
        /// The scale transform for the graphic.
        /// </summary>
        private ScaleTransform scaleTransform;

        /// <summary>
        /// The backing store for Origin
        /// </summary>
        private ILocation origin;

        /// <summary>
        /// Flag to indicate when the code is in the middle of generating a graphic.
        /// </summary>
        private bool generatingGraphic;

        /// <summary>
        /// The DependencyProperty for Bounds.
        /// </summary>
        private Rect bounds = Rect.Empty;

        /// <summary>
        /// The DependencyProperty for LocationRect.
        /// </summary>
        private ILocationRect locationRect;

        /// <summary>
        /// The DependencyProperty for StencilType
        /// </summary>
        private string stencilType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MilGraphic"/> class.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code for the desired graphic.
        /// </param>
        /// <param name="anchors">
        /// The anchors are the one, two, three, four, or n points required for this graphic by the standard.
        /// </param>
        /// <param name="isSpline">
        /// The boolean flag that determines whether to use splines to draw an otherwise, undecorated, polyline or polygon.
        /// The splines will pass through the passed in points.
        /// </param>
        /// <param name="labelOffset">
        /// The label offset - a percentage of the extent of the object to place any, otherwise centered, labels. 
        /// An offset of (0, 0) will center the labels.
        /// </param>
        /// <param name="labelString">
        /// A string of all the labels for the graphic (for example, "T=FirstLabel;W=SecondLabel" ).
        /// </param>
        /// <param name="labelT">
        /// The label T.
        /// </param>
        /// <param name="labelT1">
        /// The label T1.
        /// </param>
        /// <param name="labelW">
        /// The label W.
        /// </param>
        /// <param name="labelW1">
        /// The label W1.
        /// </param>
        /// <param name="labelX">
        /// The label X.
        /// </param>
        /// <param name="labelX1">
        /// The label X1.
        /// </param>
        public MilGraphic(
            string symbolCode,
            ILocationCollection anchors,
            bool isSpline = false,
            Offset labelOffset = null,
            string labelString = null,
            string labelT = null,
            string labelT1 = null,
            string labelW = null,
            string labelW1 = null,
            string labelX = null,
            string labelX1 = null)
        {
            try
            {
                this.suppressRefresh = true;
                this.IsSpline = isSpline;
                this.LabelOffset = labelOffset ?? new Offset();
                if (labelString != null)
                {
                    this.LabelString = labelString;
                }

                if (labelT != null)
                {
                    this.LabelT = labelT;
                }

                if (labelT1 != null)
                {
                    this.LabelT1 = labelT1;
                }

                if (labelW != null)
                {
                    this.LabelW = labelW;
                }

                if (labelW1 != null)
                {
                    this.LabelW1 = labelW1;
                }

                if (labelX != null)
                {
                    this.LabelX = labelX;
                }

                if (labelX1 != null)
                {
                    this.LabelX1 = labelX1;
                }

                this.Anchors = anchors;
                this.SymbolCode = symbolCode;
                this.suppressRefresh = false;
                this.GenerateGraphic();
            }
            catch (Exception ex)
            {
                Log.WriteMessage(LogLevel.Error, "Unable to construct military graphic", ex);
            }
        }

        /// <summary>
        /// The PropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets "MilSymFactory" - the factory that generates the various location-specific classes.
        /// </summary>
        public static IMilSymFactory MilSymFactory
        {
            set
            {
                milsymFactory = value;
                Matrix33.MilSymFactory = value;
                MapHelper.MilSymFactory = value;
                BezierSpline.MilSymFactory = value;
                ClosedBezierSpline.MilSymFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the symbol code for a multipoint graphic
        /// </summary>
        public string SymbolCode
        {
            get { return (string)GetValue(SymbolCodeProperty); }
            set { this.SetValue(SymbolCodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Anchors for the military graphic.
        /// </summary>
        public ILocationCollection Anchors
        {
            get { return (ILocationCollection)GetValue(AnchorsProperty); }
            set { this.SetValue(AnchorsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the graphic is to use splines instead of straight lines.
        /// </summary>
        public bool IsSpline
        {
            get { return (bool)GetValue(IsSplineProperty); }
            set { this.SetValue(IsSplineProperty, value); }
        }

        /// <summary>
        /// Gets the bounding rectangle for the MilGraphic.
        /// </summary>
        public Rect Bounds
        {
            get
            {
                return this.bounds;
            }
        }

        /// <summary>
        /// Gets or sets the map layer on which the graphic is to be displayed.
        /// Currently each graphic is associated with a single map layer on a single map.
        /// Higher level functionality would be required to give the appearance that the
        /// same element is displayed on multiple maps. This might change in the future.
        /// </summary>
        public IMilSymLayer Layer { get; set; }

        /// <summary>
        /// Gets "LocationRect" - the bounding box for the graphic.
        /// </summary>
        public ILocationRect LocationRect
        {
            get
            {
                return this.locationRect;
            }
        }

        /// <summary>
        /// Gets Origin, the point on the object to set for the map.
        /// </summary>
        public ILocation Origin
        {
            get
            {
                return this.origin;
            }
        }

        /// <summary>
        /// Gets the ContentControl, as acquired from the tactical graphics ResourceDictionary.
        /// </summary>
        public MilSymbolBase ContentControl { get; private set; }

        /// <summary>
        /// Gets StencilType, a user-readable name that is used in the switch statements.
        /// </summary>
        public string StencilType
        {
            get
            {
                return this.stencilType;
            }
        }

        /// <summary>
        /// Gets or sets this string property as an easy way to set
        /// multiple labels at one time. The syntax is
        /// "label=value;label=value;..." where '=' and ';' can be
        /// any two characters that are ONLY used as separators in the
        /// string. It is also possible to use the same character for both
        /// '=' and ';' with a subsequent loss of readability.
        /// </summary>
        public string LabelString
        {
            private get { return (string)GetValue(LabelStringProperty); }
            set { this.SetValue(LabelStringProperty, value); }
        }

        /// <summary>
        /// Gets or sets LabelT.
        /// </summary>
        public string LabelT
        {
            get
            {
                return (string)GetValue(LabelTProperty);
            }

            set
            {
                this.SetValue(LabelTProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets LabelT1.
        /// </summary>
        public string LabelT1
        {
            get
            {
                return (string)GetValue(LabelT1Property);
            }

            set
            {
                this.SetValue(LabelT1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets LabelW.
        /// </summary>
        public string LabelW
        {
            get
            {
                return (string)GetValue(LabelWProperty);
            }

            set
            {
                this.SetValue(LabelWProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets LabelW1.
        /// </summary>
        public string LabelW1
        {
            get
            {
                return (string)GetValue(LabelW1Property);
            }

            set
            {
                this.SetValue(LabelW1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets LabelX.
        /// </summary>
        public string LabelX
        {
            get
            {
                return (string)GetValue(LabelXProperty);
            }

            set
            {
                this.SetValue(LabelXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets LabelX1.
        /// </summary>
        public string LabelX1
        {
            get
            {
                return (string)GetValue(LabelX1Property);
            }

            set
            {
                this.SetValue(LabelX1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets LabelOffset.
        /// </summary>
        public Offset LabelOffset
        {
            get
            {
                return (Offset)GetValue(LabelOffsetProperty);
            }

            set
            {
                this.SetValue(LabelOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets "BaseRect" - the base rectangle for the code symbol - gets used a lot for decorations.
        /// </summary>
        internal Rect BaseRect { get; private set; }

        /// <summary>
        /// The default method for anytime a property value changes for the MilGraphic.
        /// Check out http://blog.ningzhang.org/2008/11/dependencyproperty-validation-coercion_10.html
        /// for details on nesting property changes.
        /// </summary>
        /// <param name="dp">
        /// The MilGraphic for which the property has changed.
        /// </param>
        /// <param name="e">
        /// This parameter is ignored.
        /// </param>
        public static void OnPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var mg = dp as MilGraphic;
            if (mg != null && mg.SymbolCode != null)
            {
                mg.GenerateGraphic();
            }
        }

        /// <summary>
        /// Computes the "horizontal" and "vertical" text angles so we can check text direction, etc.
        /// </summary>
        /// <param name="mt">
        /// The general matrix transform.
        /// </param>
        /// <param name="point">
        /// The point where the text is to be located.
        /// </param>
        /// <returns>
        /// A Point object that actually is the horizontal and vertical text angles, in degrees.
        /// </returns>
        public static Point ComputeTextAngles(GeneralTransform mt, Point point)
        {
            // We need the text angle so we know whether to reverse the text
            var offset = mt.Transform(point);
            var horOffset = mt.Transform(new Point(point.X + ArbitraryDistance, point.Y));
            var verOffset = mt.Transform(new Point(point.X, point.Y - ArbitraryDistance)); // pointing in the positive y direction
            var horAngle = Math.Atan2(horOffset.Y - offset.Y, horOffset.X - offset.X) * 180.0 / Math.PI;
            var verAngle = Math.Atan2(verOffset.Y - offset.Y, verOffset.X - offset.X) * 180.0 / Math.PI;
            return new Point(horAngle, verAngle);
        }

        /// <summary>
        /// Compute the map initial scale factor so that zooming the map thereafter
        /// properly overlays the map locations. We do this by transforming a vector
        /// and then figuring out what scale factor makes it the "proper" length.
        /// </summary>
        /// <param name="m33">
        /// The "MilSym" transformation matrix.
        /// </param>
        /// <param name="scaleFactor">
        /// The scale factor.
        /// </param>
        /// <returns>
        /// The initial map scale factor.
        /// </returns>
        public static double ComputeScales(Matrix33 m33, out double scaleFactor)
        {
            IList<ILocation> locs = m33.Transform(
                new PointCollection { new Point(0, 0), new Point(100, 0) });
            var p = MapHelper.LatLonsToPixels(locs, 0.0);

            // The 100000.0 is just a convenience scaling factor.
            var hypot = MapHelper.Hypoteneuse(p[0], p[1]);
            var mapScale = Math.Log(100.0 / hypot) / Math.Log(2.0);

            // We'll need the ScaleFactor later when the map resizes
            scaleFactor = ConvenienceScaleFactor / MapHelper.MapSize(mapScale);
            return mapScale;
        }

        /// <summary>
        /// Transform each text element according to the orientation of the element on the map.
        /// Text might need to be flipped in either x, y or both.
        /// </summary>
        /// <param name="tb">
        /// The TextBlock containing the element to be transformed.
        /// </param>
        /// <param name="m">
        /// The matrix for the transform.
        /// </param>
        /// <param name="angles">
        /// The x and y angles for the text which determines how the text may flip.
        /// </param>
        public static void SetTextTransform(FrameworkElement tb, Matrix m, Point angles)
        {
            var x = (int)Math.Round(angles.X);
            var xmy = (int)Math.Round((angles.X - angles.Y) / 90.0); // x minus y

            // These are the 8 required cases.
            // var bothFlip = (-180 <= x && x <= -90 && xmy == -270) ||     // OK
            //             (90 <= x && x <= 180 && xmy == 90);              // OK
            // var noFlip = (-90 <= x && x <= 0 && xmy == 90) ||            // OK
            //             (0 <= x && x <= 90 && xmy == 90);                // OK
            // var xFlip = (90 <= x && x <= 180 && xmy == 270) ||           // OK
            //            (-180 <= x && x <= -90 && xmy == -90);            // OK
            // var yFlip = (-90 <= x && x <= 0 && xmy == -90) ||            // OK
            //            (0 <= x && x <= 90 && xmy == -90);                // OK
            Flip flip;
            if (xmy == 1 || xmy == -3)
            {
                flip = (-90 <= x && x <= 90) ? Flip.NoFlip : Flip.BothFlip;  // the difference is 90 degrees
            }
            else
            {
                flip = (-90 <= x && x <= 90) ? Flip.YFlip : Flip.XFlip; // the difference is -90 degrees
            }

            var top = (double)tb.GetValue(TopProperty);
            var left = (double)tb.GetValue(LeftProperty);

            switch (flip)
            {
                case Flip.NoFlip:
                    tb.RenderTransform = new MatrixTransform { Matrix = new Matrix(m.M11, m.M12, m.M21, m.M22, 0.0, 0.0) };
                    break;
                case Flip.BothFlip:
                    (tb as TextBlock).FindTextExtent();
                    left += tb.Width;
                    top += tb.Height;
                    tb.RenderTransform = new MatrixTransform { Matrix = new Matrix(-m.M11, -m.M12, -m.M21, -m.M22, 0.0, 0.0) };
                    break;
                case Flip.XFlip:
                    (tb as TextBlock).FindTextExtent();
                    left += tb.Width;
                    tb.RenderTransform = new MatrixTransform { Matrix = new Matrix(-m.M11, -m.M12, m.M21, m.M22, 0.0, 0.0) };
                    break;
                case Flip.YFlip:
                    (tb as TextBlock).FindTextExtent();
                    top += tb.Height;
                    tb.RenderTransform = new MatrixTransform { Matrix = new Matrix(m.M11, m.M12, -m.M21, -m.M22, 0.0, 0.0) };
                    break;
            }

            var pt = m.Transform(new Point(left, top));
            tb.SetValue(LeftProperty, pt.X);
            tb.SetValue(TopProperty, pt.Y);
        }

        /// <summary>
        /// Notify property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name that changed.
        /// </param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Copies the anchors from one collection into a new collection.
        /// </summary>
        /// <param name="anchors">
        /// The anchors in the original collection.
        /// </param>
        /// <returns>
        /// The new ILocationCollection.
        /// </returns>
        internal static ILocationCollection CopyAnchors(ILocationCollection anchors)
        {
            var saveAnchors = anchors;
            anchors = milsymFactory.LocationCollection();
            foreach (var a in saveAnchors)
            {
                anchors.Add(a);
            }

            return anchors;
        }

        /// <summary>
        /// Computes the full transformation needed to take the graphic object to the map.
        /// </summary>
        /// <param name="dowe">
        /// The DependencyObject that represents the MilGraphic.
        /// </param>
        /// <param name="indices">
        /// The indices are a collection of reference point pairs that are used to define
        /// possibly multiple sub-transformations to square up text and other attributes associated
        /// with a graphic - following from the fact that parts of the ResourceDictionary graphic may have to 
        /// scale unequally, depending on orientation and horizontal and vertical scaling.
        /// </param>
        /// <param name="points">
        /// The ResourceDictionary points or points in some local coordinate system.
        /// </param>
        /// <param name="anchors">
        /// The map locations corresponding to the local coordinate system points.
        /// </param>
        /// <param name="origin">
        /// The origin to be plotted directly to the map. Everything else scales around this point.
        /// For composite layouts it would be nice if it is a point in common with
        /// the various components - most typically, it is points[0].
        /// </param>
        /// <param name="scaleFactor">
        /// The scale factor.
        /// </param>
        /// <returns>
        /// The location of the origin, but now in map coordinates.
        /// </returns>
        internal static ILocation FullTransformation(
            DependencyObject dowe,
            string[] indices,
            IList<Point> points,
            IList<ILocation> anchors,
            Point origin,        // is usually, but not always, points[0]
            out double scaleFactor)
        {
            // Get the transformation that maps the x-y points for the given graphic
            // using the lat-lons provided by the user.
            Matrix33 baseM33 = MapHelper.ComputeTransform(points, anchors);

            // Now we can compute the map-based transform for the three given points
            var baseMatrix = ComputeCompleteMatrix(baseM33, points, anchors, origin, out scaleFactor);

            // We need a dictionary of matrices for the transformations below
            var matrices = new Dictionary<string, Matrix>(3) { { string.Empty, baseMatrix } };

            // And we need the transformed text angles (x and y vectors) so we know how to 
            // adjust the text (reverse in x, reverse in y, reverse in x and y)
            var angles = new Dictionary<string, Point>
            {
                { string.Empty, ComputeTextAngles(new MatrixTransform { Matrix = baseMatrix }, origin) } 
            };

            if (indices != null)
            {
                foreach (var indexString in indices)
                {
                    matrices.Add(
                        indexString,
                        ComputeCompleteMatrix(baseM33, baseMatrix, indexString, points, anchors, new Point(0, 0), out scaleFactor));
                    angles.Add(
                        indexString, ComputeTextAngles(new MatrixTransform { Matrix = matrices[indexString] }, origin));
                }
            }

            // Apply the matrix transform(s) to the content control's contents.
            // Since this method can be called multiple times, we need to make sure
            // we're not improperly modifying an earlier matrix transform.
            SetElementTransforms(dowe, matrices, angles);
            matrices.Clear();
            angles.Clear();

            // Transform the point at which to locate the graphic using the "origin" point.
            // X and Y are reversed since Location uses (Latitude, Longitude)
            return baseM33.Transform(milsymFactory.Location(Order.LatLon, origin.Y, origin.X));
        }

        /// <summary>
        /// Add a child element to the current MilGraphic
        /// </summary>
        /// <param name="name">
        /// The name to be associated with the child.
        /// </param>
        /// <param name="ue">
        /// The child element.
        /// </param>
        internal void AddChild(string name, UIElement ue)
        {
            var canvas = VisualTreeHelper.GetChild(this.ContentControl, 0) as Canvas;
            if (canvas == null)
            {
                return;
            }

            var children = (ue == this.ContentControl) ? this.Children : canvas.Children;

            if (ue == null)
            {
                // Remove any existing references
                if (this.elements.ContainsKey(name))
                {
                    UIElement oldU = this.elements[name];
                    this.elements.Remove(name);
                    children.Remove(oldU); // shouldn't hurt if not there
                }

                return;
            }

            Rect rect;
            if (ue is Path)
            {
                rect = ((Path)ue).Data.Bounds;  // most graphical elements are now paths
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

                this.BaseRect = rect;    // we need these dimensions in a number of places
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

            // Not updating the bounding box with changes to the symbol, yet
            if (this.elements.ContainsKey(name))
            {
                UIElement oldU = this.elements[name];
                this.elements[name] = ue;
                int index = Children.IndexOf(oldU); // preserve rendering order
                children.Remove(oldU);  // shouldn't hurt if not there
                if (index == -1)
                {
                    children.Add(ue);   // some evil person deleted our child!
                }
                else
                {
                    children.Insert(index, ue); // insert new element where old one was
                }
            }
            else
            {
                // Make the bounding box bigger
                if (!rect.IsEmpty)
                {
                    this.bounds.Union(rect);
                    Width = this.bounds.Width;
                    Height = this.bounds.Height;
                }

                this.elements.Add(name, ue); // just add the element
                children.Add(ue);
            }
        }

        /// <summary>
        /// Manages the changes to the label string. This is a different approach from MilSymbol.
        /// </summary>
        /// <param name="dp">
        /// The MilGraphic with the associated label string.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnLabelStringChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            var mg = dp as MilGraphic;
            if (mg == null)
            {
                return;
            }

            mg.isDirty = true;
            IDictionary<string, string> labels = new Dictionary<string, string>();
            labels = MilLabels.Generate(mg.LabelString, labels);
            foreach (var kvp in labels)
            {
                switch (kvp.Key)
                {
                    case "T":
                        mg.LabelT = kvp.Value;
                        break;
                    case "T1":
                        mg.LabelT1 = kvp.Value;
                        break;
                    case "W":
                        mg.LabelW = kvp.Value;
                        break;
                    case "W1":
                        mg.LabelW1 = kvp.Value;
                        break;
                    case "X":
                        mg.LabelX = kvp.Value;
                        break;
                    case "X1":
                        mg.LabelX1 = kvp.Value;
                        break;
                }
            }

            mg.GenerateLabels();
        }

        /// <summary>
        /// This method is called whenever there is a need to update labels.
        /// </summary>
        /// <param name="dp">
        /// The MilGraphic with the associated labels.
        /// </param>
        /// <param name="ea">
        /// This parameter is ignored.
        /// </param>
        private static void OnLabelsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
        {
            var mg = dp as MilGraphic;
            if (mg == null)
            {
                return;
            }

            mg.isDirty = true;
            mg.GenerateLabels();
        }

        /// <summary>
        /// Speciality method for the security graphic;
        /// replaces the center point of the graphic with two different points to match the four point version
        /// </summary>
        /// <param name="anchors">
        /// The three input map locations.
        /// </param>
        /// <returns>
        /// The four output map locations.
        /// </returns>
        private static IList<ILocation> SecurityScreenPoints(IList<ILocation> anchors)
        {
            var newAnchors = new List<ILocation>(4) { anchors[2] };

            var range = MapHelper.CalculateRange(anchors[2], anchors[0]);
            var bearing = MapHelper.CalculateBearing(anchors[2], anchors[0]);
            newAnchors.Add(MapHelper.CalculateLocation(anchors[2], bearing, 0.73 * range));

            range = MapHelper.CalculateRange(anchors[0], anchors[1]);
            bearing = MapHelper.CalculateBearing(anchors[0], anchors[1]);
            newAnchors.Add(MapHelper.CalculateLocation(anchors[0], bearing, 0.27 * range));
            newAnchors.Add(anchors[1]);

            anchors[0] = newAnchors[0];
            anchors[1] = newAnchors[1];
            anchors[2] = newAnchors[2];
            anchors.Add(newAnchors[3]);
            return anchors;
        }

        /// <summary>
        /// Compute a complete transformation matrix that converts a control template element to the map 
        /// (except for zoom-dependent scaling).
        /// But this matrix only uses two points of the transformation instead of three.
        /// </summary>
        /// <param name="baseM33">
        /// The 3x3 MilSym matrix that defines the transformation.
        /// </param>
        /// <param name="baseMatrix">
        /// The base Microsoft 2x3 that defines the transformation.
        /// </param>
        /// <param name="indexString">
        /// The index string representing the various coordinate pair mappings from the ResourceDictionary.
        /// </param>
        /// <param name="pointsIn">
        /// The list of coordinates typically in the ResourceDictionary's ControlTemplate space.
        /// </param>
        /// <param name="anchorsIn">
        /// The list of coordinates in map space.
        /// </param>
        /// <param name="origin">
        /// The origin in the pointsIn space about which everything else will scale.
        /// </param>
        /// <param name="scaleFactor">
        /// The scale factor to apply to get the graphic to line up properly.
        /// </param>
        /// <returns>
        /// The Matrix that will map the ControlTemplate coordinates to map coordinates.
        /// </returns>
        private static Matrix ComputeCompleteMatrix(
            Matrix33 baseM33,
            Matrix baseMatrix,
            string indexString,
            IList<Point> pointsIn,
            IList<ILocation> anchorsIn,
            Point origin,
            out double scaleFactor)
        {
            var p0 = new Point(
                anchorsIn[1].Longitude - anchorsIn[0].Longitude,
                anchorsIn[1].Latitude - anchorsIn[0].Latitude);
            var p1 = new Point(
                anchorsIn[1].Longitude - anchorsIn[2].Longitude,
                anchorsIn[1].Latitude - anchorsIn[2].Latitude);
            var left = ((p0.X * p1.Y) - (p0.Y * p1.X)) <= 0.0;

            var index0 = indexString[0] - '1';
            var index1 = indexString[1] - '1';
            if (index0 > index1) 
            {
                left = !left;   // this is a convention to allow us to set the normal direction
            }

            IList<ILocation> anchors = new List<ILocation> { anchorsIn[index0], anchorsIn[index1] };
            IList<Point> points = new List<Point> { pointsIn[index0], pointsIn[index1] };
            ExtendPoints(ref points, ref anchors, left);
            Matrix m = ComputeCompleteMatrix(baseM33, points, anchors, origin, out scaleFactor);

            var a = baseMatrix.Transform(points[0]);
            var b = m.Transform(points[0]);
            m.OffsetX = a.X - b.X;
            m.OffsetY = a.Y - b.Y;
            return m;
        }

        /// <summary>
        /// Compute a complete transformation matrix that converts a control template element to the map 
        /// (except for zoom-dependent scaling)
        /// </summary>
        /// <param name="m33">
        /// The 3x3 MilSym matrix that defines the transformation.
        /// </param>
        /// <param name="points">
        /// The list of coordinate points in the ResourceDictionary, or ControlTemplate, space.
        /// </param>
        /// <param name="anchors">
        /// The list of coordinates in map space.
        /// </param>
        /// <param name="origin">
        /// The origin of the coordinate points about which everything will scale.
        /// </param>
        /// <param name="scaleFactor">
        /// The normalizing scale factor that will scale the template properly on the map.
        /// </param>
        /// <returns>
        /// The traditional 3x2 Matrix that scales the template properly on the map.
        /// </returns>
        private static Matrix ComputeCompleteMatrix(
            Matrix33 m33,
            IList<Point> points,
            IList<ILocation> anchors,
            Point origin,
            out double scaleFactor)
        {
            // Find the map scale at which the transformed size equals the requested size
            // (for the specified location).
            // The map size at this scale will be the normalizing scale factor. 
            double mapScale = ComputeScales(m33, out scaleFactor);

            // Get the transform from the control template to pixel space
            var final = MapHelper.ComputeTransform(points, MapHelper.LatLonsToPixels(anchors, mapScale));

            // We've got the scale, skew, and rotation but we still need the translation.
            var mt = new MatrixTransform { Matrix = new Matrix(final.M11, final.M12, final.M21, final.M22, 0.0, 0.0) };

            // Transform a reference (origin) point to get the translation.
            // Any point will do here as long as it is used for the PositionProperty too.
            var offset = mt.Transform(origin);
            var mat = new Matrix(final.M11, final.M12, final.M21, final.M22, -offset.X, -offset.Y);
            return mat;
        }

        /// <summary>
        /// Unfortunately, each element in the ContentControl must be transformed, possibly differently from other
        /// elements in the same control. This is not very efficient. Alternatives include generating the elements
        /// via code or doing some type of binding in the control templates.
        /// </summary>
        /// <param name="contentControl">
        /// The content control whose transforms are to be set.
        /// </param>
        /// <param name="final">
        /// The final, top level, transform.
        /// </param>
        /// <param name="angles">
        /// The list of keys to the other transforms.
        /// </param>
        private static void SetElementTransforms(
            DependencyObject contentControl,
            IDictionary<string, Matrix> final,
            IDictionary<string, Point> angles)
        {
            // If we want transform a single element, here's a short circuit
            if (contentControl is Path)
            {
                (contentControl as Path).SetTransform(final[string.Empty]);
                return;
            }

            var canvas = VisualTreeHelper.GetChild(contentControl, 0) as Canvas;
            if (canvas != null)
            {
                foreach (FrameworkElement child in canvas.Children)
                {
                    var key = string.IsNullOrEmpty(child.Name) ? string.Empty : child.Name.Substring(1, 2);
                    if (!final.ContainsKey(key))
                    {
                        continue;
                    }

                    if (child is Path)
                    {
                        (child as Path).SetTransform(final[key]);
                    }
                    else if (child is TextBlock)
                    {
                        SetTextTransform(child, final[key], angles[key]);
                    }
                }
            }
        }

        /// <summary>
        /// If there are only two points, we may need third point to establish a transform.
        /// So pick one perpendicular to the other two, with the same scale.
        /// </summary>
        /// <param name="points">
        /// The points in ControlTemplate space that will be extended with the third point.
        /// </param>
        /// <param name="anchors">
        /// The map locations corresponding to "points" that will be extended with the third point.
        /// </param>
        /// <param name="right">
        /// The boolean that determines whether to use a right-hand or left-hand rule for the third point.
        /// </param>
        private static void ExtendPoints(ref IList<Point> points, ref IList<ILocation> anchors, bool right)
        {
            double angle = right ? -90.0 : 90.0;

            // Extend "points"
            var pt0 = points[0];
            var pt1 = points[1];
            var pt2 = new Point(pt1.X + (pt1.Y - pt0.Y), pt1.Y - (pt1.X - pt0.X));
            points.Add(pt2);

            // Extend "anchors"
            var bearing = MapHelper.CalculateBearing(anchors[0], anchors[1]);
            var range = MapHelper.CalculateRange(anchors[0], anchors[1]);
            var newAnchor = MapHelper.CalculateLocation(anchors[1], bearing + angle, range);
            anchors.Add(newAnchor);
        }

        /// <summary>
        /// Parses the transform string from the ResourceDictionary. The transform string also includes the stencil name.
        /// </summary>
        /// <param name="stencil">
        /// The string to be parsed.
        /// </param>
        /// <param name="stencilType">
        /// The parsed stencil type.
        /// </param>
        /// <returns>
        /// The list of coordinate pairs.
        /// </returns>
        private static IList<Point> ParsePointString(string stencil, ref string stencilType)
        {
            // Read the points as a string
            string data = SymbolData.GetString(stencil + "_1");
            if (data == null)
            {
                return null;
            }

            var parts = data.Split(new[] { ',', ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            stencilType = parts[0];
            var size = (parts.GetLength(0) - 1) / 2;

            // Load up the points
            var points = new List<Point>();
            for (int i = 1; i <= size; i++)
            {
                double value1;
                double value2;
                if (double.TryParse(parts[(2 * i) - 1], out value1) &&
                    double.TryParse(parts[2 * i], out value2))
                {
                    points.Add(new Point(value1, value2));
                }
                else
                {
                    return points;
                }
            }

            return points;
        }

        /// <summary>
        /// Generate the labels if all conditions are favorable.
        /// </summary>
        private void GenerateLabels()
        {
            if (this.suppressRefresh)
            {
                return;
            }

            if (this.SymbolCode == null)
            {
                return;
            }

            if (this.Anchors == null)
            {
                return;
            }

            this.GenerateGraphic();
        }

        /// <summary>
        /// Finds the minimum and maximum latitude and longitude values in the Anchors array.
        /// </summary>
        private void SetLocationRect()
        {
            double minLat = 360.0;
            double maxLat = -360.0;
            double minLon = 360.0;
            double maxLon = -360.0;
            var anchors = this.Anchors;
            foreach (var loc in anchors)
            {
                if (minLat > loc.Latitude)
                {
                    minLat = loc.Latitude;
                }

                if (maxLat < loc.Latitude)
                {
                    maxLat = loc.Latitude;
                }

                if (minLon > loc.Longitude)
                {
                    minLon = loc.Longitude;
                }

                if (maxLon < loc.Longitude)
                {
                    maxLon = loc.Longitude;
                }
            }

            var buffer = BufferScale * Math.Max(maxLat - minLat, maxLon - minLon);
            this.locationRect = milsymFactory.LocationRect(
                milsymFactory.Location(Order.LatLon, minLat - buffer, minLon - buffer),
                milsymFactory.Location(Order.LatLon, maxLat + buffer, maxLon + buffer));
        }

        /// <summary>
        /// This workhorse method is responsible for actually generating the graphic.
        /// A lot of effort centers around transformation generation.
        /// We'll need to generate multiple transformations from the three points we are given.
        /// We need the original 3-point transformation and then squared up transformations based on just
        /// two of the three points (so there are three of those). These transformations can then be
        /// applied to any specified element within the template.
        /// </summary>
        private void GenerateGraphic()
        {
            try
            {
                if (this.suppressRefresh)
                {
                    return;
                }

                string symbolCode = this.SymbolCode;
                if (!SymbolData.Check(ref symbolCode))
                {
                    return;
                }

                IList<ILocation> anchors = this.Anchors;
                if (anchors == null || anchors.Count == 0)
                {
                    return;
                }

                if (this.generatingGraphic)
                {
                    return;
                }

                this.isDirty = true;
                this.generatingGraphic = true;

                // Load the control template
                this.ContentControl = new MilSymbolBase(symbolCode, null, null);

                // Get a "rough" bounding location rectangle
                this.SetLocationRect();

                // Find the "stem" for this symbol
                string stencil = MilSymbolBase.CodeToStencil(symbolCode);

                // Stencil lookup always uses Present
                this.stencilType = string.Empty;
                var points = ParsePointString(stencil, ref this.stencilType);
                if (points == null)
                {
                    this.generatingGraphic = false;
                    return;
                }

                bool right = true;

                ILocation midPoint;
                string[] indices = null;
                switch (this.StencilType)
                {
                    case "SecurityGuard":
                    case "SecurityCover":
                    case "SecurityScreen":
                        if (anchors.Count == 3)    
                        {
                            anchors = SecurityScreenPoints(anchors);    // convert to 4 points if you only have 3
                        }

                        indices = new[] { "12", "34" };
                        break;
                    case "ReliefInPlace":        // uses four points, as given
                        indices = new[] { "12", "34", "32", "21" };
                        break;
                    case "ThreeCross": // these are items that use a third point but not as a point to pass through
                        // Compute the cross product of the anchors to see which template to use
                        var p0 = new Point(
                            anchors[1].Longitude - anchors[0].Longitude,
                            anchors[1].Latitude - anchors[0].Latitude);
                        var p1 = new Point(
                            anchors[1].Longitude - anchors[2].Longitude,
                            anchors[1].Latitude - anchors[2].Latitude);
                        right = ((p0.X * p1.Y) - (p0.Y * p1.X)) >= 0.0;
                        break;
                    case "Contain": // use third point as a length, from midpoint of first two points
                    case "Clear":
                    case "PlannedRoadblock":
                    case "SafeRoadblock":
                    case "ArmedRoadblock":
                    case "AttackByFirePosition":
                    case "Ambush":
                    case "ObstacleBlock":
                    case "BypassEasy":
                    case "BypassDifficult":
                    case "BypassImpossible":
                    case "FordEasy":
                    case "FordDifficult":
                        midPoint = MapHelper.CalculateMidpoint(anchors[0], anchors[1]);
                        anchors[2] = MapHelper.CalculatePerpendicular(midPoint, anchors[1], anchors[2]);
                        anchors.Add(midPoint);
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[0], anchors[1], anchors[2]));
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[1], anchors[0], anchors[2]));
                        indices = new[] { "12", "34", "56" };
                        break;
                    case "TripWire": // use third point as a length, from 1/4 of first two points
                        var qtrPoint = MapHelper.CalculateQuarterpoint(anchors[0], anchors[1]);
                        anchors[2] = MapHelper.CalculatePerpendicular(qtrPoint, anchors[1], anchors[2]);
                        anchors.Add(qtrPoint);
                        var range = MapHelper.CalculateRange(qtrPoint, anchors[2]);
                        var angle = MapHelper.CalculateBearing(anchors[0], anchors[1]);
                        anchors.Add(MapHelper.CalculateLocation(anchors[1], angle, range));
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[4], anchors[1], anchors[2]));
                        anchors[4] = anchors[1];
                        indices = new[] { "12", "34", "56" };
                        break;

                    // case "ThreeProjectMidpointOne": // use third point as a length, from midpoint of first two points
                    //    anchors[0] = MapHelper.CalculatePerpendicular(
                    //        MapHelper.CalculateMidpoint(anchors[1], anchors[2]), anchors[1], anchors[0]);
                    //    break;
                    case "WithdrawUnderPressure":
                    case "Delay":
                    case "Retirement":
                    case "Withdraw":
                        anchors[2] = MapHelper.CalculatePerpendicular(anchors[1], anchors[0], anchors[2]);
                        indices = new[] { "12", "23" };
                        break;
                    case "ObstacleDisrupt":
                        anchors[2] = MapHelper.CalculatePerpendicular(anchors[0], anchors[1], anchors[2]);
                        break;
                    case "Penetrate":
                    case "TaskBlock":
                        midPoint = MapHelper.CalculateMidpoint(anchors[0], anchors[1]);
                        anchors[2] = MapHelper.CalculatePerpendicular(midPoint, anchors[1], anchors[2]);
                        anchors.Add(midPoint);
                        indices = new[] { "12", "43" };
                        break;
                    case "TaskDisrupt":
                        anchors[2] = MapHelper.CalculatePerpendicular(anchors[1], anchors[0], anchors[2]);
                        anchors.Add(MapHelper.CalculateMidpoint(anchors[0], anchors[1]));
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[3], anchors[1], anchors[2]));
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[0], anchors[3], anchors[2]));
                        indices = new[] { "12", "23", "45", "16" };
                        break;
                    case "Infiltration":
                        anchors[2] = MapHelper.CalculatePerpendicular(anchors[1], anchors[0], anchors[2]);
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[0], anchors[1], anchors[2]));
                        anchors.Add(MapHelper.Reflect(anchors[1], anchors[2]));
                        anchors.Add(MapHelper.Reflect(anchors[0], anchors[3]));
                        indices = new[] { "21", "34", "56" };
                        break;
                    case "Bypass":
                    case "Breach":
                    case "Canalize": // use third point as a length, from midpoint of first two points
                        midPoint = MapHelper.CalculateMidpoint(anchors[0], anchors[1]);
                        anchors[2] = MapHelper.CalculatePerpendicular(midPoint, anchors[1], anchors[2]);
                        anchors.Add(midPoint);
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[0], anchors[1], anchors[2]));
                        anchors.Add(MapHelper.CalculatePerpendicular(anchors[1], anchors[0], anchors[2]));
                        indices = new[] { "65", "51", "62" };
                        break;
                    case "PrincipalDirectionOfFire":
                        indices = new[] { "12", "13", "21", "32" };
                        break;
                    case "RoadblockExecuted":
                        midPoint = MapHelper.CalculateMidpoint(anchors[0], anchors[1]);
                        anchors[2] = MapHelper.CalculatePerpendicular(midPoint, anchors[1], anchors[2]);

                        var centerTopRight = milsymFactory.Location(anchors[0]);
                        var centerBottomLeft = milsymFactory.Location(anchors[1]);
                        var crossRange = MapHelper.CalculateRange(midPoint, anchors[2]);
                        var longRange = MapHelper.CalculateRange(midPoint, anchors[0]);
                        var bearing = MapHelper.CalculateBearing(midPoint, anchors[0]);
                        anchors[0] = MapHelper.CalculateLocation(centerTopRight, bearing + 90, crossRange);
                        anchors[1] = MapHelper.CalculateLocation(centerBottomLeft, bearing + 90, crossRange);
                        anchors[2] = MapHelper.CalculateLocation(centerTopRight, bearing - 90, crossRange);
                        anchors.Add(MapHelper.CalculateLocation(centerBottomLeft, bearing - 90, crossRange));

                        var centerTopLeft = MapHelper.CalculateLocation(midPoint, bearing + 60, longRange);
                        var centerBottomRight = MapHelper.CalculateLocation(midPoint, bearing - 120, longRange);
                        anchors.Add(MapHelper.CalculateLocation(centerTopLeft, bearing - 30, crossRange));
                        anchors.Add(MapHelper.CalculateLocation(centerBottomRight, bearing - 30, crossRange));
                        anchors.Add(MapHelper.CalculateLocation(centerTopLeft, bearing + 150, crossRange));
                        anchors.Add(MapHelper.CalculateLocation(centerBottomRight, bearing + 150, crossRange));

                        indices = new[] { "12", "34", "56", "78" };
                        break;
                    case "DropZone":
                    case "GeneralArea":
                    case "AssemblyArea":
                    case "EngagementArea":
                    case "ExtractionZone":
                    case "LandingZone":
                    case "PickupZone":
                    case "AttackPosition":
                    case "Objective":
                    case "AreaOfOperations":
                    case "TargetedAreaOfInterest":
                    case "NamedAreaOfInterest":
                        points = MilZone.AddZone(this, anchors);
                        break;
                    case "WeaponsFreeZone":
                        points = MilZone.AddWeaponsFreeZone(this, anchors);
                        break;
                    case "PentrationBox":
                        points = MilZone.AddZone(this, anchors, new[] { string.Empty });
                        break;
                    case "ObstacleBelt":
                        points = MilZone.AddZone(this, anchors, new[] { this.LabelT, this.LabelT1 });
                        break;
                    case "ObstacleRestrictedArea":
                        points = MilZone.AddZone(this, anchors, new[] { this.LabelT, this.LabelW + "-", this.LabelW1 });
                        break;
                    case "ObstacleFreeArea":
                        points = MilZone.AddZone(this, anchors, new[] { this.LabelT, this.LabelW + "-", this.LabelW1 });
                        break;
                    case "DetaineeHoldingArea":
                    case "EPWHoldingArea":
                    case "ForwardArmingAndRefuelingArea":
                    case "RefugeeHoldingArea":
                    case "BrigadeSupportArea":
                    case "RegimentalSupportArea":
                    case "DivisionSupportArea":
                    case "ObstacleZone":
                        points = MilZone.AddZone(this, anchors, new[] { this.LabelT });
                        break;
                    case "AssaultPosition":
                        points = MilZone.AddZone(this, anchors, new[] { "PSN", this.LabelT });
                        break;
                    case "HighAltitudeMissileEngagementZone":
                    case "LowAltitudeMissileEngagementZone":
                    case "MissileEngagementZone":
                    case "HighDensityAirspaceControlZone":
                    case "ShortRangeAirDefenseEngagementZone":
                    case "RestrictedOperationsZone":
                        points = MilZone.AddAirZone(this, anchors);
                        break;
                    case "ProbableLineOfDeployment":
                        // Force dashed style - hope this works
                        this.ContentControl.NeedDashed = true;
                        points = MilLine.AddPhaseLineType(this, anchors, "PLD");
                        break;
                    case "LineOfDeparture/LineOfContact":
                        points = MilLine.AddPhaseLineType(this, anchors, "LD/LC");
                        break;
                    case "ReleaseLine":
                        points = MilLine.AddPhaseLineType(this, anchors, "RL");
                        break;
                    case "BridgeheadLine":
                        points = MilLine.AddPhaseLineType(this, anchors, "Bridgehead Line");
                        break;
                    case "HoldingLine":
                        points = MilLine.AddPhaseLineType(this, anchors, "Holding Line");
                        break;
                    case "LineOfDeparture":
                        points = MilLine.AddPhaseLineType(this, anchors, "LD");
                        break;
                    case "LimitOfAdvance":
                        points = MilLine.AddPhaseLineType(this, anchors, "LOA");
                        break;
                    case "FinalCoordinationLine":
                        points = MilLine.AddPhaseLineType(this, anchors, "Final CL");
                        break;
                    case "LightLine":
                        points = MilLine.AddPhaseLineType(this, anchors, "LL");
                        break;
                    case "Boundaries":
                        points = MilLine.AddBoundary(this, anchors);
                        break;
                    case "PhaseLine":
                        points = MilLine.AddPhaseLine(this, anchors);
                        break;
                    case "ObstacleLine":
                    case "AntiTankDitchAntiMine":
                    case "AntiTankDitchUnderConstruction":
                    case "AntiTankDitchComplete":
                    case "FortifiedArea":
                        points = MilLine.AddObstacles(this, anchors);
                        break;
                    case "ForwardLineOwnTroops":
                        points = MilLine.AddFlot(this, anchors);
                        break;
                    case "CounterAttackByFire":        // draw some arrows
                    case "CounterAttack":
                    case "AxisOfAdvanceFeint":
                    case "AxisOfAdvanceMainAttack":
                    case "AxisOfAdvanceSupportingAttack":
                    case "AxisOfAdvanceAviation":
                    case "AxisOfAdvanceAirborne":
                    case "AxisOfAdvanceRotaryWing":
                        // We have to copy the anchors since we're going to modify them
                        anchors = new List<ILocation> { this.Anchors[0], this.Anchors[1], this.Anchors[this.Anchors.Count - 1] };
                        anchors[1] = MapHelper.CalculateProjection(anchors[0], anchors[1], anchors[2]);
                        points = new List<Point> { points[0], points[1], points[points.Count - 1] };

                        // Using points[0] for the origin, which is the tip of the arrow
                        // and not part of the arrow body makes the code messier since 
                        // we have to transform this point all the way through but make 
                        // sure we don't ever actually include it in the arrow body.
                        MilArrow.AddArrow(this, points, anchors);
                        indices = new[] { "12", "13" };
                        break;
                }

                // Need a third anchor if we only have two.
                // Third anchor should generate perpendicular and be the same length.
                if (points.Count == 2)
                {
                    if (points.Count < anchors.Count)
                    {
                        var saveAnchors = this.Anchors;
                        anchors = new List<ILocation>
                        {
                            milsymFactory.Location(saveAnchors[0]), 
                            milsymFactory.Location(saveAnchors[1])
                        };
                    }

                    ExtendPoints(ref points, ref anchors, right);
                }

                this.origin = FullTransformation(
                    this.ContentControl,
                    indices,
                    points,
                    anchors,
                    points[0],
                    out this.scaleFactor);

                // Create a placeholder RenderTransformation.
                //
                // This is the global scaled transform to apply to all elements, some of which are
                // also scaled, rotated, and translated locally.
                this.scaleTransform = new ScaleTransform { ScaleX = 0.01, ScaleY = 0.01 };
                RenderTransform = new TransformGroup { Children = { this.scaleTransform } };

                // Add the content control to the canvas
                this.AddChild("Base", this.ContentControl);
                this.LayoutUpdated += this.CanvasLayoutUpdated;
                this.generatingGraphic = false;
            }
            catch (Exception ex)
            {
                this.generatingGraphic = false;
                Log.WriteMessage(LogLevel.Error, "Unable to generate military graphic", ex);
            }
        }

        /// <summary>
        /// Updates the size of the object when requested.
        /// </summary>
        /// <param name="sender">
        /// This parameter is ignored.
        /// </param>
        /// <param name="e">
        /// This parameter is also ignored.
        /// </param>
        private void CanvasLayoutUpdated(object sender, EventArgs e)
        {
            if (this.Layer == null)
            {
                return;
            }

            if (!this.Layer.CanBeSeen)
            {
                return;
            }

            // If graphic is not visible (roughly speaking since these bounds are not precise), then return
            var extent = this.Layer.MapExtent;
            if (extent == null)
            {
                return;
            }

            if (!extent.Intersects(this.locationRect))
            {
                return;
            }

            // Get the zoom so we can compute the map scale
            double zoom = this.Layer.ZoomLevel;
            if (!this.isDirty && this.lastZoomLevel == zoom) 
            {
                return;     // don't recompute if we have the same zoom value
            }

            this.isDirty = false;
            this.lastZoomLevel = zoom;

            // The ScaleFactor was computed in ComputeMapScales
            var scale = this.scaleFactor * MapHelper.MapSize(zoom) / ConvenienceScaleFactor;

            // Need to scale the line thickness
            var contentControl = Children[0] as MilSymbolBase;
            if (contentControl != null)
            {
                contentControl.LineThickness = 3.0 / scale;     // 3.0 is arbitrary and should be further investigated
            }

            // Need to change the scale transformation
            this.scaleTransform.ScaleX = this.scaleTransform.ScaleY = scale;
        }
    }
}
