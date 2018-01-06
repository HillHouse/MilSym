// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="StatusOperationalCapacity.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Standard Operation Capacity code portion of symbols in MIL STD-2525C
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
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Support methods for managing the Standard Operation Capacity code portion of symbols in MIL STD-2525C
    /// </summary>
    public static class StatusOperationalCapacity
    {
        // A - ANTICIPATED/PLANNED
        // P - PRESENT (Units only)
        // C - PRESENT/FULLY CAPABLE
        // D - PRESENT/DAMAGED
        // X - PRESENT/DESTROYED
        // F - PRESENT/FULL TO CAPACITY

        /// <summary>
        /// The symbol's Status Operational Capacity code for Anticipated/planned
        /// </summary>
        public const char AnticipatedPlanned = 'A';

        /// <summary>
        /// The symbol's Status Operational Capacity code for Present
        /// </summary>
        public const char Present = 'P';

        /// <summary>
        /// The symbol's Status Operational Capacity code for Present/damaged
        /// </summary>
        public const char PresentDamaged = 'D';

        /// <summary>
        /// The symbol's Status Operational Capacity code for Present/destroyed
        /// </summary>
        public const char PresentDestroyed = 'X';

        /// <summary>
        /// The symbol's Status Operational Capacity code for Present/full to capacity
        /// </summary>
        public const char PresentFullToCapacity = 'F';

        /// <summary>
        /// The symbol's Status Operational Capacity code for Present/fully capable
        /// </summary>
        public const char PresentFullyCapable = 'C';

        /// <summary>
        /// The symbol code index for the status or operational capacity
        /// </summary>
        private const int Index = 3;

        /// <summary>
        /// Dictionary that maps symbol code values to friendly names
        /// </summary>
        private static readonly Dictionary<char, string> Names = new Dictionary<char, string>
        {
             { AnticipatedPlanned, "Anticipated/Planned" },
             { Present, "Present (Units Only)" },
             { PresentFullyCapable, "Present/Fully Capable" },
             { PresentDamaged, "Present/Damaged" },
             { PresentDestroyed, "Present/Destroyed" },
             { PresentFullToCapacity, "Present/Full to Capacity" }
         };

        /// <summary>
        /// Gets the symbol code's status or operational capacity
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the character for the symbol code's status or operational capacity</returns>
        public static char GetCode(string symbolCode)
        {
            return !SymbolData.Check(ref symbolCode) ? (char)0 : symbolCode[Index];
        }

        /// <summary>
        /// Get the symbol code's friendly name for the status or operational capacity.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the status/operational capacity</returns>
        public static string GetName(string symbolCode)
        {
            char key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }

        /// <summary>
        /// Generate a full or partial "X" shape to represent damaged or destroyed entities 
        /// </summary>
        /// <param name="symbolCode">symbol code for the entity</param>
        /// <returns>shape representing the damage</returns>
        internal static Shape Generate(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return null;
            }

            if (GetCode(symbolCode) == PresentDamaged)
            {
                return new Path
                {
                    Style = SymbolData.GetStyle("TS50"),
                    Data = new PathGeometry
                    {
                        Figures = new PathFigureCollection
                          {
                              Segment(-120, 174, 120, -174)
                          }
                    }
                };
            }

            if (GetCode(symbolCode) == PresentDestroyed)
            {
                return new Path
                {
                    Style = SymbolData.GetStyle("TS50"),
                    Data = new PathGeometry
                    {
                        Figures = new PathFigureCollection
                          {
                              Segment(-120, 174, 120, -174),
                              Segment(-120, -174, 120, 174)
                          }
                    }
                };
            }

            return null;
        }

        /// <summary>
        /// Generate a line segment as part of generating a slash or an X covering a map symbol.
        /// </summary>
        /// <param name="x1">
        /// First X-coordinate of the line segment.
        /// </param>
        /// <param name="y1">
        /// First Y-coordinate of the line segment.
        /// </param>
        /// <param name="x2">
        /// Second X-coordinate of the line segment.
        /// </param>
        /// <param name="y2">
        /// Second Y-coordinate of the line segment.
        /// </param>
        /// <returns>
        /// A PathFigure representing the given line segment.
        /// </returns>
        private static PathFigure Segment(double x1, double y1, double x2, double y2)
        {
            return new PathFigure
            {
                StartPoint = new Point(x1, y1),
                Segments = new PathSegmentCollection { new LineSegment { Point = new Point(x2, y2) } }
            };
        }
    }
}