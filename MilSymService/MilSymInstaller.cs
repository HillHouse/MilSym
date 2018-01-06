// --------------------------------------------------------------------------------------------------------------------
// <copyright company="CF" file="MilSymInstaller.cs">
//   Copyright © 2012 CF
// </copyright>
// <summary>
//   Service definition for the MilSymbol PNG service.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSymService
{
    using System.Collections;
    using System.ComponentModel;
    using System.ServiceProcess;

    /// <summary>
    /// The installer for the military symbol service.
    /// </summary>
    [RunInstaller(true)]
    public class MilSymInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MilSymInstaller"/> class.
        /// </summary>
        public MilSymInstaller()
        {
            Installers.Add(
                new ServiceInstaller
                {
                    ServiceName = Service.Name,
                    StartType = ServiceStartMode.Automatic,
                });

            Installers.Add(
                new ServiceProcessInstaller
                {
                    Account = ServiceAccount.LocalSystem
                });
        }

        /// <summary>
        /// The event handler for the committed state.
        /// </summary>
        /// <param name="savedState">
        /// The saved state.
        /// </param>
        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);
            new ServiceController(Service.Name).Start();
        }
    }
}
