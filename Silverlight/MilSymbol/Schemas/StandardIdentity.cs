// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="StandardIdentity.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Support methods for managing the Standard Identity code portion of symbols in MIL STD-2525C
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
    using System.Text.RegularExpressions;

    /// <summary>
    /// Support methods for managing the Standard Identity code portion of symbols in MIL STD-2525C
    /// </summary>
    public static class StandardIdentity 
    {
        // All possible standard idenity (+exercise amplifying descriptor) index values
        //    P - PENDING                        use yellow unknown with dashed outline
        //    U - UNKNOWN                        use yellow unknown with solid outline
        //    G - EXERCISE PENDING            use yellow unknown with dashed outline + X
        //    W - EXERCISE UNKNOWN            use yellow unknown with solid outline + X
        //    A - ASSUMED FRIEND                use blue friend with dashed outline
        //    F - FRIEND                        use blue friend with solid outline
        //    M - EXERCISE ASSUMED FRIEND        use blue friend with dashed outline + X
        //    D - EXERCISE FRIEND                use blue friend with solid outline + X
        //    N - NEUTRAL                        use green neutral with solid outline
        //    L - EXERCISE NEUTRAL            use green neutral with solid outline + X
        //    S - SUSPECT                        use red hostile with dashed outline
        //    H - HOSTILE                        use red hostile with solid outline
        //    J - JOKER                        use red friend! with solid outline + J                        
        //    K - FAKER                        use ref friend! with solid outline + K

        /// <summary>
        /// The symbol's Standard Identity code for Assumed friend as a distinct enumerated value
        /// </summary>
        public const int AssumedFriend = 0x1;
        
        /// <summary>
        /// The symbol's Standard Identity code for Exercise assumed friend as a distinct enumerated value
        /// </summary>
        public const int ExerciseAssumedFriend = 0x2;
        
        /// <summary>
        /// The symbol's Standard Identity code for Exercise friend as a distinct enumerated value
        /// </summary>
        public const int ExerciseFriend = 0x3;
        
        /// <summary>
        /// The symbol's Standard Identity code for Exercise neutral as a distinct enumerated value
        /// </summary>
        public const int ExerciseNeutral = 0x4;
        
        /// <summary>
        /// The symbol's Standard Identity code for Exercise pending as a distinct enumerated value
        /// </summary>
        public const int ExercisePending = 0x5;
        
        /// <summary>
        /// The symbol's Standard Identity code for Exercise unknown as a distinct enumerated value
        /// </summary>
        public const int ExerciseUnknown = 0x6;
        
        /// <summary>
        /// The symbol's Standard Identity code for Faker as a distinct enumerated value
        /// </summary>
        public const int Faker = 0x7;
        
        /// <summary>
        /// The symbol's Standard Identity code for Friend as a distinct enumerated value
        /// </summary>
        public const int Friend = 0x8;
        
        /// <summary>
        /// The symbol's Standard Identity code for Hostile as a distinct enumerated value
        /// </summary>
        public const int Hostile = 0x9;
        
        /// <summary>
        /// The symbol's Standard Identity code for Joker as a distinct enumerated value
        /// </summary>
        public const int Joker = 0xa;
        
        /// <summary>
        /// The symbol's Standard Identity code for Neutral as a distinct enumerated value
        /// </summary>
        public const int Neutral = 0xb;
        
        /// <summary>
        /// The symbol's Standard Identity code for Pending as a distinct enumerated value 
        /// </summary>
        public const int Pending = 0xc;
        
        /// <summary>
        /// The symbol's Standard Identity code for Suspect as a distinct enumerated value
        /// </summary>
        public const int Suspect = 0xd;
        
        /// <summary>
        /// The symbol's Standard Identity code for Unknown as a distinct enumerated value
        /// </summary>
        public const int Unknown = 0xe;

        /// <summary>
        /// The symbol code's index for the standard identity, aka affiliation
        /// </summary>
        private const int Index = 1;

        /// <summary>
        /// Dictionary mapping symbol code value to unique numeric values
        /// </summary>
        private static readonly IDictionary<char, int> Sis = new Dictionary<char, int>
        {
            { 'P', Pending },
            { 'U', Unknown },
            { 'A', AssumedFriend },
            { 'F', Friend },
            { 'N', Neutral },
            { 'S', Suspect },
            { 'H', Hostile },
            { 'G', ExercisePending },
            { 'W', ExerciseUnknown },
            { 'M', ExerciseAssumedFriend },
            { 'D', ExerciseFriend },
            { 'L', ExerciseNeutral },
            { 'J', Joker },
            { 'K', Faker }
        };

        /// <summary>
        /// Dictionary mapping unique standard identity numeric values to symbol code values
        /// </summary>
        private static readonly IDictionary<int, char> SisRev = new Dictionary<int, char>
        {
            { Pending, 'P' },
            { Unknown, 'U' },
            { AssumedFriend, 'A' },
            { Friend, 'F' },
            { Neutral, 'N' },
            { Suspect, 'S' },
            { Hostile, 'H' },
            { ExercisePending, 'G' },
            { ExerciseUnknown, 'W' },
            { ExerciseAssumedFriend, 'M' },
            { ExerciseFriend, 'D' },
            { ExerciseNeutral, 'L' },
            { Joker, 'J' },
            { Faker, 'K' }
        };

        /// <summary>
        /// Dictionary mapping unique numeric identifiers to friendly names
        /// </summary>
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { Pending, "Pending" },
            { Unknown, "Unknown" },
            { Friend, "Friend" },
            { Neutral, "Neutral" },
            { Hostile, "Hostile" },
            { AssumedFriend, "Assumed Friend" },
            { Suspect, "Suspect" },
            { ExercisePending, "Exercise Pending" },
            { ExerciseUnknown, "Exercise Unknown" },
            { ExerciseFriend, "Exercise Friend" },
            { ExerciseNeutral, "Exercise Neutral" },
            { ExerciseAssumedFriend, "Exercise Assumed Friend" },
            { Joker, "Joker" },
            { Faker, "Faker" }
        };

        /// <summary>
        /// Those helpful Navy guys added some X's of their own so we'll have to special case it for now
        /// </summary>
        private static readonly Regex Exercise = new Regex("^S.U.WM[FGMS]*X-");

        /// <summary>
        /// Get the standard identity (aka affiliation) for the given symbol code.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>an arbitrary standard identity integer code for the given symbol code</returns>
        public static int GetCode(string symbolCode)
        {
            return !SymbolData.Check(ref symbolCode)
                       ? 0
                       : (Sis.ContainsKey(symbolCode[Index]) ? Sis[symbolCode[Index]] : 0);
        }

        /// <summary>
        /// Get the standard identity for the coding scheme.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>a friendly name for the standard identity</returns>
        public static string GetName(string symbolCode)
        {
            int key = GetCode(symbolCode);
            return Names.ContainsKey(key) ? Names[key] : string.Empty;
        }

        /// <summary>
        /// Determines if the background color should be hostile.
        /// This includes Hostile, Joker, Faker, and Suspect.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>true if the background color should be hostile</returns>
        public static bool IsColorHostile(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return false;
            }

            switch (symbolCode[Index])
            {
                case 'H':
                case 'J':
                case 'K':
                case 'S':
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Maps the actual standard identity into one of the four basic standard identities (affiliations)
        /// which include Friend, Hostile, Neutral, and Unknown. Normalize here strictly means "What will the templates understand?"
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>an arbitrary integer representing Friend, Hostile, Neutral or Unknown or 0 if there is an error</returns>
        public static int GetNormalizedStandardIdentity(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return 0;
            }

            switch (GetCode(symbolCode))
            {
                case ExercisePending:
                case Pending:
                case Unknown:
                case ExerciseUnknown:
                    return Unknown;
                case AssumedFriend:
                case ExerciseFriend:
                case Friend:
                case Joker: // uses the friendly shape
                case Faker: // uses the friendly shape
                case ExerciseAssumedFriend:
                    return Friend;
                case ExerciseNeutral:
                case Neutral: // neutral
                    return Neutral;
                case Hostile: // hostile
                case Suspect:
                    return Hostile;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Maps the internal numeric identifier back into the symbol's code for Standard Identity
        /// </summary>
        /// <param name="key">
        /// The internal numeric identifier
        /// </param>
        /// <returns>
        /// The Standard Identity symbol code
        /// </returns>
        public static char ToChar(int key)
        {
            return SisRev.ContainsKey(key) ? SisRev[key] : (char)0;
        }

        /// <summary>
        /// Gets the exercise amplifying descriptor which can be
        /// Exercise, Joker, or Faker.
        /// </summary>
        /// <param name="symbolCode">the symbol code</param>
        /// <returns>the display character for Exercise, Joker, or Faker or (char)0 if there is an error</returns>
        public static char GetExerciseAmplifyingDescriptor(string symbolCode)
        {
            if (!SymbolData.Check(ref symbolCode))
            {
                return (char)0;
            }

            int identityKey = GetCode(symbolCode);
            if (identityKey == Joker)
            {
                return 'J';
            }

            if (identityKey == Faker)
            {
                return 'K';
            }

            if (identityKey == ExerciseAssumedFriend || identityKey == ExerciseFriend || identityKey == ExerciseNeutral ||
                identityKey == ExercisePending || identityKey == ExerciseUnknown)
            {
                return 'X';
            }

            // Check the Navy's special cases
            if (Exercise.IsMatch(symbolCode))
            {
                return 'X';
            }

            return (char)0;
        }
    }
}