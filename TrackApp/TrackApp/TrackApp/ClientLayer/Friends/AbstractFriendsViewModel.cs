using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Friends
{
    public abstract class AbstractFriendsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //commands
        public ICommand OnButtonTappedCommand { protected set; get; }
        public ICommand OnRefreshCommand { protected set; get; }

        //implementation fields
        protected UserFriends currentUserFriends;
        protected TrackUser currentUser;

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

        protected AbstractFriendsViewModel(TrackUser currentUser)
        {
            IsBusy = false;
            this.currentUser = currentUser;
        }


        public abstract Task PopulateAsync();


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
