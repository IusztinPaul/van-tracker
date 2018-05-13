using System;
using TrackApp.DataFormat.UserData;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using TrackApp.ServerLayer.Query;
using TrackApp.ClientLayer.Extensions;
using System.Net;
using TrackApp.ClientLayer.Exceptions;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using System.Collections.Generic;

namespace TrackApp.ClientLayer.Maper
{
    public class MapGroupsViewModel : BasicRefreshingModelView
    {
        public const string ADMINISTRATOR_DETAIL = "Administrator";
        public const string DRIVER_DETAIL = "Sofer";

        private ObservableCollection<GroupListViewWrapper> _groups;
        public ObservableCollection<GroupListViewWrapper> Groups
        {
            get => _groups;
            set
            {

                _groups = value;
                OnPropertyChanged("Groups");
            }
        }

        public MapGroupsViewModel(TrackUser currentUser) : base(currentUser)
        {
           
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
                Groups = new ObservableCollection<GroupListViewWrapper>();
            
            // query and add data to Groups list
            var userFriends = await QueryHashLoader.LoadData<UserFriends>(currentUser.Username);
            if (userFriends != null)
            {
                foreach (var group in userFriends.Groups)
                {
                    string[] data = group.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                    if (data.Length >= 2)
                    {
                            var listItem = new GroupListViewWrapper
                            {
                                Name = data[0],
                                Type = data[1] == ClientConsts.ADMINISTRATOR_SIGNAL ? ADMINISTRATOR_DETAIL : DRIVER_DETAIL
                            };

                            Groups.Add(listItem);
                    }
                }

                    //sort by name
                    Groups.Sort<GroupListViewWrapper>((a, b) => a.Name.CompareTo(b.Name));
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

    }

    public class GroupListViewWrapper
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string UserTypeIcon {
            get {
                if (Type != null && Type.Equals(MapGroupsViewModel.ADMINISTRATOR_DETAIL))
                    return ClientConsts.ADMINISTRATOR_ICON;

                if (Type != null && Type.Equals(MapGroupsViewModel.DRIVER_DETAIL) )
                    return ClientConsts.DRIVER_ICON;

                return ClientConsts.APP_LOGO;
            } }
    }
}
