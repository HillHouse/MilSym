// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ExtensionMethodsSL.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Silverlight specific extension methods for Microsoft classes.
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// ExtensionMethods is a static class with some common extension methods for all components.
    /// All components depend on MilSym.MilSymbol. This portion of the class is Silverlight dependent.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// The Silverlight version simply copies the TextBlock's ActualWidth/Height to Width/Height
        /// so they can be used for simple bounding box computations.
        /// </summary>
        /// <param name="tb">
        /// The TextBlock for which to copy the width and height.
        /// </param>
        public static void FindTextExtent(this TextBlock tb)
        {
            if (tb == null)     
            {
                return; // this should never happen
            }

            tb.Width = tb.ActualWidth;
            tb.Height = tb.ActualHeight;
        }

        /// <summary>
        /// Sets the dashed line parameters for a given style.
        /// </summary>
        /// <param name="st">
        /// The style for which the dashed line parameters are to be set.
        /// </param>
        /// <param name="dashes">
        /// A list of dash lengths as integers.
        /// </param>
        public static void SetDashArray(this Style st, params int[] dashes)
        {
            var s = string.Empty;
            s = dashes.Aggregate(s, (current, d) => current + (d + " "));
            st.Setters.Add(new Setter(Shape.StrokeDashArrayProperty, s));
        }

        /// <summary>
        /// Sets the matrix transform for the PathGeometry along a Path 
        /// </summary>
        /// <param name="path">
        /// The path for which to set the PathGeometry's matrix.
        /// </param>
        /// <param name="matrix">
        /// The transform matrix to set.
        /// </param>
        public static void SetTransform(this Path path, Matrix matrix)
        {
            var pg = path.Data as PathGeometry;
            if (pg == null)
            {
                return;
            }

            if (pg.Transform == null)
            {
                pg.Transform = new MatrixTransform { Matrix = matrix };
            }
        }
    }
}
