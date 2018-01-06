// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Category.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Category portion of symbols in MIL STD-2525C
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
    /// Support methods for managing the Category code portion of symbols in MIL STD-2525C
    /// </summary>
    public static class Category
    {
        // Index is 2 for tactical graphics (G)
        // T - TASKS
        // G - C2 & GENERAL MANEUVER
        // M - MOBILITY/SURVIVABILITY
        // F - FIRE SUPPORT
        // S - COMBAT SERVICE SUPPORT
        // O - OTHER

        // Index is 1 for METOC (W)
        // A - Atmospheric
        // O - Oceanic
        // S - Space

        // Index is 2 for stability operations (O)
        // V - VIOLENT ACTIVITIES
        // L - LOCATIONS
        // O - OPERATIONS
        // I - ITEMS
        // P - INDIVIDUAL
        // G - NONMILITARY GROUP OR ORGANIZATION
        // R - RAPE

        // Index is 2 for emergency management (E)
        // I - INCIDENT
        // N - NATURAL EVENTS
        // O - OPERATIONS
        // F - INFRASTRUCTURE

        /// <summary>
        /// The index of the first character in the Modifier code
        /// </summary>
        public const int Index = 2;

        /// <summary>
        /// The character value for the category code Dummy
        /// </summary>
        public const char FeintDummy = 'F';

        /// <summary>
        /// The character value for the category code Feint Dummy Headquarters
        /// </summary>
        public const char FeintDummyHeadquarters = 'C';

        /// <summary>
        /// The character value for the category code Feint Dummy/Task Force
        /// </summary>
        public const char FeintDummyTaskForce = 'G';

        /// <summary>
        /// The character value for the category code Feint Dummy/Task Force Headquarters
        /// </summary>
        public const char FeintDummyTaskForceHeadquarters = 'D';

        /// <summary>
        /// The character value for the category code Headquarters
        /// </summary>
        public const char Headquarters = 'A';

        /// <summary>
        /// The character value for the category code Installation
        /// </summary>
        public const char Installation = 'H';

        /// <summary>
        /// The character value for the category code Mobility
        /// </summary>
        public const char Mobility = 'M';

        /// <summary>
        /// Empty character for modifier code
        /// </summary>
        public const char None = '-';

        /// <summary>
        /// The character value for the category code Task Force
        /// </summary>
        public const char TaskForce = 'E';

        /// <summary>
        /// The character value for the category code Task Force Headquarters
        /// </summary>
        public const char TaskForceHeadquarters = 'B';

        /// <summary>
        /// The character value for the category code Towed array
        /// </summary>
        public const char Towed = 'N';

        /// <summary>
        /// A mapping of the character codes to more meaningful strings
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