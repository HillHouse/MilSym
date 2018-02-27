using System;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using System.Windows;
#endif
using MilSym.LoadResources;

namespace MilSym.AppendixE
{
    public class AppendixResources : ILoadResources
    {
        // Dictionary string should be "/MilSym.AppendixE;component/Themes/AppendixE.xaml"
        public void LoadDictionary(ResourceDictionary stencils)
        {
#if SILVERLIGHT
            const string dictionary = "/MilSym.AppendixE;component/AppendixE.xaml";
#elif WINDOWS_UWP
            // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
            const string dictionary = "ms-appx:///MilSym.AppendixE.UWP/AppendixE.xaml";
#else
            const string dictionary = "/MilSym.AppendixE.WPF;component/AppendixE.xaml";
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
