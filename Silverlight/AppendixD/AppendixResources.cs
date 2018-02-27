using System;
using System.Linq;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using System.Windows;
#endif
using MilSym.LoadResources;

namespace MilSym.AppendixD
{
    public class AppendixResources : ILoadResources
    {
        // Dictionary string should be "/MilSym.AppendixD;component/Themes/AppendixD.xaml"
        public void LoadDictionary(ResourceDictionary stencils)
        {
#if SILVERLIGHT
            const string dictionary = "/MilSym.AppendixD;component/AppendixD.xaml";
#elif WINDOWS_UWP
            // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
            const string dictionary = "ms-appx:///MilSym.AppendixD.UWP/AppendixD.xaml";
#else
            const string dictionary = "/MilSym.AppendixD.WPF;component/AppendixD.xaml";
#endif
#if WINDOWS_UWP
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Absolute) };
#else
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
#endif
            stencils.MergedDictionaries.Add(rd);
        }
    }
}
