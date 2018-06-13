using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using TrackApp.ClientLayer.Extensions;

namespace TrackApp.ClientLayer.Friends
{
    public class FriendsListViewModel : AbstractFriendsViewModel
    {

        //private fields
        private List<TrackUser> _allUsers; //list that stores all users


        //observable list
        private ObservableCollection<TrackUser> _trackUsers; //bindable list that is filtered by the search bar
        public ObservableCollection<TrackUser> TrackUsers
        {
            get => _trackUsers; set
            {
                _trackUsers = value;
                OnPropertyChanged("TrackUsers");
            }
        }


        public FriendsListViewModel(TrackUser currentUser) : base(currentUser)
        {

            //add commands
            OnButtonTappedCommand = new Command(OnButtonTapped);
            OnRefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(async () => await PopulateAsync()),
                () => !IsBusy); // repopulate the data, refresh command for the list
        }


        public async override Task PopulateAsync()
        {

            // if already refreshing don't populate
            if (IsBusy)
                return;

            //set the refresh state so another refresh wont be possible
            IsBusy = true;
            ((Command)OnRefreshCommand).ChangeCanExecute();

            try

            {
                currentUserFriends = await new QueryUser().LoadData<UserFriends>(currentUser.Username);

                var list = await QueryUser.ScanAllTrackUsers();

                if (currentUserFriends != null && currentUserFriends.Friends != null)
                    _allUsers = new List<TrackUser>(FilterAllUsers(list, currentUserFriends)); //filter data
                else
                    _allUsers = new List<TrackUser>(list.Where((x) => x.Username != currentUser.Username));
                // filter only for not displaying the current user

                _allUsers.Sort((a, b) => a.Username.ToUpper().CompareTo(b.Username.ToUpper())); // sort stored list

                TrackUsers = new ObservableCollection<TrackUser>(_allUsers); //bind listview

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
                //allow another refreshes
                IsBusy = false;
                ((Command)OnRefreshCommand).ChangeCanExecute();
            }

            Console.Write("FINISHED");

        }

        private async void OnButtonTapped(object obj)
        {

            if (!IsButtonTapped) //if button is not tapped continue logic
            {
                IsButtonTapped = true;

                var item = obj as TrackUser;
                if (item != null)
                {

                    //send message to the user that the logic started
                    DependencyService.Get<IMessage>().ShortAlert(ClientConsts.REQUEST_MESSAGE);

                    try
                    {
                        await Task.Run(async () =>
                        {
                            await OnButtonTappedAsync(item);
                        });

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
                        IsButtonTapped = false; //release the button in any situation
                    }

                    //if everything went ok send a message to the user
                    DependencyService.Get<IMessage>().ShortAlert(ClientConsts.ACHIEVED_MESSAGE);
                }
            }
        }

        private async Task OnButtonTappedAsync(TrackUser selectedUser)
        {

            //first refresh the view
            if(TrackUsers.IndexOf(selectedUser) != -1)
                TrackUsers.Remove(selectedUser);

            //delete it also from the cached list
            if(TrackUsers.IndexOf(selectedUser) != -1)
                _allUsers.Remove(selectedUser);

            var query = new QueryUser();

            // add id in the current currentUser friendlist
            var userList = await query.LoadData<UserFriends>(currentUser.Username);

            if (userList != null && userList.Friends != null)
            {
                userList.Friends.Add(selectedUser.Username);
            }
            else
            {
                if (userList == null)
                    userList = new UserFriends { Username = currentUser.Username, Notifications = currentUserFriends?.Notifications };

                if (userList != null && userList.Friends == null)
                    userList.Friends = new List<string> { selectedUser.Username };
            }

            // add id in the selected currentUser notifications
            var selectedUserList = await query.LoadData<UserFriends>(selectedUser.Username);



            string notifStorage = currentUser.Username +
                                  ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                  ClientConsts.ADD_SIGNAL;

            // the notification are stored with the following form: username#add
            if (selectedUserList != null && selectedUserList.Notifications != null)
            {
                //add the real index of the item so we will display the items in the ordered they where 
                //really added (dynamodb sorts the elements inside the db)
                selectedUserList.Notifications.AddIndexedString(notifStorage);


                //resize the notifications if the length exceeded the desired value
                selectedUserList.Notifications = selectedUserList.Notifications.ResizeIfNeeded(NotificationPage.NOTIFICATION_MAX_NUMBER, 2, sort: true);
            }
            else
            {
                if (selectedUserList == null)
                    selectedUserList = new UserFriends { Username = selectedUser.Username, Friends = selectedUserList?.Friends };

                if (selectedUserList != null && selectedUserList.Notifications == null)
                    selectedUserList.Notifications = new List<string> { notifStorage + ClientConsts.CONCAT_SPECIAL_CHARACTER + "0" };
            }

            // save both UserFriends objects
            await FriendsBatchSaver.SaveUserFriends(userList, selectedUserList);

            //refresh the current user friends list
            currentUserFriends = await new QueryUser().LoadData<UserFriends>(currentUser.Username);

        }





        public IList<TrackUser> GetAllUsers()
        {
            if (_allUsers != null)
                return _allUsers;
            else return new List<TrackUser>();
        }

        private IEnumerable<TrackUser> FilterAllUsers(IEnumerable<TrackUser> allUsers, UserFriends friends)
        {
            if (allUsers != null)
            {
                if (friends == null)
                    // it means we have no friends list to filter
                    return allUsers.Where(x => x.Username != currentUser.Username);

                // display only users that are not already friends of the current user 
                // and users that are not the current user
                return allUsers.Where(x => !friends.Friends.Contains(x.Username) && x.Username != currentUser.Username);
            }

            return null;
        }


    }
}
