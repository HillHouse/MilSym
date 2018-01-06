// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Mobility.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Mobility code portion of symbols in MIL STD-2525C
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
    /// Support methods for managing the Mobility code portion of symbols in MIL STD-2525C
    /// </summary>
    public static class Mobility
    {
        ////    MO - WHEELED (LIMITED CROSSCOUNTRY)
        ////    MP - WHEELED (CROSSCOUNTRY)
        ////    MQ - TRACKED
        ////    MR - WHEELED AND TRACKED COMBINATION
        ////    MS - TOWED
        ////    MT - RAILWAY
        ////    MU - OVER-SNOW (PRIME MOVER)
        ////    MV - SLED
        ////    MW - PACK ANIMALS
        ////    MX - BARGE
        ////    MY - AMPHIBIOUS
        ////    NS - TOWED SONAR ARRAY (SHORT)
        ////    NL - TOWED SONAR ARRAY (LONG)

        /// <summary>
        /// The Mobility symbol code for Amphibious
        /// </summary>
        public const string Amphibious = "MY";

        /// <summary>
        /// The Mobility symbol code for Barge
        /// </summary>
        public const string Barge = "MX";

        /// <summary>
        /// The Mobility symbol code for Over snow
        /// </summary>
        public const string OverSnow = "MU";

        /// <summary>
        /// The Mobility symbol code for Pack animals
        /// </summary>
        public const string PackAnimals = "MW";

        /// <summary>
        /// The Mobility symbol code for Railway
        /// </summary>
        public const string Railway = "MT";

        /// <summary>
        /// The Mobility symbol code for Sled
        /// </summary>
        public const string Sled = "MV";

        /// <summary>
        /// The Mobility symbol code for Towed
        /// </summary>
        public const string Towed = "MS";

        /// <summary>
        /// The Mobility symbol code for Towed array (long)
        /// </summary>
        public const string TowedArrayLong = "NL";

        /// <summary>
        /// The Mobility symbol code for Towed array (short)
        /// </summary>
        public const string TowedArrayShort = "NS";

        /// <summary>
        /// The Mobility symbol code for Tracked
        /// </summary>
        public const string Tracked = "MQ";

        /// <summary>
        /// The Mobility symbol code for Wheeled - cross country
        /// </summary>
        public const string WheeledCrossCountry = "MP";

        /// <summary>
        /// The Mobility symbol code for Wheeled - limited
        /// </summary>
        public const string WheeledLimited = "MO";

        /// <summary>
        /// The Mobility symbol code for Wheeled and tracked
        /// </summary>
        public const string WheeledTracked = "MR";

        /// <summary>
        /// The symbol code's starting index for the two character mobility code.
        /// Mobility is a special case of the modifier code.
        /// </summary>
        private const int Index = 10;

        /// <summary>
        ///  Dictionary to map Mobility symbol codes to friendly names
        /// </summary>
        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>
           {
               { WheeledLimited, "Wheeled (Limited Cross-country)" },
               { WheeledCrossCountry, "Wheeled (Cross-country)" },
               { Tracked, "Tracked" },
               { WheeledTracked, "Wheeled & Tracked Combination" },
               { Towed, "Towed" },
               { Railway, "Railway" },
               { OverSnow, "Over-Snow (Prime Mover)" },
               { Sled, "Sled" },
               { PackAnimals, "Pack Animals" },
               { Barge, "Barge" },
               { Amphibious, "Amphibious" },
               { TowedArrayShort, "Towed Sonar Array (Short)" },
               { TowedArrayLong, "Towed Sonar Array (Long)" }
           };

        /// <summary>
        /// Get the symbol code's two character mobility code.
        /// Mobility is a special case of the modifier code.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the symbol code's two character mobility code</returns>
        public static string GetCode(string symbolCode)
        {
            return !SymbolData.Check(ref symbolCode) ? string.Empty : symbolCode.Substring(Index, 2);
        }

        /// <summary>
        /// Get the mobility code's friendly name for the symbol code.
        /// Mobility is a special case of the modifier code.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the mobility code</returns>
        public static string GetName(string symbolCode)
        {
            string key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }

        /// <summary>
        /// Determines whether a given symbol code has a non-empty mobility component
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code to check for mobility.
        /// </param>
        /// <returns>
        /// True is the symbol code contains a mobility component.
        /// </returns>
        public static bool IsMobility(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return false;
            }

            return symbolCode[Index] == 'M' || symbolCode[Index] == 'N';
        }
    }
}