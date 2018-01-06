// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="ILogger.cs">
//   Copyright © 2009-2012 HillHouse
// </copyright>
// <summary>
//   Logger interface
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

    /// <summary>
    /// Enum defining log levels to use in the common logging interface
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The message represents a fatal, unrecoverable error
        /// </summary>
        Fatal = 0,

        /// <summary>
        /// The message represents a recoverable error
        /// </summary>
        Error = 1,

        /// <summary>
        /// The message is a warning only
        /// </summary>
        Warn = 2,

        /// <summary>
        /// The message is simply informational
        /// </summary>
        Info = 3,

        /// <summary>
        /// The message is intended to detail progress or steps
        /// </summary>
        Verbose = 4
    }

    /// <summary>
    /// The logger interface that log implementations are expected to support 
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes a message to the log
        /// </summary>
        /// <param name="level">The message level</param>
        /// <param name="message">The message to write to the log</param>
        void WriteMessage(LogLevel level, string message);

        /// <summary>
        /// Writes a message to the log
        /// </summary>
        /// <param name="level">The message level</param>
        /// <param name="message">The message to write to the log</param>
        /// <param name="exception">The exception to write to the log</param>
        void WriteMessage(LogLevel level, string message, Exception exception);
    }
}
