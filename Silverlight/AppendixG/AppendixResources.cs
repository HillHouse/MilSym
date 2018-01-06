using System;
using System.Windows;
using MilSym.LoadResources;

namespace MilSym.AppendixG
{
    public class AppendixResources : ILoadResources
    {
        // Dictionary string should be "/MilSym.AppendixG;component/Themes/AppendixG.xaml"
        public void LoadDictionary(ResourceDictionary stencils)
        {
#if SILVERLIGHT
            const string dictionary = "/MilSym.AppendixG;component/AppendixG.xaml";
#else
            const string dictionary = "/MilSym.AppendixG.WPF;component/AppendixG.xaml";
#endif
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
            stencils.MergedDictionaries.Add(rd);
        }
    }
}
