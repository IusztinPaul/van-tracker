using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper
{
    public abstract class BasicRefreshingModelView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OnRefreshCommand { protected set; get; }

        //list refresh property
        private bool isBusy; // state parameter for the refresh logic
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        protected TrackUser currentUser;

        public BasicRefreshingModelView(TrackUser currentUser)
        {
            this.currentUser = currentUser;
            IsBusy = false;
            OnRefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(async () => await PopulateAsync()), () => !IsBusy);
        }

        public abstract Task PopulateAsync();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
