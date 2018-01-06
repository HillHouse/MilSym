// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="CodingScheme.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Coding Scheme portion of symbols in MIL STD-2525C
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
    using System.Collections.Generic;

    /// <summary>
    /// Support methods for managing the Coding Scheme portion of symbols in MIL STD-2525C
    /// </summary>
    public static class CodingScheme
    {
        ////    S - warfighting
        ////    G - tactical graphics
        ////    W - METOC
        ////    I -    intelligence
        ////    O - stability operations (SO)
        ////    E - emergency management

        /// <summary>
        /// Unique value for mapping Coding Scheme representing Emergency management
        /// </summary>
        public const int EmergencyManagement = 0x1000;
        
        /// <summary>
        /// Unique value for mapping Coding Scheme representing Intelligence
        /// </summary>
        public const int Intelligence = 0x2000;
        
        /// <summary>
        /// Unique value for mapping Coding Scheme representing Stability operations 
        /// </summary>
        public const int StabilityOperations = 0x3000;
        
        /// <summary>
        /// Unique value for mapping Coding Scheme representing Tactical graphics
        /// </summary>
        public const int TacticalGraphics = 0x4000;
        
        /// <summary>
        /// Unique value for mapping Coding Scheme representing Warfighting
        /// </summary>
        public const int Warfighting = 0x5000;
        
        /// <summary>
        /// Unique value for mapping Coding Scheme representing Weather, also METOC
        /// </summary>
        public const int Weather = 0x6000;

        /// <summary>
        /// The symbol code index for the coding scheme
        /// </summary>
        private const int Index = 0;

        /// <summary>
        /// Dictionary mapping the coding scheme to unique numeric values
        /// </summary>
        private static readonly IDictionary<char, int> Css = new Dictionary<char, int>
        {
            { 'S', Warfighting },
            { 'G', TacticalGraphics },
            { 'W', Weather },
            { 'I', Intelligence },
            { 'O', StabilityOperations },
            { 'E', EmergencyManagement }
        };

        /// <summary>
        /// Dictionary for mapping unique coding scheme numeric values to friendly strings
        /// </summary>
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { Warfighting, "Warfighting" },
            { TacticalGraphics, "Tactical Graphics" },
            { Weather, "Weather (METOC)" },
            { Intelligence, "Intelligence" },
            { StabilityOperations, "Stability Operations" },
            { EmergencyManagement, "Emergency Management" }
        };

        /// <summary>
        /// Get the symbol code's character for the coding scheme
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the coding scheme for the symbol code</returns>
        public static int GetCode(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return 0;
            }

            return Css.ContainsKey(symbolCode[Index]) ? Css[symbolCode[Index]] : 0;
        }

        /// <summary>
        /// Get a friendly name for the coding scheme.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the coding scheme</returns>
        public static string GetName(string symbolCode)
        {
            int key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }
    }
}