using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using TrackApp.ClientLayer.Extensions;
using Xamarin.Forms;
using System.Net;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.CustomUI;
using Amazon.Runtime;

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

        public NotificationsModelView(TrackUser currentUser) : base(currentUser)
        {

            OnRefreshCommand = new Command(async () => await PopulateAsync(), () => !IsBusy);
        }

        public async override Task PopulateAsync()
        {

            if (IsBusy)
                return;

            IsBusy = true;
            (OnRefreshCommand as Command)?.ChangeCanExecute();

            try
            {
                await PopulateAsyncLogic();
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
            }
            catch (ValidationException e) // display error message to currentUser
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
            }
            catch (WebException e)
            {
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error 
            {
                Console.WriteLine("EXCEPTION COUGHT: " + e.Message);
                Console.WriteLine("TYPE: " + e.GetType());
                DependencyService.Get<IMessage>().LongAlert(e.Message);
            }
            finally
            {

                IsBusy = false;
                (OnRefreshCommand as Command)?.ChangeCanExecute();
            }
        }

        private async Task PopulateAsyncLogic()
        {
            currentUserFriends = await new QueryUser().LoadData<UserFriends>(currentUser.Username);

            Notifications = new ObservableCollection<Notification>();

            if (currentUserFriends != null && currentUserFriends.Notifications != null)
            {

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
