// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="LoggerFactory.cs">
//   Copyright © 2009-2012 HillHouse
// </copyright>
// <summary>
//   Sample logger factory that understands neither reflection nor configuration files
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
    /// <summary>
    /// Factory class to get the appropriate ILogger based on what is specified in 
    /// the App.Config file
    /// </summary>
    /// <typeparam name="T">
    /// The class for which the logger is being created
    /// </typeparam>
    public static class LoggerFactory<T>
        where T : class
    {
        /// <summary>
        /// Reference to the ILogger object.  Get a reference the first time, then keep it.
        /// In a generic class, there is a new static instance of this variable for each type!
        /// </summary>
        private static ILogger logger;

        /// <summary>
        /// Method to return our fake logger factory. Real applications should do this differently.
        /// </summary>
        /// <returns>
        /// The ILogger interface for the fake logger factory.
        /// </returns>
        public static ILogger GetLogger()
        {
            return logger ?? (logger = new DebugLogger<T>());
        }
    }
}