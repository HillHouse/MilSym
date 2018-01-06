// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ExtensionMethods.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Extension methods to extend specific Microsoft classes
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
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// ExtensionMethods is a static class with some common extension methods for all components.
    /// All components depend on MilSym.MilSymbol.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Applies only the rotation portion of a transform matrix.
        /// </summary>
        /// <param name="m">The matrix that contains the rotation transform.</param>
        /// <param name="p">The point to be rotated.</param>
        /// <returns>The rotated point.</returns>
        public static Point Rotate(this Matrix m, Point p)
        {
            return new Point((m.M11 * p.X) + (m.M12 * p.Y), (m.M21 * p.X) + (m.M22 * p.Y));
        }

        /// <summary>
        /// Returns all the visuals in the tree branch, one at a time.
        /// </summary>
        /// <param name="root">
        /// The root of the tree branch as a dependency object.
        /// </param>
        /// <returns>
        /// Each visual in the tree branch, one at a time.
        /// </returns>
        public static IEnumerable<DependencyObject> GetVisuals(this DependencyObject root)
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendants in child.GetVisuals())
                {
                    yield return descendants;
                }
            }
        }
    }
}
