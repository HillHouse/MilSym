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
using System.Windows.Navigation;

namespace GraphicsTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private NavigationWindow navigationWindow;

        private void Application_Startup(object sender, object e)
        {
            navigationWindow = new NavigationWindow
            {
                Height = 667,
                Width = 657
            };
            var page = new MainPage();
            navigationWindow.Navigate(page);
            navigationWindow.Show();
        }
    }
}
