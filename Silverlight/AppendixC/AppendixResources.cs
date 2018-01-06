using System;
using System.Windows;
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
#else
            const string dictionary = "/MilSym.AppendixC.WPF;component/AppendixC.xaml";
#endif
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
            stencils.MergedDictionaries.Add(rd);
        }
    }
}
