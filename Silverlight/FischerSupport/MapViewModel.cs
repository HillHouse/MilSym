using MapControl;
using System.ComponentModel;

namespace MilSym.FischerSupport
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MapViewModel : ViewModelBase
    {
        private Location mapCenter = new Location(35.0, -70.0);
        public Location MapCenter
        {
            get { return mapCenter; }
            set
            {
                mapCenter = value;
                RaisePropertyChanged(nameof(MapCenter));
            }
        }
		
        public MapLayers MapLayers { get; } = new MapLayers();

        public MapViewModel()
        {
        }
    }
}
