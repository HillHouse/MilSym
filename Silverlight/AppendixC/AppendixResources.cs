using System;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using System.Windows;
#endif
using MilSym.LoadResources;

namespace MilSym.AppendixC
{
    public class AppendixResources : ILoadResources
    {
        // Dictionary string should be "/MilSym.AppendixC;component/Themes/AppendixC.xaml"
        public void LoadDictionary(ResourceDictionary stencils)
        {
#if SILVERLIGHT
            const string dictionary = "/MilSym.AppendixC;component/AppendixC.xaml";
#elif WINDOWS_UWP
            // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
            const string dictionary = "ms-appx:///MilSym.AppendixC.UWP/AppendixC.xaml";
#else
            const string dictionary = "/MilSym.AppendixC.WPF;component/AppendixC.xaml";
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
