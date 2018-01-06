// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="OrderOfBattle.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Order of Battle code portion of symbols in MIL STD-2525C
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
    /// Support methods for managing the Order of Battle code portion of symbols in MIL STD-2525C
    /// </summary>
    public static class OrderOfBattle
    {
        ////    A - AIR 
        ////    E - ELECTRONIC 
        ////    C - CIVILIAN 
        ////    G - GROUND 
        ////    N - MARITIME 
        ////    S - STRATEGIC FORCE RELATED

        /// <summary>
        /// The symbol's Order of Battle code for Air
        /// </summary>
        public const char Air = 'A';

        /// <summary>
        /// The symbol's Order of Battle code for Civilian
        /// </summary>
        public const char Civilian = 'C';

        /// <summary>
        /// The symbol's Order of Battle code for Electronic
        /// </summary>
        public const char Electronic = 'E';

        /// <summary>
        /// The symbol's Order of Battle code for Ground
        /// </summary>
        public const char Ground = 'G';

        /// <summary>
        /// The symbol's Order of Battle code for Maritime
        /// </summary>
        public const char Maritime = 'N';

        /// <summary>
        /// The symbol's Order of Battle code for Strategic force related
        /// </summary>
        public const char StrategicForceRelated = 'S';

        /// <summary>
        /// The symbol code index for the order of battle.
        /// </summary>
        private const int Index = 14;

        /// <summary>
        /// Dictionary mapping Order of Battle symbol code values to friendly names
        /// </summary>
        private static readonly Dictionary<char, string> Names = new Dictionary<char, string>
         {
             { Air, "Air" },
             { Electronic, "Electronic" },
             { Civilian, "Civilian" },
             { Ground, "Ground" },
             { Maritime, "Maritime" },
             { StrategicForceRelated, "StrategicForceRelated" }
         };

        /// <summary>
        /// Get the symbol code's order of battle.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the order of battle code</returns>
        public static char GetCode(string symbolCode)
        {
            return !SymbolData.Check(ref symbolCode) ? (char)0 : symbolCode[Index];
        }

        /// <summary>
        /// Get the order of battle's friendly name for the given symbol code.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the order of battle</returns>
        public static string GetName(string symbolCode)
        {
            char key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }
    }
}