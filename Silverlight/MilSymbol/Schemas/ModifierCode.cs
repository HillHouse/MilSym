// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ModifierCode.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Modifier code portion of symbols in MIL STD-2525C
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
    /// Support methods for managing the Modifier code portion of symbols in MIL STD-2525C
    /// </summary>
    public static class ModifierCode
    {
        ////    - -    none
        ////    A -    Headquarters (Hq)
        ////    B - Task Force & Headquarters (TfHq)
        ////    C - Feint Dummy & Headquarters (FdHq)
        ////    D -    Feint Dummy, Task Force, & Headquarters (FdTfHq)
        ////    E - Task Force (Tf)
        ////    F - Feint Dummy (Fd)
        ////    G - Feint Dummy & Task Force (FdTf)
        ////    H - Installation
        ////    M - Mobility
        ////    N - towed

        /// <summary>
        /// The symbol's modifier code for Feint Dummy
        /// </summary>
        public const char FeintDummy = 'F';

        /// <summary>
        /// The symbol's modifier code for Feint Dummy Headquarters
        /// </summary>
        public const char FeintDummyHeadquarters = 'C';

        /// <summary>
        /// The symbol's modifier code for Feint Dummy/Task Force
        /// </summary>
        public const char FeintDummyTaskForce = 'G';

        /// <summary>
        /// The symbol's modifier code for Feint Dummy/Task Force Headquarters
        /// </summary>
        public const char FeintDummyTaskForceHeadquarters = 'D';

        /// <summary>
        /// The symbol's modifier code for Headquarters
        /// </summary>
        public const char Headquarters = 'A';

        /// <summary>
        /// The symbol's modifier code for Installation
        /// </summary>
        public const char Installation = 'H';

        /// <summary>
        /// The symbol's modifier code for Mobility
        /// </summary>
        public const char Mobility = 'M';

        /// <summary>
        /// The symbol's modifier code for Empty 
        /// </summary>
        public const char None = '-';

        /// <summary>
        /// The symbol's modifier code for Task Force
        /// </summary>
        public const char TaskForce = 'E';

        /// <summary>
        /// The symbol's modifier code for Task Force Headquarters
        /// </summary>
        public const char TaskForceHeadquarters = 'B';

        /// <summary>
        /// The symbol's modifier code for Towed array
        /// </summary>
        public const char Towed = 'N';

        /// <summary>
        /// The index of the first character in the Modifier code
        /// </summary>
        private const int Index = 10;

        /// <summary>
        /// Dictionary mapping modifier codes to friendly names
        /// </summary>
        private static readonly Dictionary<char, string> Names = new Dictionary<char, string>
        {
             { None, "None" },
             { Headquarters, "Headquarters" },
             { TaskForceHeadquarters, "Task Force & Headquarters" },
             { FeintDummyHeadquarters, "Feint Dummy & Headquarters" },
             { FeintDummyTaskForceHeadquarters, "Feint Dummy, Task Force, & Headquarters" },
             { TaskForce, "Task Force" },
             { FeintDummy, "Feint Dummy" },
             { FeintDummyTaskForce, "Feint Dummy & Task Force" },
             { Installation, "Installation" },
             { Mobility, "Mobility" },
             { Towed, "Towed" }
        };

        /// <summary>
        /// Get the first character of the Modifier code 
        /// Use CombinedModifierCode.GetCode to get both characters.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the first character of the Modifier code</returns>
        public static char GetCode(string symbolCode)
        {
            return !SymbolData.Check(ref symbolCode) ? (char)0 : symbolCode[Index];
        }

        /// <summary>
        /// Get the friendly name for the first character of the modifier code.
        /// Use CombinedModifierCode.GetName to get the friendly name for both characters.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the first character of the modifier code</returns>
        public static string GetName(string symbolCode)
        {
            char key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }
    }
}