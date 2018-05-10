using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackApp.DataFormat.UserData;

namespace TrackApp.ClientLayer.Profile
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TrackUser _currentUser;
        public TrackUser CurrentUser {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        private bool _isSaving; // bool to control the activiyindicator state of running
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                _isSaving = value;
                OnPropertyChanged("IsSaving");
            }
        }

        private int _showGridData;
        public int ShowGridData // when the inidicator it's starting don't show the grid
        {
            get => _showGridData;
            set
            {
                _showGridData = value;
                OnPropertyChanged("ShowGridData");
            }
        }

        public ProfileViewModel(TrackUser currentUser)
        {
            CurrentUser = currentUser;
            IsSaving = false;
            ShowGridData = -1;
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
