using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TrackApp.ClientLayer.Maper
{
    public class RequestsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<RequestNotification> _groupRequests;
        public ObservableCollection<RequestNotification> GroupRequests
        {
            get => _groupRequests;
            set
            {

                _groupRequests = value;
                OnPropertyChanged("GroupRequests");
            }
        }

        public RequestsViewModel()
        {
            GroupRequests = new ObservableCollection<RequestNotification>
            {
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                },
                new RequestNotification
                {
                    Username = "PaulakaPaul",
                    GroupName = "GroupTest1",
                    Icon = ClientConsts.USER_PLACEHOLDER
                }
            };
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class RequestNotification
        {
            public string Username { get; set; }
            public string GroupName { get; set; }
            public string Icon { get; set; }
        }
    }
}
