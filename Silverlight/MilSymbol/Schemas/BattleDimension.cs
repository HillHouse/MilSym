// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="BattleDimension.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the battle dimension code portion of symbols in MIL STD-2525C.
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
    /// Support methods for managing the battle dimension code portion of symbols in MIL STD-2525C.
    /// </summary>
    public static class BattleDimension
    {
        ////    P - SPACE                        blacken the upper portion of the air symbol
        ////    A - AIR                            
        ////    G - GROUND                        
        ////    S - SEA SURFACE                    
        ////    U - SEA SUBSURFACE                
        ////    F - SOF                            
        ////    X - OTHER (No frame)            
        ////    Z - UNKNOWN                        display large black question mark for interior marking

        /// <summary>
        /// The symbol code index for the battle dimension
        /// </summary>
        public const int Index = 2;

        /// <summary>
        /// Air character.
        /// </summary>
        public const char Air = 'A';

        /// <summary>
        /// Ground character.
        /// </summary>
        public const char Ground = 'G';

        /// <summary>
        /// Other character.
        /// </summary>
        public const char Other = 'X';

        /// <summary>
        /// Sea surface character.
        /// </summary>
        public const char SeaSurface = 'S';

        /// <summary>
        /// Space character.
        /// </summary>
        public const char Space = 'P';

        /// <summary>
        /// Special operations force character.
        /// </summary>
        public const char SpecialOperationsForce = 'F';

        /// <summary>
        /// Subsurface character.
        /// </summary>
        public const char Subsurface = 'U';

        /// <summary>
        /// Unknown character.
        /// </summary>
        public const char Unknown = 'Z';

        /// <summary>
        /// Dictionary mapping of character values to friendly names.
        /// </summary>
        private static readonly Dictionary<char, string> Names = new Dictionary<char, string>
        {
            { Space, "Space" },
            { Air, "Air" },
            { Ground, "Ground" },
            { SeaSurface, "Sea Surface" },
            { Subsurface, "Sea Subsurface" },
            { SpecialOperationsForce, "SOF" },
            { Other, "Other (No Frame)" },
            { Unknown, "Unknown" },
        };

        /// <summary>
        /// Get the battle dimension code from the symbol code.
        /// </summary>
        /// <param name="symbolCode">The symbol code.</param>
        /// <returns>The battle dimension code.</returns>
        public static char GetCode(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return (char)0;
            }

            return symbolCode[Index];
        }

        /// <summary>
        /// Get a friendly name for the battle dimension.
        /// </summary>
        /// <param name="symbolCode">The symbol code.</param>
        /// <returns>A friendly name for the battle dimension.</returns>
        public static string GetName(string symbolCode)
        {
            char key = GetCode(symbolCode);
            if (Names.ContainsKey(key))
            {
                return Names[key];
            }

            return string.Empty;
        }
    }
}