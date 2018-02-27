using System;
using System.Linq;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using System.Windows;
#endif
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
#elif WINDOWS_UWP
                // https://stackoverflow.com/questions/338056/resourcedictionary-in-a-separate-assembly - the last answer
                "ms-appx:///MilSym.AppendixA.UWP/AirSpaceAppendixA.xaml",
                "ms-appx:///MilSym.AppendixA.UWP/GroundEquipmentAppendixA.xaml",
                "ms-appx:///MilSym.AppendixA.UWP/GroundInstallationAppendixA.xaml",
                "ms-appx:///MilSym.AppendixA.UWP/GroundUnitAppendixA.xaml",
                "ms-appx:///MilSym.AppendixA.UWP/SofWaterAppendixA.xaml",
                "ms-appx:///MilSym.AppendixA.UWP/SubsurfaceAppendixA.xaml"
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
#if WINDOWS_UWP
                dic => new ResourceDictionary { Source = new Uri(dic, UriKind.Absolute) }))
#else
                dic => new ResourceDictionary { Source = new Uri(dic, UriKind.Relative) }))
#endif
            {
                stencils.MergedDictionaries.Add(rd);
            }
        }
    }
}
