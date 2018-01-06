// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="DebugLogger.cs">
//   Copyright © 2009-2012 HillHouse
// </copyright>
// <summary>
//   Sample logger implementation
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.LoadResources
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A sample default logger
    /// </summary>
    /// <typeparam name="T">
    /// The clas for which the logger is being created
    /// </typeparam>
    public class DebugLogger<T> : ILogger
        where T : class
    {
        /// <summary>
        /// Writes a log message to the debugger.
        /// </summary>
        /// <param name="level">
        /// The logging level for the message to be reported.
        /// </param>
        /// <param name="message">
        /// The error message to be reported.
        /// </param>
        public void WriteMessage(LogLevel level, string message)
        {
            Debug.WriteLine(typeof(T) + "::" + level + ":" + message);
        }

        /// <summary>
        /// Writes a log message to the debugger.
        /// </summary>
        /// <param name="level">
        /// The level for the message.
        /// </param>
        /// <param name="message">
        /// The error message to be reported.
        /// </param>
        /// <param name="exception">
        /// The exception to be reported.
        /// </param>
        public void WriteMessage(LogLevel level, string message, Exception exception)
        {
            Debug.WriteLine(typeof(T) + "::" + level + ":" + message + " " + exception);
        }
    }
}
