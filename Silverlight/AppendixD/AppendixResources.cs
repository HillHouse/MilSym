using System;
using System.Linq;
using System.Windows;
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
#else
            const string dictionary = "/MilSym.AppendixD.WPF;component/AppendixD.xaml";
#endif
            var rd = new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) };
            stencils.MergedDictionaries.Add(rd);
        }
    }
}
