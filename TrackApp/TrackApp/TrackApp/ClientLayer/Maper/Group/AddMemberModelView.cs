using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group
{
    public class AddMemberModelView : BasicRefreshingModelView
    {

        public const string DATA_ADD_PROCESS_MESSAGE = "Asteptati ca cererea sa fie procesata!";
        public const string USERNAME_TRACKUSER_TO_FILTER = "Filter";

        private string groupName;
        private AddMemberPage addMemberPage;

        private List<TrackUser> allUsers;

        private ObservableCollection<TrackUser> _users;
        public ObservableCollection<TrackUser> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }

        public ICommand OnButtonTappedCommand { get; protected set; }

        public AddMemberModelView(RoledTrackUser currentUser, string groupName, AddMemberPage addMemberPage) : base(currentUser)
        {
            this.groupName = groupName;
            this.addMemberPage = addMemberPage;

            OnButtonTappedCommand = new Command(async (object item) => await OnButtonTappedCommandAsync(item));
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
                var userFriends = await QueryHashLoader.LoadData<UserFriends>(currentUser.Username);

                var asyncTasks = new List<Task<TrackUser>>();
                foreach (string friendUsername in userFriends.Friends)
                {
                    var task = Task.Run(async () =>
                    {
                        return await QueryHashLoader.LoadData<TrackUser>(friendUsername);

                    });
                    asyncTasks.Add(task);
                }

                //wait for all the tasks to complete their work
                var users = await Task.WhenAll(asyncTasks);

                //add data and filter it 
                IEnumerable<TrackUser> allUsersEnumarble = new List<TrackUser>(users);
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);

                if (group != null) // filter the users that already are in the group or have a request
                    allUsersEnumarble = allUsersEnumarble.Where(x =>
                    {

                        if (group.Admins.Contains(x.Username) || x.Username.Equals(USERNAME_TRACKUSER_TO_FILTER))
                            return false;

                        if (group.Drivers != null && group.Drivers.Contains(x.Username))
                            return false;

                        return true;
                    });


                //bind view and sort it by Username
                if (allUsersEnumarble != null)
                {
                    allUsers = new List<TrackUser>(allUsersEnumarble);
                    allUsers.Sort((a, b) => a.Username.ToUpper().CompareTo(b.Username.ToUpper()));

                    Users = new ObservableCollection<TrackUser>(allUsers);
                }
                else
                {
                    Users = new ObservableCollection<TrackUser>();
                    allUsers = new List<TrackUser>();
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


        protected async Task OnButtonTappedCommandAsync(object item)
        {
            try
            {
                bool alreadyInvited = false;

                //check if it already has a request
                var user = item as TrackUser;
                if (user != null)
                {
                    var userTappedFriends = await QueryHashLoader.LoadData<UserFriends>(user.Username);
                    if (userTappedFriends != null && userTappedFriends.GroupRequests != null)
                        foreach (var request in userTappedFriends.GroupRequests)
                        {
                            var groupInvitied = request.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0];
                            Console.WriteLine("GROUP INVITED: {0} AND GROUP NAME: {1}", groupInvitied, groupName);
                            if (groupInvitied.Equals(groupName))
                            {
                                alreadyInvited = true;
                                break;
                            }
                        }
                }


                if (!alreadyInvited)
                {
                    var answer = await addMemberPage.DisplayAlertOnAddButtonClicked();

                    if (answer) // continue logic only if the user answered positivley 
                    {

                        DependencyService.Get<IMessage>().ShortAlert(DATA_ADD_PROCESS_MESSAGE);
                        addMemberPage.OnAddButtonClicked();

                        if (user != null)
                        {

                            //query data
                            var userFriends = await QueryHashLoader.LoadData<UserFriends>(user.Username);

                            if (userFriends == null)
                                userFriends = new UserFriends() { Username = user.Username };

                            if (userFriends != null && userFriends.GroupRequests == null)
                                userFriends.GroupRequests = new List<string>();

                            // add request
                            // format -> groupname#username#type(administrator/driver)#index  
                            string groupRequest = groupName + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                    currentUser.Username + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                    addMemberPage.GetCurrentState() + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                                    userFriends.GroupRequests.Count;

                            userFriends.GroupRequests.Add(groupRequest);

                            // save data
                            var saver = new SaveUserFriends { UserFriends = userFriends };
                            await saver.SaveData();

                            //go back to the choice buttons
                            addMemberPage.SwitchDisplayView();

                        }
                    }
                }
                else // send the user some message
                {
                    await addMemberPage.DisplayAlert(AddMemberPage.DISPLAY_ALERT_ATTENTION_TITLE,
                        AddMemberPage.DISPLAY_ALERT_ATTENTION_CONTENT, "Ok");
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
        }

        public void SearchBarListener(string searchBarText)
        {
            if (allUsers == null)
                allUsers = new List<TrackUser>();

            if (String.IsNullOrEmpty(searchBarText))
            {
                Users = new ObservableCollection<TrackUser>(allUsers);
            }
            else
            {
                Users = new ObservableCollection<TrackUser>(
                    allUsers.Where((x) => x.Username.ToUpper().StartsWith(searchBarText.ToUpper()))
                    );
            }

        }
    }

}

