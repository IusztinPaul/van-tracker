using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.Extensions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group
{
    public class GroupNotificationPageViewModel : BasicRefreshingModelView
    {
        private string groupName;

        private ObservableCollection<GroupNotification> _groupNotifications;
        public ObservableCollection<GroupNotification> GroupNotifications
        {
            get => _groupNotifications;
            set
            {

                _groupNotifications = value;
                OnPropertyChanged("GroupNotifications");
            }
        }

        public GroupNotificationPageViewModel(TrackUser currentUser, string groupName) : base(currentUser)
        {
            this.groupName = groupName;
        }



        public async override Task PopulateAsync()
        {

            // if already refreshing don't populate
            if (IsBusy)
                return;

            //set the refresh state so another refresh wont be possible
            IsBusy = true;
            (OnRefreshCommand as Command)?.ChangeCanExecute();

            try
            {
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);

                GroupNotifications = new ObservableCollection<GroupNotification>();

                if (group != null)
                {
                    foreach (string notification in group.Notifications)
                    {
                        var items = notification.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);

                        var notif = new GroupNotification
                        {
                            Username = items?[0],
                            Type = items?[1],
                            Index = items?[2]
                        };

                        GroupNotifications.Add(notif);
                    }

                    //sort by index
                    GroupNotifications.Sort<GroupNotification>((a, b) => -a.Index.CompareTo(b.Index));
                }
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
                Console.WriteLine(e.Message);
            }
            catch (ValidationException e) // show error message to the user
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
                Console.WriteLine(e.Message);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
            }
            catch (Exception e) // in case of unexpected error like Error: NameResolutionFailure
            {
                Console.WriteLine("EXCEPTION COUGHT {0} ", e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
            finally
            {
                //allow another refreshes
                IsBusy = false;
                (OnRefreshCommand as Command)?.ChangeCanExecute();
            }
        }


        public class GroupNotification
        {
            public string Username { get; set; }
            public string Type { get; set; } // uses Group class consts states: ACCEPTED_REQUEST_STATE/DENIED_REQUEST_STATE
            public string Index { get; set; }
            public string Icon
            {
                get
                {

                    if (Type.Equals(DataFormat.Group.ACCEPTED_REQUEST_STATE))
                        return ClientConsts.FOLLOWED_ICON;

                    if (Type.Equals(DataFormat.Group.DENIED_REQUEST_STATE))
                        return ClientConsts.UNFOLLWED_ICON;

                    return ClientConsts.USER_PLACEHOLDER;
                }
            }

            public string TypeDisplay
            {
                get {
                    if (Type.Equals(DataFormat.Group.ACCEPTED_REQUEST_STATE))
                        return "a acceptat invitatia";

                    if (Type.Equals(DataFormat.Group.DENIED_REQUEST_STATE))
                        return "a respins invitatia";

                    return "";
                }
            }
        }
    }
}
