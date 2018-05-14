using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class GroupPageViewModel : BasicRefreshingModelView
    {
        private DataFormat.Group currentGroup;
        private string groupName;

        private List<RoledUsername> allMembers;

        private ObservableCollection<RoledUsername> _members;
        public ObservableCollection<RoledUsername> Members
        {
            get => _members;
            set
            {

                _members = value;
                OnPropertyChanged("Members");
            }
        }

        public GroupPageViewModel(RoledTrackUser currentUser, string groupName) : base(currentUser)
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
                currentGroup = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                Populate();
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

        private void Populate()
        {
            allMembers = new List<RoledUsername>();

            // add admins
            if (currentGroup != null && currentGroup.Admins != null)
                foreach (string username in currentGroup?.Admins)
                {
                    var member = new RoledUsername
                    {
                        Username = username,
                        Role = RoledTrackUser.TYPE_ADMINISTRATOR
                    };
                    allMembers.Add(member);
                }

            // add drivers
            if (currentGroup != null && currentGroup.Drivers != null)
                foreach (string username in currentGroup?.Drivers)
                {
                    var member = new RoledUsername
                    {
                        Username = username,
                        Role = RoledTrackUser.TYPE_DRIVER
                    };
                    allMembers.Add(member);
                }

            allMembers.Sort((a, b) => a.Username.CompareTo(b.Username));

            //bind view
            Members = new ObservableCollection<RoledUsername>(allMembers);
        }


        public void SearchBarListener(string searchBarText)
        {
            if (allMembers == null)
                allMembers = new List<RoledUsername>();

            if (String.IsNullOrEmpty(searchBarText))
            {
                Members = new ObservableCollection<RoledUsername>(allMembers);
            }
            else
            {
                Members = new ObservableCollection<RoledUsername>(
                    allMembers.Where((x) => x.Username.ToUpper().StartsWith(searchBarText.ToUpper()))
                    );
            }

        }

    }

    public class RoledUsername
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string Icon { get
            {
                if (Role == RoledTrackUser.TYPE_ADMINISTRATOR)
                    return ClientConsts.ADMINISTRATOR_ICON;

                if (Role == RoledTrackUser.TYPE_DRIVER)
                    return ClientConsts.DRIVER_ICON;

                return ClientConsts.USER_PLACEHOLDER;
            }
        }
    }
}
