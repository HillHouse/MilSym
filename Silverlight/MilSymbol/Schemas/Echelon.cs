// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Echelon.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Echelon portion of symbols in MIL STD-2525C
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
#if !SILVERLIGHT
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Support methods for managing the Echelon portion of symbols in MIL STD-2525C
    /// </summary>
    public static class Echelon
    {
        //// Echelon data
        ////    Ø        A    TEAM/CREW
        ////    •        B    SQUAD 
        ////    ••       C    SECTION
        ////    •••      D    PLATOON/DETACHMENT 
        ////    |        E    COMPANY/BATTERY/TROOP
        ////    ||       F    BATTALION/SQUADRON 
        ////    |||      G    REGIMENT/GROUP
        ////    X        H    BRIGADE 
        ////    XX       I    DIVISION
        ////    XXX      J    CORPS/MEF 
        ////    XXXX     K    ARMY
        ////    XXXXX    L    ARMY GROUP/FRONT 
        ////    XXXXXX   M    REGION
        ////    ++       N    COMMAND

        /// <summary>
        /// The character echelon code for Army
        /// </summary>
        public const char Army = 'K';

        /// <summary>
        /// The character echelon code for Army/Group/Front
        /// </summary>
        public const char ArmyGroupFront = 'L';

        /// <summary>
        /// The character echelon code for Battalion/Squadron
        /// </summary>
        public const char BattalionSquadron = 'F';

        /// <summary>
        /// The character echelon code for Brigade
        /// </summary>
        public const char Brigade = 'H';

        /// <summary>
        /// The character echelon code for Command
        /// </summary>
        public const char Command = 'N';

        /// <summary>
        /// The character echelon code for Company/Battery/Troop
        /// </summary>
        public const char CompanyBatteryTroop = 'E';

        /// <summary>
        /// The character echelon code for Corps/Marine Expeditionary Force (MEF)
        /// </summary>
        public const char CorpsMarineExpeditionaryForce = 'J';

        /// <summary>
        /// The character echelon code for Division
        /// </summary>
        public const char Division = 'I';

        /// <summary>
        /// The character echelon code for Null - there is no echelon code
        /// </summary>
        public const char None = '-';

        /// <summary>
        /// The character echelon code for Platoon/Detachment
        /// </summary>
        public const char PlatoonDetachment = 'D';

        /// <summary>
        /// The character echelon code for Regiment/Group
        /// </summary>
        public const char RegimentGroup = 'G';

        /// <summary>
        /// The character echelon code for Region
        /// </summary>
        public const char Region = 'M';

        /// <summary>
        /// The character echelon code for Section
        /// </summary>
        public const char Section = 'C';

        /// <summary>
        /// The character echelon code for Squad
        /// </summary>
        public const char Squad = 'B';

        /// <summary>
        /// The character echelon code for Team/Crew
        /// </summary>
        public const char TeamCrew = 'A';

        /// <summary>
        /// The symbol code index corresponding to the echelon code. 
        /// </summary>
        private const int Index = 11;

        /// <summary>
        /// Dictionary mapping the echelon symbol codes to strings to represent echelon value graphically
        /// </summary>
        private static readonly IDictionary<char, string> Echelons = new Dictionary<char, string>
        {
             { None, string.Empty },
             { TeamCrew, "Ø" },                 // 00D8 - latin captital O with stroke
             { Squad, "●" },                    // 25CF - black circle in Arial
             { Section, "●●" },                 // 25CF - black circle in Arial
             { PlatoonDetachment, "●●●" },      // 25CF - black circle in Arial
             { CompanyBatteryTroop, "ı" },      // 0131 - small latin dotless i
             { BattalionSquadron, "ıı" },       // 0131 - small latin dotless i
             { RegimentGroup, "ııı" },          // 0131 - small latin dotless i
             { Brigade, "x" },                  // small x
             { Division, "xx" },
             { CorpsMarineExpeditionaryForce, "xxx" },
             { Army, "xxxx" },
             { ArmyGroupFront, "xxxxx" },
             { Region, "xxxxxx" },
             { Command, "++" }                  // plus
         };

        /// <summary>
        /// Dictionary mapping the echelon symbol codes to friendly string descriptions
        /// </summary>
        private static readonly Dictionary<char, string> Names = new Dictionary<char, string>
         {
             { None, "None" },
             { TeamCrew, "Team/Crew" },
             { Squad, "Squad" },
             { Section, "Section" },
             { PlatoonDetachment, "Platoon/Detachment" },
             { CompanyBatteryTroop, "Company/Battery/Troop" },
             { BattalionSquadron, "Battalion/Squadron" },
             { RegimentGroup, "Regiment/Group" },
             { Brigade, "Brigade" },
             { Division, "Division" },
             { CorpsMarineExpeditionaryForce, "Corps/MEF" },
             { Army, "Army" },
             { ArmyGroupFront, "Army Group/Front" },
             { Region, "Region" },
             { Command, "Command" }
         };

        /// <summary>
        /// Get the echelon code for the given symbol code.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the echelon code for the given symbol code</returns>
        public static char GetCode(string symbolCode)
        {
            return !SymbolData.Check(ref symbolCode) ? (char)0 : symbolCode[Index];
        }

        /// <summary>
        /// Get the echelon symbol for the coding scheme.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a echelon symbol for the echelon</returns>
        public static string GetEchelonSymbol(string symbolCode)
        {
            char key = GetCode(symbolCode);
            return Echelons.ContainsKey(key) ? Echelons[key] : string.Empty;
        }

        /// <summary>
        /// Get the echelon name for the coding scheme.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the echelon</returns>
        public static string GetName(string symbolCode)
        {
            char key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }

        /// <summary>
        /// Generate the text block representing the possible echelon value
        /// </summary>
        /// <param name="ms">the symbol to which the generated rendering is attached.</param>
        /// <param name="symbolCode">the symbol code so we know which echelon to use</param>
        /// <returns>the height of the echelon text string above the symbol</returns>
        internal static double Generate(MilSymbol ms, string symbolCode)
        {
            // This is the maximum height generated by this combination of pieces
            System.Windows.Rect r = ms.BaseRect;
            if (r.IsEmpty)
            {
                return 0;
            }

            double top = r.Top;

            // Echelon ignored if modifier index represents Installation or Mobility
            char mi = ModifierCode.GetCode(symbolCode);
            if (mi == ModifierCode.Installation || mi == ModifierCode.Mobility || mi == ModifierCode.Towed)
            {
                return top;
            }

            char echelon = GetCode(symbolCode);
            if (!Echelons.ContainsKey(echelon))
            {
                return top;
            }

            var tb = new TextBlock { Style = SymbolData.GetStyle("EchelonLabels") };

            var height = (double)tb.GetValue(TextBlock.FontSizeProperty);
            tb.Text = Echelons[echelon];
            if (string.IsNullOrEmpty(tb.Text))
            {
                return top;
            }

            // Make the text a little closer (0.90) to the symbol than it would otherwise be.
            top -= 0.90 * height;

            tb.FindTextExtent();
            tb.SetValue(Canvas.TopProperty, top); // this positions the top line just above the symbol, I think
            tb.SetValue(Canvas.LeftProperty, -tb.Width / 2);

            // Add the echelon to the symbol
            ms.AddChild("Echelon", tb);

            return top; // return the new height so that other decorations can be properly placed
        }
    }
}