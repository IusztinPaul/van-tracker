using Amazon.Runtime;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.Extensions;
using TrackApp.DataFormat;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.Maper.Group
{
    public class SingleRouteViewModel : BasicRefreshingModelView
    {
        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged("Country");
            }
        }

        private string _region;
        public string Region
        {
            get => _region;
            set
            {
                _region = value;
                OnPropertyChanged("Region");
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged("City");
            }
        }

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

        private RouteInfo routeInfo;

        public SingleRouteViewModel(RouteInfo routeInfo) : base(null)
        {
            this.routeInfo = routeInfo;

            Country = "Se incarca...";
            Region = "Se incarca...";
            City = "Se incarca...";
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
                //grab routes
                var routeId = routeInfo.RouteId + ClientConsts.CONCAT_SPECIAL_CHARACTER + routeInfo.Count; // groupname#username#count
                var routeData = await QueryRoute.QueryRoutes(routeId, routeInfo.CountRouteAddresses);

                //update view
                if (routeData != null)
                {
                    AddressesList = new ObservableCollection<Route>(routeData);

                    if (AddressesList.Count >= 1)
                    {
                        Country = AddressesList[0].Location.Country;
                        Region = AddressesList[0].Location.Region;
                        City = AddressesList[0].Location.City;
                    }
                    else
                    {
                        Country = "Nu exista";
                        Region = "Nu exista";
                        City = "Nu exista";
                    }
                    
                }
                else
                {
                    AddressesList = new ObservableCollection<Route>();
                    Country = "Nu exista";
                    Region = "Nu exista";
                    City = "Nu exista";
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
}
