// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ExtensionMethodsWpf.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   WPF specific extension methods.
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
    using System.Globalization;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;
#endif

    /// <summary>
    /// WPF specific extensions.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Use the current culture.
        /// </summary>
        private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;

        /// <summary>
        /// The default black brush.
        /// </summary>
        private static readonly Brush Black = MilBrush.Black;

        /// <summary>
        /// Finds the text extent. This is a lot messier than the Silverlight version.
        /// But with WPF we could actually bind the Width/Height properties
        /// and auto-adjust according to the content - maybe sometime.
        /// </summary>
        /// <param name="tb">
        /// The TextBlock for which to find the extent.
        /// </param>
        public static void FindTextExtent(this TextBlock tb)
        {
            if (tb == null)     
            {
                return;     // this should never happen
            }
#if WINDOWS_UWP // similar to Silverlight
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tb.Arrange(new Rect(new Point(0.0, 0.0), tb.DesiredSize));

            tb.Width = tb.ActualWidth;
            tb.Height = tb.ActualHeight;
#else
            var tf = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.Width = 0.0;
                var lineCount = 1;
                foreach (var inl in tb.Inlines)
                {
                    if (inl is LineBreak)
                    {
                        lineCount++;
                        continue;
                    }

                    var ft = new FormattedText(((Run)inl).Text, Culture, tb.FlowDirection, tf, tb.FontSize, Black);
                    if (tb.Width < ft.Width)
                    {
                        tb.Width = ft.Width;
                    }
                }

                if (double.IsNaN(tb.LineHeight))
                {
                    tb.LineHeight = 1.15 * tb.FontSize;
                }

                tb.Height = lineCount * tb.LineHeight;
            }
            else
            {
                var ft = new FormattedText(tb.Text, Culture, tb.FlowDirection, tf, tb.FontSize, Black);
                tb.Width = ft.Width;
                tb.Height = ft.Height;
            }
#endif
        }

        /// <summary>
        /// Set the dash array for WPF.
        /// </summary>
        /// <param name="st">
        /// The style for which to set the dash array.
        /// </param>
        /// <param name="dashes">
        /// A series of integers to set for the dash array.
        /// </param>
        public static void SetDashArray(this Style st, params int[] dashes)
        {
            var dc = new DoubleCollection();
            foreach (var d in dashes)
            {
                dc.Add(d);
            }

            st.Setters.Add(new Setter(Shape.StrokeDashArrayProperty, dc));
        }

        /// <summary>
        /// Set the transform matrix for a given path.
        /// </summary>
        /// <param name="path">
        /// The path on which to set the transform.
        /// </param>
        /// <param name="matrix">
        /// The transform matrix to set for the path.
        /// </param>
        public static void SetTransform(this Path path, Matrix matrix)
        {
            var pg = path.Data as PathGeometry;
            if (pg == null)
            {
                return;
            }

#if WINDOWS_UWP // same as Silverlight
            if (pg.Transform == null)
            {
                pg.Transform = new MatrixTransform { Matrix = matrix };
            }
#else
            // "pg" may be frozen in WPF which is a bad thing, so clone and own
            if (pg.Transform == null ||
               (pg.Transform is MatrixTransform &&
                    ((MatrixTransform)pg.Transform).Matrix.IsIdentity))
            {
                if (pg.IsFrozen)
                {
                    pg = pg.Clone();
                }                pg.Transform = new MatrixTransform(matrix);
                path.Data = pg;
            }
#endif
        }
    }
}
