// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ISymbol.cs">
//   Copyright © 2009-2012 HillHouse
// </copyright>
// <summary>
//   A interface for a sample business object.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace BindingTest
{
    using System.Windows;

    /// <summary>
    /// Pretend this is some sort of business object interface
    /// </summary>
    public interface ISymbol
    {
        /// <summary>
        /// Gets or sets the name associated with the symbol.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the symbol code.
        /// </summary>
        string SymbolCode { get; set; }

        /// <summary>
        /// Gets or sets the symbol's position.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Gets or sets the symbols display angle in degrees.
        /// </summary>
        double AngleDegrees { get; set; }
    }
}