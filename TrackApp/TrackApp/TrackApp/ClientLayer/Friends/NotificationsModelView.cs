using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using TrackApp.ClientLayer.Extensions;

namespace TrackApp.ClientLayer.Friends
{
    public class NotificationsModelView : AbstractFriendsViewModel
    {

        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;

            set
            {
                _notifications = value;
                OnPropertyChanged("Notifications");
            }
        }

        public NotificationsModelView(TrackUser currentUser) : base(currentUser) { }

        public async override Task PopulateAsync()
        {
            currentUserFriends = await new QueryUser().LoadData<UserFriends>(currentUser.Username);

            Notifications = new ObservableCollection<Notification>();
            //if (currentUserFriends == null || currentUserFriends.Notifications == null)
            //{
            //    Notifications = new ObservableCollection<Notification>();
            //} else
            if(currentUserFriends != null && currentUserFriends.Notifications != null) {

                // format username#[add|remove]#index
                // sort notifications so we will show them in real time order
                currentUserFriends.Notifications.Sort((a, b) =>
                    {
                        var a_index = a.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[2];
                        var b_index = b.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[2];
                        return -a_index.CompareTo(b_index); //sort descending
                    });


                foreach (string notif in currentUserFriends.Notifications)
                {
                    string[] elements = notif.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                    var notification = new Notification
                    {
                        Icon = elements[1].Equals(ClientConsts.ADD_SIGNAL) ? ClientConsts.FOLLOWED_ICON : ClientConsts.UNFOLLWED_ICON,
                        Username = elements[0],
                        DisplayText = elements[1].Equals(ClientConsts.ADD_SIGNAL) ? NotificationPage.FOLLOW_TEXT : NotificationPage.UNFOLLOW_TEXT
                    };
                    Notifications.Add(notification);
                }
            }
        }
    }

    public class Notification
    {
        public string Icon { get; set; }
        public string Username { get; set; }
        public string DisplayText { get; set; }
    }

}
