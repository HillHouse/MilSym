// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="CombinedModifierCode.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the combined modifier code portion of symbols in MIL STD-2525C
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
    /// <summary>
    /// This is an artificial class to provide a useful interface
    /// for a combined friendly name for a two character modifier code. Because the
    /// codes are so convoluted, the actual program logic uses the
    /// ModifierCode, Mobility, and Echelon classes instead
    /// </summary>
    public static class CombinedModifierCode
    {
        /// <summary>
        /// The staring symbol index for the combined (2-character) modifier code
        /// </summary>
        private const int Index = 10;

        /// <summary>
        /// Get the two character string that is the combined modifier code
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the combined two character modifier code</returns>
        public static string GetCode(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            return symbolCode.Substring(Index, 2);
        }

        /// <summary>
        /// Get the combined modifier code for the coding scheme.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the combined modifier code</returns>
        public static string GetName(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return string.Empty;
            }

            char leadCode = ModifierCode.GetCode(symbolCode);
            char trailCode = Echelon.GetCode(symbolCode);
            if (leadCode == ModifierCode.Mobility || leadCode == ModifierCode.Towed)
            {
                return Mobility.GetName(symbolCode);
            }

            if (leadCode == ModifierCode.Installation)
            {
                return (trailCode == 'B') ? "Feint Dummy Installation" : "Installation";
            }

            string mod = ModifierCode.GetName(symbolCode);
            string ech = Echelon.GetName(symbolCode);
            if (string.IsNullOrEmpty(mod) || string.IsNullOrEmpty(ech))
            {
                return string.Empty;
            }

            if (mod == "None" && ech == "None")
            {
                return string.Empty;
            }

            if (mod == "None")
            {
                return ech;
            }

            if (ech == "None")
            {
                return mod;
            }

            return mod + "\n" + ech;
        }
    }
}