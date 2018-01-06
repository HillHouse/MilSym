// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="App.xaml.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   Standard App.xaml.cs file
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace EsriMapTest
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Browser;

    /// <summary>
    /// The startup application object.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.Startup += this.ApplicationStartup;
            this.Exit += ApplicationExit;
            this.UnhandledException += ApplicationUnhandledException;

            InitializeComponent();
        }

        /// <summary>
        /// Application exit.
        /// </summary>
        /// <param name="sender">
        /// This parameter is ignored.
        /// </param>
        /// <param name="e">
        /// This parameter is also ignored.
        /// </param>
        private static void ApplicationExit(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Report the error to the DOM
        /// </summary>
        /// <param name="e">
        /// The exception event args.
        /// </param>
        private static void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The unhandled exception handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The exception event args.
        /// </param>
        private static void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!Debugger.IsAttached)
            {
                // This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
            }
        }

        /// <summary>
        /// Application startup.
        /// </summary>
        /// <param name="sender">
        /// This parameter is ignored.
        /// </param>
        /// <param name="e">
        /// This parameter is also ignored.
        /// </param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            RootVisual = new MainPage();
        }
    }
}