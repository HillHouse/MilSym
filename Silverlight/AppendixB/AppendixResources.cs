using System;
using System.Windows;
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
#else
            const string dictionary = "/MilSym.AppendixB.WPF;component/AppendixB.xaml";
#endif
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
            stencils.MergedDictionaries.Add(rd);
        }
    }
}
