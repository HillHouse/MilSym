#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif

namespace MilSym.LoadResources
{
    public interface ILoadResources
    {
        void LoadDictionary(ResourceDictionary stencils);
    }
}
