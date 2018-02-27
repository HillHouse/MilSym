// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Scaling.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Scaling is an internal class that defines scaling data for single point milsymbols.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilSymbol.Schemas
{
#if WINDOWS_UWP
    using Windows.Foundation;
#else
    using System.Windows;
#endif

    /// <summary>
    /// A scaling class that contains a bounding rectangle and some scaling factors
    /// </summary>
    public class Scaling
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scaling"/> class.
        /// </summary>
        /// <param name="r">
        /// The bounding rectangle.
        /// </param>
        /// <param name="af">
        /// The area scaling factor.
        /// </param>
        /// <param name="lf">
        /// The linear scaling factor.
        /// </param>
        public Scaling(Rect r, double af, double lf)
        {
            this.Bounds = r;
            this.AreaFactor = af;
            this.LinearFactor = lf;
        }

        /// <summary>
        /// Gets or sets the bounding rectangle.
        /// </summary>
        public Rect Bounds { get; set; }

        /// <summary>
        /// Gets or sets the area scaling factor.
        /// </summary>
        public double AreaFactor { get; set; }

        /// <summary>
        /// Gets or sets the linear scaling factor.
        /// </summary>
        public double LinearFactor { get; set; }
    }
}
