// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Offset.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A simple class to hold an x/y offset for label placement.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilGraph.Support
{
    /// <summary>
    /// A simple class to hold an x/y offset for label placement.
    /// </summary>
    public class Offset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Offset"/> class.
        /// </summary>
        /// <param name="x">
        /// The x offset coordinate.
        /// </param>
        /// <param name="y">
        /// The y offset coordinate.
        /// </param>
        public Offset(double x = 0.0, double y = 0.0)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the X offset coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets Y offset coordinate.
        /// </summary>
        public double Y { get; set; }
    }
}
