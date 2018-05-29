using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using TrackApp.ClientLayer.Extensions;
using TrackApp.ServerLayer.Save;

namespace TrackApp.ClientLayer.Friends
{
    public class CurrentFriendsListViewModel : AbstractFriendsViewModel
    {
        public IList<TrackUser> AllCurrentFriends { get; set; }

        public ObservableCollection<TrackUser> _currentUserFriends;
        public ObservableCollection<TrackUser> CurrentUserFriends
        { get => _currentUserFriends;
            set
            {
                _currentUserFriends = value;
                OnPropertyChanged("CurrentUserFriends");
            }
        }

        public CurrentFriendsListViewModel(TrackUser currentUser) : base(currentUser)
        { 
            //set commands
            this.OnButtonTappedCommand = new Command(OnButtonUnfollowCommand);
            OnRefreshCommand = new Command(() => Device.BeginInvokeOnMainThread(async () => await PopulateAsync()), () => !IsBusy);
        }


         public override async Task PopulateAsync()
        {

            // if already refreshing don't populate
            if (IsBusy)
                return;

            //set the refresh state so another refresh wont be possible
            IsBusy = true;
            (OnRefreshCommand as Command)?.ChangeCanExecute();

            try
            {
                //query for the id list of friends
                currentUserFriends = await new QueryUser().LoadData<UserFriends>(currentUser.Username);

                if (currentUserFriends != null && currentUserFriends.Friends != null)
                {
                    // query users in parallel
                    var tasks = new List<Task<TrackUser>>();
                    foreach (string username in currentUserFriends.Friends)
                    {
                        var task = Task.Run(async () => await new QueryUser().LoadData<TrackUser>(username));
                        tasks.Add(task);
                    }

                    // wait for all the tasks to finish
                    var users = await Task.WhenAll(tasks);

                    //refresh listview
                    CurrentUserFriends = new ObservableCollection<TrackUser>(users);

                    //sort
                    CurrentUserFriends.Sort<TrackUser>((a, b) => a.Username.ToUpper().CompareTo(b.Username.ToUpper()));

                    //store a sorted copy for search performance
                    AllCurrentFriends = new List<TrackUser>(CurrentUserFriends);
                }
                else //initialize empty lists if there are no friends
                {
                    CurrentUserFriends = new ObservableCollection<TrackUser>();
                    AllCurrentFriends = new List<TrackUser>();
                }
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
                (OnRefreshCommand as Command)?.ChangeCanExecute();

            }
        }

        private async void OnButtonUnfollowCommand(object obj)
        {
            var item = obj as TrackUser;
            if (item != null)
            {

                if (!IsButtonTapped) //if button is not tapped continue logic
                {

                    IsButtonTapped = true;

                    //send message to the user that the logic started
                    DependencyService.Get<IMessage>().ShortAlert(ClientConsts.REQUEST_MESSAGE);

                    try
                    {
                        await OnButtonUnfollowCommandAsync(item);
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

        private async Task OnButtonUnfollowCommandAsync(TrackUser clickUser)
        {

            //remove it from the cached list and binded listview
            AllCurrentFriends.Remove(clickUser);
            CurrentUserFriends.Remove(clickUser);

            //after update the currentUser obj in case it was changed from a different device
            currentUserFriends = await new QueryUser().LoadData<UserFriends>(currentUser.Username);

            //remove id from db object and after update it in the database (current user friend list)
            if (currentUserFriends.Friends != null && currentUserFriends.Friends.Count == 1)
            {
                // can't delete the last item in a list so we delete the whole row
                await ISaveData.DeleteOnlyHashKeyData<UserFriends>(currentUserFriends.Username);

                // if there are any notifications save them again
                if( (currentUserFriends.Notifications != null && currentUserFriends.Notifications.Count != 0) || 
                    (currentUserFriends.GroupRequests != null && currentUserFriends.GroupRequests.Count != 0) ||
                    (currentUserFriends.Groups != null && currentUserFriends.Groups.Count != 0) )
                {
                    var s = new SaveUserFriends { UserFriends = new UserFriends
                    {
                        Username = currentUserFriends.Username,
                        Friends = new List<string>(),
                        Notifications = currentUserFriends.Notifications,
                        GroupRequests = currentUserFriends.GroupRequests,
                        Groups = currentUserFriends.Groups
                    }
                    };
                    await s.SaveData();
                }
            }
            else
            {
                currentUserFriends?.Friends.Remove(clickUser.Username);
                var s = new SaveUserFriends { UserFriends = currentUserFriends };
                await s.SaveData();
            }


            //update notifications for the unfollowed user
            var clickedUserFriendsObj = await new QueryUser().LoadData<UserFriends>(clickUser.Username);
            var notifStorage = currentUser.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER + ClientConsts.REMOVE_SIGNAL;

            //create objects if needed
            if (clickedUserFriendsObj == null)
                clickedUserFriendsObj = new UserFriends { Username = clickUser.Username , Friends = clickedUserFriendsObj?.Friends};

            //create objects if needed            
            if(clickedUserFriendsObj != null && clickedUserFriendsObj.Notifications == null)
                clickedUserFriendsObj.Notifications = new List<string>();
            
            //add the real index of the item so we will display the items in the ordered they where 
            //really added (dynamodb sorts the elements inside the db)
            clickedUserFriendsObj.Notifications.AddIndexedString(notifStorage);

            //resize the notifications if the length exceeded the desired value
            clickedUserFriendsObj.Notifications = clickedUserFriendsObj.Notifications.ResizeIfNeeded(NotificationPage.NOTIFICATION_MAX_NUMBER, 2, sort: true);

            //save the updated obj
            var saver = new SaveUserFriends { UserFriends = clickedUserFriendsObj };
            await saver.SaveData();
        }
    }
}
