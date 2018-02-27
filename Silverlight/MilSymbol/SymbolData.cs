// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="SymbolData.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   SymbolData is the core data class for the resource dictionary templates for all symbology.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilSymbol
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
#if WINDOWS_UWP
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
 #else
    using System.Windows;
    using System.Windows.Controls;
 #endif

    using MilSym.LoadResources;
    using MilSym.MilSymbol.Schemas;

    /// <summary>
    /// SymbolData is the core data class for the resource dictionary templates for all symbology.
    /// </summary>
    public class SymbolData
    {
        /// <summary>
        /// Rectangular bounding boxes for the various symbol backgrounds plus ...
        /// the area of each bounding box plus ...
        /// the height of each bounding box.
        /// </summary>
        public static readonly IDictionary<string, Scaling> Rects = new Dictionary<string, Scaling> 
        {                                                                                       /* SQRT */

            // Neutral Default
            { "N137137G", new Scaling(new Rect(new Point(-137, -137), new Point(137, 137)), 274 /*75076*/, 274) },
            
            // Neutral Air, Space
            { "N137149A", new Scaling(new Rect(new Point(-137, -149), new Point(137, 144)), 283 /*80282*/, 293) }, 
            
            // Neutral Subsurface
            { "N137149U", new Scaling(new Rect(new Point(-137, -144), new Point(137, 149)), 283 /*80282*/, 293) },
            
            // Hostile Default
            { "H180180G", new Scaling(new Rect(new Point(-180, -180), new Point(180, 180)), 255 /*65166*/, 360) },
            
            // Hostile Air, Space
            { "H137161A", new Scaling(new Rect(new Point(-137, -161), new Point(137, 156)), 265 /*70226*/, 317) },
            
            // Hostile Subsurface
            { "H137161U", new Scaling(new Rect(new Point(-137, -156), new Point(137, 161)), 265 /*70226*/, 317) },
            
            // Friendly Default
            { "F185125G", new Scaling(new Rect(new Point(-185, -125), new Point(185, 125)), 304 /*92500*/, 250) },
            
            // Friendly Air, Space
            { "F137149A", new Scaling(new Rect(new Point(-137, -149), new Point(137, 144)), 253 /*64143*/, 293) }, 
            
            // Friendly Subsurface
            { "F137149U", new Scaling(new Rect(new Point(-137, -144), new Point(137, 149)), 253 /*64145*/, 293) },
            
            // Friendly Sea Surface, Other
            { "F149149G", new Scaling(new Rect(new Point(-149, -149), new Point(149, 149)), 265 /*70140*/, 298) },
            
            // Unknown Default
            { "U178178G", new Scaling(new Rect(new Point(-176, -178), new Point(176, 178)), 286 /*81862*/, 356) },
            
            // Unknown Air, Space
            { "U183161A", new Scaling(new Rect(new Point(-183, -159), new Point(183, 160)), 291 /*84903*/, 319) },
            
            // Unknown Subsurface
            { "U183161U", new Scaling(new Rect(new Point(-183, -160), new Point(183, 159)), 291 /*84903*/, 319) }
        };

        /// <summary>
        /// Half the default line width
        /// </summary>
        internal const double HalfWidth = 5.0;

        /// <summary>
        /// This is the resource dictionary that ultimately holds all of the resource dictionary entries
        /// for all the stencils.
        /// </summary>
        private static readonly ResourceDictionary Stencils;

        /// <summary>
        /// These are the infrastructure equipment codes from appendix G ("E*F*" + code)
        /// </summary>
        private static readonly IList<string> EmInfrastructureEquipment = new List<string> { "BA", "MA", "MC" };

        /// <summary>
        /// These are the operations equipment codes from appendix G ("E*O*" + code)
        /// </summary>
        private static readonly IList<string> EmOperationsEquipment = new List<string>
            {
                "AB",
                "AE",
                "AF",
                "BB",
                "CB",
                "CC",
                "EA",
                "EB",
                "EC",
                "ED",
                "EE",
                "DB",
                "DDB",
                "DEB",
                "DFB",
                "DGB",
                "DHB",
                "DIB",
                "DJB",
                "DLB",
                "DMB",
                "DOB"
            };

        /// <summary>
        /// A list of the leading characters for each appendix (S for A, G for B, W for C, I for D, O for E, and E for G)
        /// </summary>
        private static readonly List<char> Leads = new List<char> { 'S', 'G', 'W', 'I', 'O', 'E' };

        /// <summary>
        /// The message logger
        /// </summary>
        private static readonly ILogger Log = LoggerFactory<SymbolData>.GetLogger();

        /// <summary>
        /// Initializes static members of the <see cref="SymbolData"/> class.
        /// </summary>
        static SymbolData()
        {
            if (Stencils == null)
            {
                Stencils = new ResourceDictionary();
            }
        }

        /// <summary>
        /// Does a preliminary check for a valid symbol code.
        /// In the future this may consult a rule base to see if the code is valid.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code to check for validity.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the symbol code is valid.
        /// </returns>
        public static bool Check(ref string symbolCode)
        {
            if (symbolCode == null)
            {
                Log.WriteMessage(LogLevel.Info, "Symbol code '" + symbolCode + "' cannot be null.");
                return false;
            }

            if (symbolCode.Length != 15)
            {
                Log.WriteMessage(LogLevel.Info, "Symbol code '" + symbolCode + "' must be 15 characters.");
                return false;
            }

            symbolCode = symbolCode.ToUpperInvariant();
            return true;
        }

        /// <summary>
        /// Gets a named style from the resource dictionaries
        /// </summary>
        /// <param name="style">
        /// The name of the style to get.
        /// </param>
        /// <returns>
        /// The Style corresponding to the passed in name.
        /// </returns>
        public static Style GetStyle(string style)
        {
            if (Stencils.MergedDictionaries.Count == 0)
            {
                LoadMissingData(char.MinValue);
            }

            return Stencils[style] as Style;
        }

        /// <summary>
        /// Gets a named string from the resource dictionaries
        /// </summary>
        /// <param name="key">
        /// The name of the string to get.
        /// </param>
        /// <returns>
        /// The string corresponding to the passed in name.
        /// </returns>
        public static string GetString(string key)
        {
            if (Stencils.MergedDictionaries.Count == 0)
            {
                LoadMissingData(char.MinValue);
            }

            return Stencils[key] as string;
        }

        /// <summary>
        /// This function returns the symbol bounds and some scaling factors 
        /// (area and height) for the various standard symbol backgrounds.
        /// The exceptions to these sizes are the symbols that have headquarters 
        /// or installations built-in.
        /// If that built-in is in the normal symbol code place (SymbolCode[10-11])
        /// we're OK.
        /// But the designers threw in a few curveballs that we're going to special 
        /// case for now.
        /// </summary>
        /// <param name="sc">
        /// The symbol code.
        /// </param>
        /// <returns>
        /// A Scaling object representing the bounding rectangle and various scaling factors.
        /// </returns>
        public static Scaling GetScaling(string sc)
        {
            if (!Check(ref sc))
            {
                return new Scaling(Rect.Empty, 300.0 * 300.0, 300.0);
            }

            int identityKey = StandardIdentity.GetNormalizedStandardIdentity(sc);
            int dimensionKey = NormalizeBattleDimension(sc);
            switch (identityKey)
            {
                case StandardIdentity.Friend:
                    if (dimensionKey == CategoryBattleDimension.BdSpace ||
                        dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return Rects["F137149A"];
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return Rects["F137149U"];
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSeaSurface ||
                        dimensionKey == CategoryBattleDimension.BdOther ||
                        IsEquipment(sc))
                    {
                        return Rects["F149149G"];
                    }

                    return Rects["F185125G"];
                case StandardIdentity.Hostile:
                    if (dimensionKey == CategoryBattleDimension.BdSpace ||
                        dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return Rects["H137161A"];
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return Rects["H137161U"];
                    }

                    return Rects["H180180G"];
                case StandardIdentity.Neutral:
                    if (dimensionKey == CategoryBattleDimension.BdSpace ||
                        dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return Rects["N137149A"];
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return Rects["N137149U"];
                    }

                    return Rects["N137137G"];
                case StandardIdentity.Unknown:
                    if (dimensionKey == CategoryBattleDimension.BdSpace ||
                        dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return Rects["U183161A"];
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return Rects["U183161U"];
                    }

                    return Rects["U178178G"];
            }

            return new Scaling(Rect.Empty, 300.0 * 300.0, 300.0);
        }

        /// <summary>
        /// When is the top of the symbol flat?
        /// Answer:
        /// 1. normalize StandardIdentity to 'F', 'H', 'N' or 'U'
        /// 2. return true when it is Subsurface
        /// 3. return true when it is Neutral
        /// 4. return true when it is Friendly Units, Installations, or SOF
        /// 5. return true when it is Friendly Emergency Operations
        ///   a. technically, friendly emergency equipment is not flat 
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code to check for a flat top to the symbol.
        /// </param>
        /// <returns>
        /// Returns a boolean to indicate whether the top of the symbol is flat.
        /// </returns>
        internal static bool IsTopFlat(string symbolCode)
        {
            if (!Check(ref symbolCode))
            {
                return false;
            }

            int dimensionKey = CategoryBattleDimension.GetCode(symbolCode);
            if (dimensionKey == CategoryBattleDimension.BdSubsurface)
            {
                return true;
            }

            int identityKey = StandardIdentity.GetNormalizedStandardIdentity(symbolCode);
            if (identityKey == StandardIdentity.Neutral)
            {
                return true;
            }

            if (identityKey == StandardIdentity.Friend)
            {
                if (dimensionKey == CategoryBattleDimension.BdSpace || dimensionKey == CategoryBattleDimension.BdAir ||
                    dimensionKey == CategoryBattleDimension.BdSeaSurface ||
                    dimensionKey == CategoryBattleDimension.BdUnknown)
                {
                    return false;
                }

                if (IsEquipment(symbolCode))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// This function returns the symbol bounds for the various standard symbol backgrounds.
        /// </summary>
        /// <param name="sc">
        /// The symbol code.
        /// </param>
        /// <returns>
        /// A Rect representing the bounding rectangle in the same space as the resource dictionary.
        /// </returns>
        internal static Rect GetBounds(string sc)
        {
            return GetScaling(sc).Bounds;
        }

        /// <summary>
        /// Return the headquarters factor to help position the headquarters staff
        /// </summary>
        /// <param name="sc">
        /// The symbol code for the headquarters.
        /// </param>
        /// <returns>
        /// A fractional offset as a double ranging from 0.0 to 1.0. 
        /// </returns>
        internal static double GetHqFactor(string sc)
        {
            if (!Check(ref sc))
            {
                return 0;
            }

            int identityKey = StandardIdentity.GetNormalizedStandardIdentity(sc);
            int dimensionKey = NormalizeBattleDimension(sc);
            switch (identityKey)
            {
                case StandardIdentity.Friend:
                    if (dimensionKey == CategoryBattleDimension.BdSpace || dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return 1.0;
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return 0.0;
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSeaSurface ||
                        dimensionKey == CategoryBattleDimension.BdOther || IsEquipment(sc))
                    {
                        return 0.5; // of course there is no headquarters that is also equipment
                    }

                    return 1.0;
                case StandardIdentity.Hostile:
                    if (dimensionKey == CategoryBattleDimension.BdSpace || dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return 1.0;
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return 0.0;
                    }

                    return 0.5;
                case StandardIdentity.Neutral:
                    return 1.0;
                case StandardIdentity.Unknown:
                    if (dimensionKey == CategoryBattleDimension.BdSpace || dimensionKey == CategoryBattleDimension.BdAir)
                    {
                        return 0.7;
                    }

                    if (dimensionKey == CategoryBattleDimension.BdSubsurface)
                    {
                        return 0.3;
                    }

                    return 0.5;
            }

            return 0.0;
        }

        /// <summary>
        /// Returns the ControlTemplate corresponding to the passed in template derived from a symbol code.
        /// </summary>
        /// <param name="template">
        /// The template string corresponding to a control template.
        /// </param>
        /// <returns>
        /// The control template.
        /// </returns>
        internal static ControlTemplate GetControlTemplate(string template)
        {
            try
            {
                if (Leads.Contains(template[0]))
                {
                    LoadMissingData(template[0]);
                }

                return Stencils[template] as ControlTemplate;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(LogLevel.Error, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns the DataTemplate corresponding to the passed in template derived from a symbol code.
        /// </summary>
        /// <param name="template">
        /// The template string corresponding to a data template.
        /// </param>
        /// <returns>
        /// The data template.
        /// </returns>
        internal static DataTemplate GetDataTemplate(string template)
        {
            if (Leads.Contains(template[0]))
            {
                LoadMissingData(template[0]);
            }

            return Stencils[template] as DataTemplate;
        }

        /// <summary>
        /// Determine once and for all if a symbol code is for a piece of equipment.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code.
        /// </param>
        /// <returns>
        /// A boolean indicating if the symbol code represents equipment.
        /// </returns>
        internal static bool IsEquipment(string symbolCode)
        {
            if (!Check(ref symbolCode))
            {
                return false;
            }

            var schemeKey = CodingScheme.GetCode(symbolCode);
            if (schemeKey == CodingScheme.Intelligence)
            {
                return true;
            }

            var func = symbolCode.Substring(4, 6).Trim(new[] { '-' });
            if (schemeKey == CodingScheme.Warfighting &&
                !string.IsNullOrEmpty(func) &&
                func[0] == 'E')
            {
                return true;
            }

            if (schemeKey == CodingScheme.EmergencyManagement)
            {
                var catKey = CategoryBattleDimension.GetCode(symbolCode);
                if (catKey == CategoryBattleDimension.EmOperations &&
                    EmOperationsEquipment.Contains(func))
                {
                    return true;
                }

                if (catKey == CategoryBattleDimension.EmInfrastructure &&
                    EmInfrastructureEquipment.Contains(func))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the symbol requires a dashed outline.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code to check.
        /// </param>
        /// <returns>
        /// A boolean to indicate whether the outline should be dashed.
        /// </returns>
        internal static bool IsDashed(string symbolCode)
        {
            if (!Check(ref symbolCode))
            {
                return false;
            }

            switch (StandardIdentity.GetCode(symbolCode))
            {
                case StandardIdentity.Pending:
                case StandardIdentity.AssumedFriend:
                case StandardIdentity.Suspect:
                case StandardIdentity.ExercisePending:
                case StandardIdentity.ExerciseAssumedFriend:
                    {
                        return true;
                    }

                default:
                    if (StatusOperationalCapacity.GetCode(symbolCode) == StatusOperationalCapacity.AnticipatedPlanned)
                    {
                        return true;
                    }

                    return false;
            }
        }

        /// <summary>
        /// When is the base of the symbol flat?
        /// Flat symbols are easier to draw against.
        /// Answer:
        /// 1. normalize StandardIdentity to 'F', 'H', 'N' or 'U'
        /// 2. return true when it is Air or Space
        /// 3. return true when it is Neutral
        /// 4. return true when it is Friendly Units, Installations, or SOF
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the base of the symbol is flat.
        /// </returns>
        internal static bool IsBaseFlat(string symbolCode)
        {
            if (!Check(ref symbolCode))
            {
                return false;
            }

            int dimensionKey = CategoryBattleDimension.GetCode(symbolCode);
            if (dimensionKey == CategoryBattleDimension.BdSpace ||
                dimensionKey == CategoryBattleDimension.BdAir)
            {
                return true;
            }

            int identityKey = StandardIdentity.GetNormalizedStandardIdentity(symbolCode);
            if (identityKey == StandardIdentity.Neutral)
            {
                return true;
            }

            if (identityKey == StandardIdentity.Friend)
            {
                if (dimensionKey == CategoryBattleDimension.BdSeaSurface ||
                    dimensionKey == CategoryBattleDimension.BdSubsurface ||
                    dimensionKey == CategoryBattleDimension.BdUnknown)
                {
                    return false;
                }

                return !IsEquipment(symbolCode);
            }

            return false;
        }

        /// <summary>
        /// Loads an appendix on demand.
        /// </summary>
        /// <param name="lead">
        /// The leading character for the requested template controls which resource dictionary to load.
        /// Pass 0 to just load the default label and symbol outline files.
        /// </param>
        private static void LoadMissingData(char lead)
        {
            // We'll need some of these symbols if they haven't already been loaded
            if (Stencils.MergedDictionaries.Count == 0)
            {
#if SILVERLIGHT
                LoadDictionary("/MilSym.MilSymbol;component/Themes/LabelResources.xaml");
                LoadDictionary("/MilSym.MilSymbol;component/Themes/Outlines.xaml");
#elif WINDOWS_UWP
                // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
                LoadDictionary("ms-appx:///MilSym.MilSymbol.UWP/Themes/LabelResources.xaml");
                LoadDictionary("ms-appx:///MilSym.MilSymbol.UWP/Themes/Outlines.xaml");
#else
////            LoadDictionary("/MilSym.MilSymbol.WPF;component/LabelResources.xaml");
                LoadDictionary("/MilSym.MilSymbol.WPF;component/Themes/LabelResources.xaml");
                LoadDictionary("/MilSym.MilSymbol.WPF;component/Themes/Outlines.xaml");
#endif
                if (lead == char.MinValue)  
                {
                    return;     // no particular dictionary requested
                }
            }

            var app = CreateAppendix(lead) as ILoadResources;
            if (app != null)
            {
                app.LoadDictionary(Stencils);
            }

            Leads.Remove(lead);

            // Temporary code while we continue work on the tactical graphics
            if (lead == 'G')
            {
#if SILVERLIGHT
                LoadDictionary("/MilSym.MilSymbol;component/Themes/MultipointAppendixB.xaml");
#elif WINDOWS_UWP
                // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
                LoadDictionary("ms-appx:///MilSym.MilSymbol.UWP/Themes/MultipointAppendixB.xaml");
#else
                LoadDictionary("/MilSym.MilSymbol.WPF;component/Themes/MultipointAppendixB.xaml");
#endif
            }
        }

        /// <summary>
        /// Load an indicated dictionary and add it to Stencils.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary to load into the Stencils.
        /// </param>
        private static void LoadDictionary(string dictionary)
        {
#if WINDOWS_UWP
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Absolute) };
#else
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
#endif
            Stencils.MergedDictionaries.Add(rd);
        }

        /// <summary>
        /// Create an ILoadResources instance that can be invoked to load a resource dictionary.
        /// </summary>
        /// <param name="lead">
        /// The lead character for all the symbols in this appendix such as 'S' for AppendixA
        /// </param>
        /// <returns>
        /// Returns an instance of an ILoadResources instance for later invocation. 
        /// </returns>
        [System.Runtime.CompilerServices.MethodImpl
            (System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static object CreateAppendix(char lead)
        {
            switch (lead)
            {
                case 'S': return new AppendixA.AppendixResources();
                case 'G': return new AppendixB.AppendixResources();
                case 'W': return new AppendixC.AppendixResources();
                case 'I': return new AppendixD.AppendixResources();
                case 'O': return new AppendixE.AppendixResources();
                case 'E': return new AppendixG.AppendixResources();
            }

            return null;
        }

        /// <summary>
        /// Normalize here strictly means "what will the templates understand?" which is mostly air, land, and water.
        /// </summary>
        /// <param name="symbolCode">
        /// The symbol code.
        /// </param>
        /// <returns>
        /// The enumerated battle dimension value.
        /// </returns>
        private static int NormalizeBattleDimension(string symbolCode)
        {
            if (!Check(ref symbolCode))
            {
                return 0;
            }

            int bd = CategoryBattleDimension.GetCode(symbolCode);
            if (bd == CategoryBattleDimension.BdUnknown)
            {
                return CategoryBattleDimension.BdSeaSurface;
            }

            return bd;
        }
    }
}