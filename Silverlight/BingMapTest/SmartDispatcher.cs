// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SmartDispatcher.cs">
//   Copyright © Microsoft Corporation
// </copyright>
// <summary>
//   Smart dispatcher that uses the UI thread when necessary.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace MapTest
// ReSharper restore CheckNamespace
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary> 
    /// A smart dispatcher system for routing actions to the user interface 
    /// thread. 
    /// </summary> 
    public static class SmartDispatcher
    {
        /// <summary> 
        /// A single Dispatcher instance to marshall actions to the user 
        /// interface thread. 
        /// </summary> 
        private static Dispatcher instance;

        /// <summary> 
        /// Backing field for a value indicating whether this is a design-time 
        /// environment. 
        /// </summary> 
        private static bool? designer;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!designer.HasValue)
                {
#if SILVERLIGHT
                    designer = DesignerProperties.IsInDesignTool;
#else
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    designer
                        = (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(FrameworkElement))
                        .Metadata.DefaultValue;
#endif
                }

                return designer.Value;
            }
        }

        /// <summary> 
        /// Initializes the SmartDispatcher system, attempting to use the 
        /// RootVisual of the plugin to retrieve a Dispatcher instance. 
        /// </summary> 
        public static void Initialize()
        {
            if (instance == null)
            {
                RequireInstance();
            }
        }

        /// <summary> 
        /// Initializes the SmartDispatcher system with the dispatcher 
        /// instance. 
        /// </summary> 
        /// <param name="dispatcher">The dispatcher instance.</param> 
        public static void Initialize(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            instance = dispatcher;
        }

        /// <summary> 
        /// Checks to see if we have an instance. 
        /// </summary> 
        /// <returns>Whether or not we have an instance.</returns> 
        public static bool CheckAccess()
        {
            if (instance == null)
            {
                RequireInstance();
                if (instance == null)
                {
                    return false;
                }
            }

            return instance.CheckAccess();
        }

        /// <summary> 
        /// Executes the specified delegate asynchronously on the user interface 
        /// thread. If the current thread is the user interface thread, the 
        /// dispatcher if not used and the operation happens immediately. 
        /// </summary> 
        /// <param name="a">A delegate to a method that takes no arguments and  
        /// does not return a value, which is either pushed onto the Dispatcher  
        /// event queue or immediately run, depending on the current thread.</param> 
        public static void BeginInvoke(Action a)
        {
            if (instance == null)
            {
                RequireInstance();
                if (instance == null)
                {
                    return;
                }
            }

            // If the current thread is the user interface thread, skip the 
            // dispatcher and directly invoke the Action. 
            if (instance.CheckAccess() || designer == true)
            {
                a();
            }
            else
            {
                instance.BeginInvoke(a);
            }
        }

        /// <summary> 
        /// Requires an instance and attempts to find a Dispatcher if one has 
        /// not yet been set. 
        /// </summary> 
        private static void RequireInstance()
        {
            // Design-time is more of a no-op, won't be able to resolve the 
            // dispatcher if it isn't already set in these situations. 
            if (IsInDesignModeStatic)
            {
                return;
            }

            // Attempt to use the RootVisual of the plugin to retrieve a 
            // dispatcher instance. This call will only succeed if the current 
            // thread is the UI thread. 
            try
            {
#if SILVERLIGHT
                instance = Application.Current.RootVisual.Dispatcher;
#else
                instance = Application.Current.MainWindow.Dispatcher;
#endif
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The first time SmartDispatcher is used must be from a user interface thread. Consider having the application call Initialize, with or without an instance.", e);
            }

            if (instance == null)
            {
                throw new InvalidOperationException("Unable to find a suitable Dispatcher instance.");
            }
        }
    }
} 