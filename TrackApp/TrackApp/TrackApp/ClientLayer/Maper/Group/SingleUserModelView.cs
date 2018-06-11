using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TK.CustomMap;
using TK.CustomMap.Overlays;
using TrackApp.ClientLayer.Maper.Group.MapN;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group
{
    class SingleUserModelView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TKCircle> _userCurrentPositions;
        public ObservableCollection<TKCircle> UserCurrentPositions
        {
            get => _userCurrentPositions;
            set
            {
                _userCurrentPositions = value;
                OnPropertyChanged("UserCurrentPositions");
            }
        }

        private MapSpan _mapRegion;
        public MapSpan MapRegion
        {
            get => _mapRegion;
            set
            {
                _mapRegion = value;
                OnPropertyChanged("MapRegion");
            }
        }

        public SingleUserModelView()
        {
            UserCurrentPositions = new ObservableCollection<TKCircle>();
            MapRegion = MapSpan.FromCenterAndRadius(MapPage.DEFAULT_MAP_POSITION, Distance.FromMiles(ClientConsts.FROM_KM_MAP_DISTANCE));
        }

        public void AddCircleAtPosition(Position position)
        {
            UserCurrentPositions.Add(new TKCircle
            {
                Center = position,
                Color = Color.Teal,
                StrokeColor = Color.White,
                StrokeWidth = ClientConsts.CIRCLE_STROKE_WIDTH,
                Radius = ClientConsts.CIRCLE_RADIUS,
            });
        }

        public void SetMapRegion(Position position)
        {
            MapRegion = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(
                ClientConsts.FROM_KM_MAP_DISTANCE));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
