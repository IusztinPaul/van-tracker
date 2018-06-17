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
using TrackApp.DataFormat;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group.MapN
{
    public class ActiveRouteViewModel : BasicRefreshingModelView
    {

        private bool _showNoActiveRouteAlert;
        public bool ShowNoActiveRouteAlert
        {
            get => _showNoActiveRouteAlert;
            set
            {
                if (value != _showNoActiveRouteAlert)
                {
                    _showNoActiveRouteAlert = value;
                    OnPropertyChanged("ShowNoActiveRouteAlert");
                }
            }
        }

        private int _listViewDriverHeight;
        public int ListViewDriverHeight
        {
            get => _listViewDriverHeight;
            set
            {
                if (value != _listViewDriverHeight)
                {
                    _listViewDriverHeight = value;
                    OnPropertyChanged("ListViewDriverHeight");
                }
            }
        }

        private bool _administratorShowList;
        public bool AdministratorShowList
        {
            get => _administratorShowList;
            set
            {
                if (value != _administratorShowList)
                {
                    _administratorShowList = value;
                    OnPropertyChanged("AdministratorShowList");
                }
            }
        }

        private bool _driverShowList;
        public bool DriverShowList
        {
            get => _driverShowList;
            set
            {
                if (value != _driverShowList)
                {
                    _driverShowList = value;
                    OnPropertyChanged("DriverShowList");
                }
            }
        }

        private int _listViewAdministratorHeight;
        public int ListViewAdministratorHeight
        {
            get => _listViewAdministratorHeight;
            set
            {
                if (value != _listViewAdministratorHeight)
                {
                    _listViewAdministratorHeight = value;
                    OnPropertyChanged("ListViewAdministratorHeight");
                }
            }
        }

        private List<Route> _allAddressesList; //keep all data cached for searchbar logic
        private ObservableCollection<Route> _addressesList;
        public ObservableCollection<Route> AddressesList
        {
            get => _addressesList;
            set
            {
                _addressesList = value;
                OnPropertyChanged("AddressesList");
            }
        }

        private List<RouteInfo> _allAdministratorRouteInfoView;  //keep all data cached for searchbar logic
        private ObservableCollection<RouteInfo> _administratorRouteInfoView;
        public ObservableCollection<RouteInfo> AdministratorRouteInfoListView
        {
            get => _administratorRouteInfoView;
            set
            {
                _administratorRouteInfoView = value;
                OnPropertyChanged("AdministratorRouteInfoListView");
            }
        }

        private readonly string groupName;
        private readonly RoledTrackUser currentUser;

        public ActiveRouteViewModel(RoledTrackUser currentUser, string groupName) : base(currentUser)
        {
            this.groupName = groupName;
            this.currentUser = currentUser;
            _allAddressesList = new List<Route>();
            _allAdministratorRouteInfoView = new List<RouteInfo>();

            AdministratorRouteInfoListView = new ObservableCollection<RouteInfo>();
            AddressesList = new ObservableCollection<Route>();
            ShowNoActiveRouteAlert = false;
            ListViewAdministratorHeight = 0;
            ListViewDriverHeight = 0;
            AdministratorShowList = false;
            DriverShowList = false;
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
                //grab group
                var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);

                if ((currentUser as RoledTrackUser).Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
                {
                    ListViewAdministratorHeight = -1;
                    ListViewDriverHeight = 0;

                    AdministratorShowList = true;
                    DriverShowList = false;

                    await PopulateAsyncForAdministrator(group);

                }
                else if ((currentUser as RoledTrackUser).Role.Equals(RoledTrackUser.TYPE_DRIVER))
                {
                    ListViewAdministratorHeight = 0;
                    ListViewDriverHeight = -1;

                    AdministratorShowList = false;
                    DriverShowList = true;

                    await PopulateAsyncForDriver(group);
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

        private async Task PopulateAsyncForAdministrator(DataFormat.Group group)
        {
            //get data
            if (group.ActiveDriverRoutes != null && group.ActiveDriverRoutes.Count > 0)
            {
                var listOfRouteInfos = new List<RouteInfo>();
                foreach (var activeRoute in group.ActiveDriverRoutes)
                {
                    var items = activeRoute.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]);
                    var routeInfo = new RouteInfo
                    {
                        RouteId = groupName + ClientConsts.CONCAT_SPECIAL_CHARACTER + items[0],
                        Count = Int32.Parse(items[1])
                    };
                    listOfRouteInfos.Add(routeInfo);
                }

                if (listOfRouteInfos.Count > 0)
                {
                    //get list of routes info with full data and sort it
                    listOfRouteInfos = await QueryRoute.QueryMultipleRoutesInfo(listOfRouteInfos);
                    listOfRouteInfos.Sort((a, b) => a.OwnerUsername.CompareTo(b.OwnerUsername));

                    //map colours by sorting users by username
                    for(int i = 0; i < listOfRouteInfos.Count; i++)
                        listOfRouteInfos[i].UserColor = ClientConsts.Colours[i];

                    //bind view
                    AdministratorRouteInfoListView = new ObservableCollection<RouteInfo>(listOfRouteInfos);
                    _allAdministratorRouteInfoView = new List<RouteInfo>(listOfRouteInfos);
                }
                else //no data to show
                {
                    ShowNoActiveRouteAlert = true;

                    ListViewAdministratorHeight = 0;
                    AdministratorShowList = false;
                }
            }
            else //no data to show
            {
                ShowNoActiveRouteAlert = true;

                ListViewAdministratorHeight = 0;
                AdministratorShowList = false;
            }
        }

        private async Task PopulateAsyncForDriver(DataFormat.Group group)
        {
            string activeRoute = "";
            //grab the current user active group

            if (group != null && group.ActiveDriverRoutes != null)
            {
                foreach (var activeR in group.ActiveDriverRoutes)
                    if ((activeR.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0]).Trim().Equals(currentUser.Username))
                    {
                        activeRoute = activeR;
                        break;
                    }
            }

            if (!activeRoute.Equals(""))
            {
                ShowNoActiveRouteAlert = false;

                var routeInfoCount = Int32.Parse(activeRoute.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[1]);

                var routeInfo = await QueryRoute.QuerySingleRouteInfo(currentUser.Username, groupName, routeInfoCount);

                if (routeInfo != null)
                {
                    var route = await QueryRoute.QueryRoutes(currentUser.Username, groupName, routeInfoCount.ToString(), routeInfo.CountRouteAddresses);

                    if (route != null)
                    {
                        AddressesList = new ObservableCollection<Route>(route);
                        _allAddressesList = new List<Route>(route);
                    }
                }

            }
            else // no active route found
            {
                ShowNoActiveRouteAlert = true;

                ListViewDriverHeight = 0;
                DriverShowList = false;
            }
        }


        public void SearchBarListener(string searchBarText)
        {
            if (_allAddressesList == null)
                _allAddressesList = new List<Route>();

            if (_allAdministratorRouteInfoView == null)
                _allAdministratorRouteInfoView = new List<RouteInfo>();

            if (currentUser.Role.Equals(RoledTrackUser.TYPE_ADMINISTRATOR))
            {
                if (String.IsNullOrEmpty(searchBarText))
                {
                    AdministratorRouteInfoListView = new ObservableCollection<RouteInfo>(_allAdministratorRouteInfoView);
                }
                else
                {
                    AdministratorRouteInfoListView = new ObservableCollection<RouteInfo>(
                        _allAdministratorRouteInfoView.Where((x) =>
                        x.RouteName.ToUpper().StartsWith(searchBarText.ToUpper()) ||
                        x.OwnerUsername.ToUpper().StartsWith(searchBarText.ToUpper()) ||
                        searchBarText.ToUpper().Contains(x.RouteName.ToUpper()) ||
                        searchBarText.ToUpper().Contains(x.OwnerUsername.ToUpper())
                        ));
                }
            }
            else if (currentUser.Role.Equals(RoledTrackUser.TYPE_DRIVER))
            {
                if (String.IsNullOrEmpty(searchBarText))
                {
                    AddressesList = new ObservableCollection<Route>(_allAddressesList);
                }
                else
                {
                    AddressesList = new ObservableCollection<Route>(
                        _allAddressesList.Where((x) =>
                        x.Location.Street.ToUpper().StartsWith(searchBarText.Trim().ToUpper()) || 
                        x.Location.Nr.StartsWith(searchBarText.Trim()) || 
                        searchBarText.ToUpper().Contains(x.Location.Street.ToUpper()) ||
                        searchBarText.Contains(x.Location.Nr)
                        ));
                }
            }
            else throw new Exception("There is no other possible role!");
        }

        public void ChangeAddressToTrue(Route route)
        {
            if (AddressesList != null)
            {
               AddressesList.Remove(route);
            }
        }
        
    }
}
