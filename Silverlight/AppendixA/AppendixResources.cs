using System;
using System.Linq;
using System.Windows;
using MilSym.LoadResources;

namespace MilSym.AppendixA
{
    public class AppendixResources : ILoadResources
    {
        public void LoadDictionary(ResourceDictionary stencils)
        {
            var dics = new[]
            {
#if SILVERLIGHT
                "/MilSym.AppendixA;component/AirSpaceAppendixA.xaml",
                "/MilSym.AppendixA;component/GroundEquipmentAppendixA.xaml",
                "/MilSym.AppendixA;component/GroundInstallationAppendixA.xaml",
                "/MilSym.AppendixA;component/GroundUnitAppendixA.xaml",
                "/MilSym.AppendixA;component/SofWaterAppendixA.xaml",
                "/MilSym.AppendixA;component/SubsurfaceAppendixA.xaml"
#else
                "/MilSym.AppendixA.WPF;component/AirSpaceAppendixA.xaml",
                "/MilSym.AppendixA.WPF;component/GroundEquipmentAppendixA.xaml",
                "/MilSym.AppendixA.WPF;component/GroundInstallationAppendixA.xaml",
                "/MilSym.AppendixA.WPF;component/GroundUnitAppendixA.xaml",
                "/MilSym.AppendixA.WPF;component/SofWaterAppendixA.xaml",
                "/MilSym.AppendixA.WPF;component/SubsurfaceAppendixA.xaml"
#endif
            };
            foreach (var rd in dics.Select(
                dic => new ResourceDictionary {Source = new Uri(dic, UriKind.Relative)}))
            {
                stencils.MergedDictionaries.Add(rd);
            }
        }
    }
}
