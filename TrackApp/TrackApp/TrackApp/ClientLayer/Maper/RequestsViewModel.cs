using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.Extensions;
using TrackApp.ClientLayer.Maper.Group;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper
{
    public class RequestsViewModel : BasicRefreshingModelView
    {
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

        public ICommand AcceptRequestCommand { get; protected set; }
        public ICommand DenyRequestCommand { get; protected set; }

        public RequestsViewModel(TrackUser currentUser, RequestsPage requestsPage) : base(currentUser)
        {
            AcceptRequestCommand = new Command(async (object item) => 
                {
                    requestsPage.StartActivityIndicator();
                    await AcceptRequestCommandAsync(item);
                    requestsPage.PauseActivityIndicator();
                });
            DenyRequestCommand = new Command(async (object item) =>
            {
                requestsPage.StartActivityIndicator();
                await DenyRequestCommandAsync(item);
                requestsPage.PauseActivityIndicator();
            });
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

                GroupRequests = new ObservableCollection<RequestNotification>();
                var currentUserFriends = await QueryHashLoader.LoadData<UserFriends>(currentUser.Username);

                if (currentUserFriends != null && currentUserFriends.GroupRequests != null)
                {

                    foreach (string request in currentUserFriends.GroupRequests)
                    {
                        var items = request.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                        // groupname#username#type(administrator/driver)#index  
                        var requestNotification = new RequestNotification
                        {
                            Username = items?[1],
                            GroupName = items?[0],
                            Type = items[2].Equals(AddMemberPage.ADMINISTRATOR_STATE_DB) ? AddMemberPage.ADMINISTRATOR_STATE_DISPLAY : AddMemberPage.DRIVER_STATE_DISPLAY,
                            Index = Int32.Parse(items[3])
                        };

                        GroupRequests.Add(requestNotification);
                    }

                    GroupRequests.Sort<RequestNotification>((a, b) => -a.Index.CompareTo(b.Index));

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

        private async Task AcceptRequestCommandAsync(object item)
        {
            var notif = item as RequestNotification;
            if (notif != null)
            {

                try
                {
                    
                    var group = await QueryHashLoader.LoadData<DataFormat.Group>(notif.GroupName);

                    if (group != null) {

                        // add member to group logic
                        if (notif.Type == AddMemberPage.ADMINISTRATOR_STATE_DISPLAY)
                            if (group.Admins != null)
                                group.Admins.Add(currentUser.Username);
                            else
                                group.Admins = new List<string> { currentUser.Username };

                        else if (notif.Type.Equals(AddMemberPage.DRIVER_STATE_DISPLAY))
                            if (group.Drivers != null)
                                group.Drivers.Add(currentUser.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER + "0"); // username#numberOfRoutesInGroup
                            else
                                group.Drivers = new List<string> { currentUser.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER + "0" };
                        else
                            throw new Exception("The user has a type that does not exist!");


                        //add group notification logic

                        if (group.Notifications == null)
                            group.Notifications = new List<string>();

                        var groupNotification = currentUser.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                DataFormat.Group.ACCEPTED_REQUEST_STATE + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                group.Notifications.Count;

                        group.Notifications.Add(groupNotification);
                        //don't hold more than a constant number of notifications
                        group.Notifications = group.Notifications.ResizeIfNeeded(RequestsPage.MAX_SIZE_REQUEST_NOTIFICATIONS, 2, sort: true);

                        //save data to database
                        var saver = new SaveGroup { Group = group };
                        await saver.SaveData();
                    }
                    else
                    {
                        throw new Exception("Invited to a group that does not exist!");
                    }

                    //delete grouprequest from current userfriends and add reference to the group
                    var currentUserFriends = await QueryHashLoader.LoadData<UserFriends>(currentUser.Username);

                    if(currentUserFriends != null)
                    {
                        string dbType = notif.Type == AddMemberPage.ADMINISTRATOR_STATE_DISPLAY ? AddMemberPage.ADMINISTRATOR_STATE_DB : AddMemberPage.DRIVER_STATE_DB;
                        string requestToDelete = notif.GroupName + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                notif.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                dbType + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                notif.Index;

                        if (currentUserFriends.GroupRequests.Count == 1)
                        {
                            currentUserFriends.GroupRequests.Clear();
                            await ISaveData.DeleteOnlyHashKeyData<UserFriends>(currentUser.Username); // delete it first otherwise if only one item remains in the request list it wont be delted from the db
                        }
                        else
                        {
                            currentUserFriends.GroupRequests.Remove(requestToDelete);

                            //map indexes
                            for(int i = 0; i < currentUserFriends.GroupRequests.Count; i++)
                            {
                                var items = currentUserFriends.GroupRequests[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                                var index = Int32.Parse(items[items.Length - 1]);

                                if (index > notif.Index) // subtract -1 from all the indexes that are greater than the one that it was deleted
                                    index -= 1;

                                items[items.Length - 1] = index.ToString(); // change the index
                                //recreate the element
                                currentUserFriends.GroupRequests[i] = String.Join(ClientConsts.CONCAT_SPECIAL_CHARACTER, items);
                            }
                        }

                        //add group
                        if (currentUserFriends.Groups == null)
                            currentUserFriends.Groups = new List<string>();

                        string groupStringType = notif.GroupName + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                            (notif.Type == AddMemberPage.ADMINISTRATOR_STATE_DISPLAY ? ClientConsts.ADMINISTRATOR_SIGNAL : ClientConsts.DRIVER_SIGNAL);

                        currentUserFriends.Groups.Add(groupStringType);

                        //save data to database
                        var saver = new SaveUserFriends { UserFriends = currentUserFriends};
                        await saver.SaveData();

                    } else
                    {
                        throw new Exception("Request from no userfriends objecte ?!?!?!?");
                    }

                    //remake the view
                    GroupRequests.Remove(notif);
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
            }
        }

        private async Task DenyRequestCommandAsync(object item)
        {
            var notif = item as RequestNotification;
            if (notif != null)
            {

                try
                {

                    var group = await QueryHashLoader.LoadData<DataFormat.Group>(notif.GroupName);

                    if (group != null)
                    {
                        //add group notification logic

                        if (group.Notifications == null)
                            group.Notifications = new List<string>();

                        var groupNotification = currentUser.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                DataFormat.Group.DENIED_REQUEST_STATE + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                group.Notifications.Count;

                        group.Notifications.Add(groupNotification);
                        //don't hold more than a constant number of notifications
                        group.Notifications = group.Notifications.ResizeIfNeeded(RequestsPage.MAX_SIZE_REQUEST_NOTIFICATIONS, 2, sort: true);

                        //save data to database
                        var saver = new SaveGroup { Group = group };
                        await saver.SaveData();
                    }
                    else
                    {
                        throw new Exception("Invited to a group that does not exist!");
                    }

                    //delete grouprequest from current userfriends
                    var currentUserFriends = await QueryHashLoader.LoadData<UserFriends>(currentUser.Username);

                    if (currentUserFriends != null)
                    {
                        string dbType = notif.Type == AddMemberPage.ADMINISTRATOR_STATE_DISPLAY ? AddMemberPage.ADMINISTRATOR_STATE_DB : AddMemberPage.DRIVER_STATE_DB;
                        string requestToDelete = notif.GroupName + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                notif.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                dbType + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                notif.Index;

                        if (currentUserFriends.GroupRequests.Count == 1)
                        {
                            currentUserFriends.GroupRequests.Clear();
                            await ISaveData.DeleteOnlyHashKeyData<UserFriends>(currentUser.Username); // delete it first otherwise if only one item remains in the request list it wont be delted from the db
                        }
                        else
                        {
                            currentUserFriends.GroupRequests.Remove(requestToDelete);

                            //map indexes
                            for (int i = 0; i < currentUserFriends.GroupRequests.Count; i++)
                            {
                                var items = currentUserFriends.GroupRequests[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                                var index = Int32.Parse(items[items.Length - 1]);

                                if (index > notif.Index) // subtract -1 from all the indexes that are greater than the one that it was deleted
                                    index -= 1;

                                items[items.Length - 1] = index.ToString(); // change the index
                                //recreate the element
                                currentUserFriends.GroupRequests[i] = String.Join(ClientConsts.CONCAT_SPECIAL_CHARACTER, items);
                            }
                        }

                        //save data to database
                        var saver = new SaveUserFriends { UserFriends = currentUserFriends };
                        await saver.SaveData();

                    }
                    else
                    {
                        throw new Exception("Request from no userfriends objecte ?!?!?!?");
                    }

                    //remake the view
                    GroupRequests.Remove(notif);
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
            }

        }
    }


        public class RequestNotification
        {
            public string Username { get; set; } // from who you get the notification
            public string GroupName { get; set; } // the group that you are invited to
            public string Type { get; set; } // administrator or sofer ( it is mapped from the database to romanian)
            public int Index { get; set; }
            public string Icon { get
                {
                    if (Type.Equals(AddMemberPage.ADMINISTRATOR_STATE_DISPLAY))
                        return ClientConsts.ADMINISTRATOR_ICON;

                    if (Type.Equals(AddMemberPage.DRIVER_STATE_DISPLAY))
                        return ClientConsts.DRIVER_ICON;

                    return ClientConsts.USER_PLACEHOLDER;
                }
            }

            public string BottomDisplayText
            {
                get
                {
                    return String.Format("{0} ca si {1}", GroupName, Type);
                }
            }

            public ImageSource ButtonIconAccept { get => ImageSource.FromFile(ClientConsts.THICK_ICON); }
            public ImageSource ButtonIconDeny { get => ImageSource.FromFile(ClientConsts.X_ICON); }
        }
}
