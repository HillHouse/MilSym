// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="CategoryBattleDimension.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the CategoryBattleDimension portion of symbols in MIL STD-2525C
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
    /// Support methods for managing the CategoryBattleDimension portion of symbols in MIL STD-2525C
    /// </summary>
    public static class CategoryBattleDimension // category or battle dimension
    {
        ////    BattleDimension for warfighting (S) and intelligence (I) - index is 2
        ////
        ////    A - AIR                            
        ////    F - SOF                            
        ////    G - GROUND                        
        ////    P - SPACE                        blacken the upper portion of the air symbol
        ////    S - SEA SURFACE                    
        ////    U - SEA SUBSURFACE                
        ////    X - OTHER (No frame)            
        ////    Z - UNKNOWN                      display large black question mark for interior marking

        ////    Category for tactical graphics (G) - index is 2
        ////
        ////    F - FIRE SUPPORT
        ////    G - C2 & GENERAL MANEUVER
        ////    M - MOBILITY/SURVIVABILITY
        ////    O - OTHER
        ////    S - COMBAT SERVICE SUPPORT
        ////    T - TASKS

        ////    Category for stability operations (O) - index is 2
        ////
        ////    G - NONMILITARY GROUP OR ORGANIZATION
        ////    I - ITEMS
        ////    L - LOCATIONS
        ////    O - OPERATIONS
        ////    P - INDIVIDUAL
        ////    R - RAPE
        ////    V - VIOLENT ACTIVITIES

        ////    Category for emergency management (E) - index is 2
        ////
        ////    F - INFRASTRUCTURE
        ////    I - INCIDENT
        ////    N - NATURAL EVENTS
        ////    O - OPERATIONS

        //// Category for METOC (W) - index is 1
        ////
        ////    A - Atmospheric
        ////    O - Oceanic
        ////    S - Space

        /// <summary>
        /// A unique enumerable value representing Emergency Management Infrastructure
        /// </summary>
        public const int EmInfrastructure = 0x010;
        
        /// <summary>
        /// A unique enumerable value representing Emergency Management Incident
        /// </summary>
        public const int EmIncident = 0x020;
        
        /// <summary>
        /// A unique enumerable value representing Emergency Management Natural events
        /// </summary>
        public const int EmNaturalEvents = 0x030;

        /// <summary>
        /// A unique enumerable value representing Emergency Management Operations
        /// </summary>
        public const int EmOperations = 0x040;

        /// <summary>
        /// A unique enumerable value representing Meteorological Atmospheric
        /// </summary>
        public const int MetocAtmospheric = 0x050;
        
        /// <summary>
        /// A unique enumerable value representing Meteorological Oceanic
        /// </summary>
        public const int MetocOceanic = 0x060;
        
        /// <summary>
        /// A unique enumerable value representing Meteorological Space
        /// </summary>
        public const int MetocSpace = 0x070;

        /// <summary>
        /// A unique enumerable value representing Stability Operations Non military group or organization
        /// </summary>
        public const int SoNonMilitaryGroupOrOrganization = 0x080;
        
        /// <summary>
        /// A unique enumerable value representing Stability Operations Items
        /// </summary>
        public const int SoItems = 0x090;
        
        /// <summary>
        /// A unique enumerable value representing Stability Operations Locations
        /// </summary>
        public const int SoLocations = 0x0a0;
        
        /// <summary>
        /// A unique enumerable value representing Stability Operations Operations
        /// </summary>
        public const int SoOperations = 0x0b0;
        
        /// <summary>
        /// A unique enumerable value representing Stability Operations Individual
        /// </summary>
        public const int SoIndividual = 0x0c0;
        
        /// <summary>
        /// A unique enumerable value representing Stability Operations Rape
        /// </summary>
        public const int SoRape = 0x0d0;
        
        /// <summary>
        /// A unique enumerable value representing Stability Operations Violent activities
        /// </summary>
        public const int SoViolentActivities = 0x0e0;

        /// <summary>
        /// A unique enumerable value representing Tactical Graphics Fire support
        /// </summary>
        public const int TgFireSupport = 0x0f0;
        
        /// <summary>
        /// A unique enumerable value representing Tactical Graphics Command/Control and general maneuver
        /// </summary>
        public const int TgC2GeneralManeuver = 0x100;
        
        /// <summary>
        /// A unique enumerable value representing Tactical Graphics Mobility survivability
        /// </summary>
        public const int TgMobilitySurvivability = 0x110;
        
        /// <summary>
        /// A unique enumerable value representing Tactical Graphics Other
        /// </summary>
        public const int TgOther = 0x120;
        
        /// <summary>
        /// A unique enumerable value representing Tactical Graphics Combat service support
        /// </summary>
        public const int TgCombatServiceSupport = 0x130;
        
        /// <summary>
        /// A unique enumerable value representing Tactical Graphics Tasks
        /// </summary>
        public const int TgTasks = 0x140;

        /// <summary>
        /// A unique enumerable value representing Battle Dimension Air
        /// </summary>
        public const int BdAir = 0x150;
        
        /// <summary>
        /// A unique enumerable value representing Battle Dimension Ground
        /// </summary>
        public const int BdGround = 0x160;
        
        /// <summary>
        /// A unique enumerable value representing Battle Dimension Other
        /// </summary>
        public const int BdOther = 0x170;
        
        /// <summary>
        /// A unique enumerable value representing Battle Dimension Sea surface
        /// </summary>
        public const int BdSeaSurface = 0x180;
        
        /// <summary>
        /// A unique enumerable value representing Battle Dimension Space
        /// </summary>
        public const int BdSpace = 0x190;
        
        /// <summary>
        /// A unique enumerable value representing Battle Dimension Special operations force
        /// </summary>
        public const int BdSpecialOperationsForce = 0x1a0;
        
        /// <summary>
        /// A unique enumerable value representing Battle Dimension Subsurface
        /// </summary>
        public const int BdSubsurface = 0x1b0;

        /// <summary>
        /// A unique enumerable value representing Battle Dimension Unknown
        /// </summary>
        public const int BdUnknown = 0x1c0;

        /// <summary>
        /// Location of classical battle dimension in symbol code
        /// </summary>
        private const int BdIndex = 2;

        /// <summary>
        /// Location of emergency management battle dimension category in symbol code
        /// </summary>
        private const int EmIndex = 2;

        /// <summary>
        /// Location of meteorology battle dimension category in symbol code
        /// </summary>
        private const int MetocIndex = 1;

        /// <summary>
        /// Location of stability operations battle dimension category in symbol code
        /// </summary>
        private const int SoIndex = 2;

        /// <summary>
        /// Location of tactical graphics battle dimension category in symbol code
        /// </summary>
        private const int TgIndex = 2;

        /// <summary>
        /// Dictionary mapping symbol codes to enumerable values
        /// </summary>
        private static readonly IDictionary<char, int> Ems = new Dictionary<char, int>
        {
            { 'F', EmInfrastructure },
            { 'I', EmIncident },
            { 'N', EmNaturalEvents },
            { 'O', EmOperations }
        };

        /// <summary>
        /// Dictionary mapping of enumerated values to strings
        /// </summary>
        private static readonly Dictionary<int, string> EmNames = new Dictionary<int, string>
        {
            { EmInfrastructure, "Infrastructure" },
            { EmIncident, "Incident" },
            { EmNaturalEvents, "Natural Events" },
            { EmOperations, "Operations" }
        };

        /// <summary>
        /// Dictionary mapping symbol codes to enumerable values
        /// </summary>
        private static readonly IDictionary<char, int> Bds = new Dictionary<char, int>
        {     
            { 'A', BdAir },        
            { 'G', BdGround },    
            { 'X', BdOther },    
            { 'S', BdSeaSurface },       
            { 'P', BdSpace },    
            { 'F', BdSpecialOperationsForce },  
            { 'U', BdSubsurface },       
            { 'Z', BdUnknown }    
        };

        /// <summary>
        /// Dictionary mapping of enumerated values to strings
        /// </summary>
        private static readonly Dictionary<int, string> BdNames = new Dictionary<int, string>
        { 
             { BdSpace, "Space" },
             { BdAir, "Air" },
             { BdGround, "Ground" },
             { BdSeaSurface, "Sea Surface" },
             { BdSubsurface, "Sea Subsurface" },
             { BdSpecialOperationsForce, "SOF" },
             { BdOther, "Other (No Frame)" },
             { BdUnknown, "Unknown" }
        };

        /// <summary>
        /// Dictionary mapping symbol codes to enumerable values
        /// </summary>
        private static readonly IDictionary<char, int> Tgs = new Dictionary<char, int>
        { 
            { 'F', TgFireSupport },
            { 'G', TgC2GeneralManeuver },
            { 'M', TgMobilitySurvivability },
            { 'O', TgOther },
            { 'S', TgCombatServiceSupport },
            { 'T', TgTasks }
        };

        /// <summary>
        /// Dictionary mapping of enumerated values to strings
        /// </summary>
        private static readonly Dictionary<int, string> TgNames = new Dictionary<int, string>
        { 
            { TgFireSupport, "Fire Support" },
            { TgC2GeneralManeuver, "Command/Control and General Maneuver" },
            { TgMobilitySurvivability, "Mobility Survivablity" },
            { TgOther, "Other" },
            { TgCombatServiceSupport, "Combat Service Support" },
            { TgTasks, "Tasks" }
        };

        /// <summary>
        /// Dictionary mapping symbol codes to enumerable values
        /// </summary>
        private static readonly IDictionary<char, int> Sos = new Dictionary<char, int>
        { 
            { 'G', SoNonMilitaryGroupOrOrganization },    
            { 'I', SoItems },                            
            { 'L', SoLocations },                        
            { 'O', SoOperations },                        
            { 'P', SoIndividual },                        
            { 'R', SoRape },                            
            { 'V', SoViolentActivities }    
        };

        /// <summary>
        /// Dictionary mapping of enumerated values to strings
        /// </summary>
        private static readonly Dictionary<int, string> SoNames = new Dictionary<int, string>
        { 
            { SoNonMilitaryGroupOrOrganization, "Non-Military Group Or Organization" },    
            { SoItems, "Items" },                            
            { SoLocations, "Locations" },                        
            { SoOperations, "Operations" },                        
            { SoIndividual, "Individual" },                        
            { SoRape, "Rape" },                            
            { SoViolentActivities, "ViolentActivities" }                
        };

        /// <summary>
        /// Dictionary mapping symbol codes to enumerable values
        /// </summary>
        private static readonly IDictionary<char, int> Metocs = new Dictionary<char, int>
        { 
            { 'A', MetocAtmospheric },
            { 'O', MetocOceanic },
            { 'S', MetocSpace }
        };

        /// <summary>
        /// Dictionary mapping of enumerated values to strings
        /// </summary>
        private static readonly Dictionary<int, string> MetocNames = new Dictionary<int, string>
        { 
            { MetocAtmospheric, "Atmospheric" },
            { MetocOceanic, "Oceanic" },
            { MetocSpace, "Space" }
        };

        /// <summary>
        /// Get the battle dimension code from the symbol code.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the battle dimension code</returns>
        public static int GetCode(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return 0;
            }

            int codingScheme = CodingScheme.GetCode(symbolCode);
            switch (codingScheme)
            {
                case CodingScheme.EmergencyManagement:
                    return Ems.ContainsKey(symbolCode[EmIndex]) ? Ems[symbolCode[EmIndex]] : 0;
                case CodingScheme.StabilityOperations:
                    return Sos.ContainsKey(symbolCode[SoIndex]) ? Sos[symbolCode[SoIndex]] : 0;
                case CodingScheme.Intelligence:
                case CodingScheme.Warfighting:
                    return Bds.ContainsKey(symbolCode[BdIndex]) ? Bds[symbolCode[BdIndex]] : 0;
                case CodingScheme.Weather:
                    return Metocs.ContainsKey(symbolCode[MetocIndex]) ? Metocs[symbolCode[MetocIndex]] : 0;
                case CodingScheme.TacticalGraphics:
                    return Tgs.ContainsKey(symbolCode[TgIndex]) ? Tgs[symbolCode[TgIndex]] : 0;
            }

            return 0;
        }

        /// <summary>
        /// Get a friendly name for the battle dimension.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the battle dimension</returns>
        public static string GetName(string symbolCode)
        {
            int key = GetCode(symbolCode);
            int codingScheme = CodingScheme.GetCode(symbolCode);
            switch (codingScheme)
            {
                case CodingScheme.EmergencyManagement:
                        return EmNames.ContainsKey(key) ? EmNames[key] : string.Empty;
                case CodingScheme.StabilityOperations:
                        return SoNames.ContainsKey(key) ? SoNames[key] : string.Empty;
                case CodingScheme.Intelligence:
                case CodingScheme.Warfighting:
                        return BdNames.ContainsKey(key) ? BdNames[key] : string.Empty;
                case CodingScheme.Weather:
                        return MetocNames.ContainsKey(key) ? MetocNames[key] : string.Empty;
                case CodingScheme.TacticalGraphics:
                        return TgNames.ContainsKey(key) ? TgNames[key] : string.Empty;
            }

            return string.Empty;
        }
    }
}