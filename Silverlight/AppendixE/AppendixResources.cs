using System;
using System.Windows;
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
#else
            const string dictionary = "/MilSym.AppendixE.WPF;component/AppendixE.xaml";
#endif
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
            stencils.MergedDictionaries.Add(rd);
        }
    }
}
