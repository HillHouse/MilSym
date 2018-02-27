using System;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using System.Windows;
#endif
using MilSym.LoadResources;

namespace MilSym.AppendixB
{
    public class AppendixResources : ILoadResources
    {
        // Dictionary string should be "/MilSym.AppendixB;component/Themes/AppendixB.xaml"
        public void LoadDictionary(ResourceDictionary stencils)
        {
#if SILVERLIGHT
            const string dictionary = "/MilSym.AppendixB;component/AppendixB.xaml";
#elif WINDOWS_UWP
            // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
            const string dictionary = "ms-appx:///MilSym.AppendixB.UWP/AppendixB.xaml";
#else
            const string dictionary = "/MilSym.AppendixB.WPF;component/AppendixB.xaml";
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
