using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat;
using TrackApp.ServerLayer.Query;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRoutePage : ContentPage
    {
        //second and final step of creating a route

        public const string ADDRESS_NAME_PLACEHOLDER = "Nume adresa";
        public const string ADDRESS_DETAIL_PLACEHOLDER = "numar-numar sau nr. * bl. * sc. *";
        public const string LABEL_TITLE_TEXT = "Creeaza ruta";
        public const string LABEL_DETAIL_TEXT = "Exemplu:\nNume adresa: Lugojului\nRestul pot fi completate in felul urmator:\n --- 2-10 -> toate numerele intre 2 si 10 inclusiv cu numele adresei de mai sus\n --- 1-1 adresa cu numele de mai sus si numarul 1\n --- nr. 3 bl. 5 sc. 8 -> se ia efectiv locuinta/blocul cu adresa de mai sus (datele trebuie introduse in aceasta ordine)\n";
        public const string TEXT_BUTTON_NEW_ADDRESS = "Adauga noua adresa";
        public const string TEXT_ADDRESS_NAME_EXCEPTION = "Adresa {0} nu e completata";
        public const string TEXT_ADDRESS_DETAIL_EXCEPTION = "Un detaliu de la o adresa este scris incorect";
        public const string TEXT_BUTTON_SAVE_ROUTE = "Salveaza";
        public const string TEXT_NO_DATA_TO_SAVE = "Nu exista date de salvat!";
        public const string TEXT_DATA_SAVED = "Datele au fost salvate cu succes!";

        public const string DETAIL_DATA_PATTERN = @"^\s*(nr\.(.+?))?\s*(bl\.(.+?))?\s*(sc\.(.+?))?\s*$";

        private List<List<AddressEntryAndDetailEntry>> routeData = new List<List<AddressEntryAndDetailEntry>>();
        private List<StackLayout> addressesStacks = new List<StackLayout>();

        private StackLayout stackContent;
        private Button BtnAddNewStack;

        private string groupName;
        private string driverUsername;

        private string routeName;
        private string country;
        private string region;
        private string city;

        public CreateRoutePage(string groupName, string driverUsername, string routeName, string country, string region, string city)
        {
            InitializeComponent();

            this.groupName = groupName;
            this.driverUsername = driverUsername;

            this.routeName = routeName;
            this.country = country;
            this.region = region;
            this.city = city;

            stackContent = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Margin = new Thickness(8),
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Content = new ScrollView {
                Content = stackContent,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };


            //add first address Stack
            var addressStack = new StackLayout();
            addressesStacks.Add(addressStack); // list reference

            //add main address name entry
            var addressNameEntry = new AddAddressView(ADDRESS_NAME_PLACEHOLDER);
            addressNameEntry.BtnAdd.IsVisible = false;
            addressNameEntry.LabelDelete.IsVisible = true;
            addressNameEntry.LabelDelete.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => DeleteDetailEntryButtonListener(addressStack, null))
            });
            addressStack.Children.Add(addressNameEntry);

            //create first detail Entry
            var detailEntry = new AddAddressView(ADDRESS_DETAIL_PLACEHOLDER);
            detailEntry.HorizontalOptions = LayoutOptions.StartAndExpand;
            detailEntry.BtnAdd.Clicked += (s, a) => AddDetailEntryButtonListener(addressStack, addressNameEntry.EntryAddress, s, a);

            addressStack.Children.Add(detailEntry);

            //add reference to the route data
            routeData.Add(new List<AddressEntryAndDetailEntry> { new AddressEntryAndDetailEntry
            {
                AddressEntry = addressNameEntry.EntryAddress,
                DetailEntry = detailEntry.EntryAddress
            }});


            stackContent.Children.Add(new Label
            {
                Text = LABEL_TITLE_TEXT,
                Margin = new Thickness(1),
                FontSize = 22d,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.StartAndExpand
            });

            stackContent.Children.Add(new Label
            {
                Text = LABEL_DETAIL_TEXT,
                Margin = new Thickness(2),
                FontSize = 16d,
                TextColor = Color.DarkGray,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            });

            var BtnSaveRoute = new Button()
            {
                Text = TEXT_BUTTON_SAVE_ROUTE,
                Margin = new Thickness(6, 1),
                Scale = 0.8,
                FontAttributes = FontAttributes.Bold
            };
            BtnSaveRoute.Clicked += async (source, args) => await SaveData(source, args);
            stackContent.Children.Add(BtnSaveRoute);

            stackContent.Children.Add(addressStack);
            BtnAddNewStack = new Button()
            {
                Scale = 0.8,
                Margin = new Thickness(6, 1),
                Text = TEXT_BUTTON_NEW_ADDRESS,
                FontAttributes = FontAttributes.Bold
            };
            BtnAddNewStack.Clicked += (source, args) => AddNewAddressButtonListener(source, args);
            stackContent.Children.Add(BtnAddNewStack);
        }



        #region ButtonListeners
        private void AddNewAddressButtonListener(object source, EventArgs args)
        {

            //add visual separator
            var boxView = new BoxView
            {
                Color = Color.DarkSeaGreen,
                HeightRequest = 2d,
                WidthRequest = 380d,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(8)
            };
            stackContent.Children.Add(boxView);

            //add wrapper address Stack
            var addressStack = new StackLayout();
            stackContent.Children.Add(addressStack);
            addressesStacks.Add(addressStack); // list reference

            //add main address name entry
            var addressNameEntry = new AddAddressView(ADDRESS_NAME_PLACEHOLDER);
            addressNameEntry.BtnAdd.IsVisible = false;
            addressNameEntry.LabelDelete.IsVisible = true;
            addressNameEntry.LabelDelete.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => DeleteDetailEntryButtonListener(addressStack, boxView))
            });
            addressStack.Children.Add(addressNameEntry);

            //create detail Entry
            var detailEntry = new AddAddressView(ADDRESS_DETAIL_PLACEHOLDER);
            detailEntry.BtnAdd.Clicked += (s, a) => AddDetailEntryButtonListener(addressStack, addressNameEntry.EntryAddress, s, a);
            addressStack.Children.Add(detailEntry);

            //add reference to the route data
            routeData.Add(new List<AddressEntryAndDetailEntry> { new AddressEntryAndDetailEntry
            {
                AddressEntry = addressNameEntry.EntryAddress,
                DetailEntry = detailEntry.EntryAddress
            }});

            //put button in the end of the view
            stackContent.Children.Remove(BtnAddNewStack);
            stackContent.Children.Add(BtnAddNewStack);
        }

        private void AddDetailEntryButtonListener(StackLayout parentStackLayout, CustomEntry entryAddressName, object source, EventArgs args)
        {
            //make current button invisible
            var btn = source as Button;
            if (btn != null)
                btn.IsVisible = false;

            //create new identical view
            var newDetailView = new AddAddressView(ADDRESS_DETAIL_PLACEHOLDER);
            newDetailView.HorizontalOptions = LayoutOptions.StartAndExpand;
            newDetailView.BtnAdd.Clicked += (s, a) => AddDetailEntryButtonListener(parentStackLayout, entryAddressName, s, a);
            parentStackLayout.Children.Add(newDetailView);

            //add reference to routeData
            int indexInRouteData = addressesStacks.IndexOf(parentStackLayout); //direct mapping from the stacks and the lines of routeData
            routeData[indexInRouteData].Add(new AddressEntryAndDetailEntry
            {
                AddressEntry = entryAddressName,
                DetailEntry = newDetailView.EntryAddress
            });
        }

        private void DeleteDetailEntryButtonListener(StackLayout parentStackLayout, BoxView boxView)
        {
            //remove from route data
            int index = addressesStacks.IndexOf(parentStackLayout); // the data is mapped directly from the addressesStacks 
            routeData.RemoveAt(index);

            //remove from addressesStacks references
            addressesStacks.Remove(parentStackLayout);

            //remove from view
            stackContent.Children.Remove(parentStackLayout);
            if (boxView != null) // for example the first entry group has no boxview
                stackContent.Children.Remove(boxView);
        }

        private async Task SaveData(object source, EventArgs args)
        {
            // add activity indicator to view
            var activityIndicator = new ActivityIndicator
            {
                IsEnabled = true,
                IsRunning = true
            };
            stackContent.Children.Insert(2, activityIndicator);
            var button = source as Button;
            if (button != null) button.IsEnabled = false;

            if (routeData.Count != 0)
            {
                try
                {
                    //create routeid
                    var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
                    int numberOfRoutesInGroup = -1;
                    string newDriverString = ""; //updated driver string in group
                    int indexOfDriver = -1;

                    //find this user in group
                    foreach (var driver in group.Drivers)
                        if (driver.Trim().StartsWith(driverUsername.Trim()))
                        {
                            numberOfRoutesInGroup = Int32.Parse(driver.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[1]);
                            newDriverString = driver.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[0] +
                                    ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                    (numberOfRoutesInGroup + 1).ToString();
                            indexOfDriver = group.Drivers.IndexOf(driver);
                            break;
                        }

                    //normally it is not possible
                    if (numberOfRoutesInGroup == -1)
                        throw new Exception("You did something wrong idiot!");

                    //create hash ids
                    var routeIdInfo = groupName + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                   driverUsername;
                    var routeId = routeIdInfo + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                    (numberOfRoutesInGroup + 1).ToString();

                    //parse data from entries
                    List<Route> routes = new List<Route>();
                    int numberOfAddresses = 0; //range key for route

                    for (int l = 0; l < routeData.Count; l++)
                    {
                        ValidateAddressName(routeData[l]?[0].AddressEntry.Text?.Trim(), l); //same addressList at all the classes so the validation occurs only once

                        for (int m = 0; m < routeData[l].Count; m++)
                        {
                            int dataType = ValidateAddressDetail(routeData[l][m].DetailEntry.Text?.Trim(), l, m + 1); // + 1 cuz in the addressStacks references there is the first address Name entry which is not in routeData
                            Route route;

                            if (dataType == 1)
                            {
                                string[] interval = routeData[l][m].DetailEntry.Text?.Trim().Split('-');
                                int start = Int32.Parse(interval[0]);
                                int end = Int32.Parse(interval[1]);

                                //add addresses for all those numbers
                                for (int k = start; k <= end; k++)
                                {
                                    route = new Route
                                    {
                                        RouteId = routeId,
                                        AddressId = numberOfAddresses,
                                        Location = new Location
                                        {
                                            Country = country,
                                            City = city,
                                            Region = region,
                                            Street = routeData[l][m].AddressEntry.Text?.Trim(),
                                            Nr = k.ToString(),
                                        }
                                    };

                                    if (!routes.Contains(route))
                                        routes.Add(route);

                                    numberOfAddresses++;
                                }
                            }
                            else if (dataType == 2)
                            {
                                Match detailMatch = Regex.Match(routeData[l][m].DetailEntry.Text?.Trim(), DETAIL_DATA_PATTERN);
                                GroupCollection groups = detailMatch.Groups;
                                route = new Route
                                {
                                    RouteId = routeId,
                                    AddressId = numberOfAddresses,
                                    Location = new Location
                                    {
                                        Country = country,
                                        City = city,
                                        Region = region,
                                        Street = routeData[l][m].AddressEntry.Text?.Trim(),
                                        Nr = groups[2].Value?.Trim(),
                                        Block = groups[3].Value?.Trim() + " " + groups[5].Value?.Trim()
                                    }
                                };

                                if (!routes.Contains(route))
                                    routes.Add(route);

                                numberOfAddresses++;
                            }
                            else
                            {
                                throw new Exception("There shouldn't exist another returns than 1 or 2");
                            }
                        }
                    }
                    //save data
                    var routeInfo = new RouteInfo
                    {
                        RouteName = routeName,
                        RouteId = routeIdInfo,
                        Count = numberOfRoutesInGroup + 1,
                        CountRouteAddresses = numberOfAddresses,
                    };

                    //update group
                    if (!String.IsNullOrEmpty(newDriverString))
                    {
                        group.Drivers[indexOfDriver] = newDriverString;
                        var saver = new GroupAndRouteSaver
                        {
                            Group = group,
                            Routes = new List<Route>(routes),
                            RouteInfo = routeInfo
                        };

                        await saver.SaveData();
                    }
                    else
                    {
                        throw new Exception("Bugs in your code!!! String should not be null!!!");
                    }

                    //after data it's saved leave page
                    //TODO make pop logic work
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                    DependencyService.Get<IMessage>().ShortAlert(TEXT_DATA_SAVED);
                }
                catch (AmazonServiceException e) // if there are problems with the service or with the internet
                {
                    DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
                }
                catch (ValidationException e) // show error message to the user
                {
                    DependencyService.Get<IMessage>().ShortAlert(e.Message);
                }
                catch (WebException e)
                {
                    DependencyService.Get<IMessage>().LongAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE1);
                }
                catch (Exception e) // in case of unexpected error like Error: NameResolutionFailure
                {
                    DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
                }
                finally
                {
                    stackContent.Children.Remove(activityIndicator);
                   // if (button != null) button.IsEnabled = true;
                }
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert(TEXT_NO_DATA_TO_SAVE);
                stackContent.Children.Remove(activityIndicator);
            }
        }
        #endregion

        #region Validation
        private void ValidateAddressName(string addressName, int stackIndex)
        {
            if (String.IsNullOrEmpty(addressName))
            {
                (addressesStacks[stackIndex].Children[0] as AddAddressView).LabelIncorrectRedStar.IsVisible = true;
                throw new ValidationException(String.Format(TEXT_ADDRESS_NAME_EXCEPTION, stackIndex + 1));
            }

            //if data is correct make red start view invisible (maybe it was visible from a before inccorect save logic) 
            (addressesStacks[stackIndex].Children[0] as AddAddressView).LabelIncorrectRedStar.IsVisible = false;
        }

        private int ValidateAddressDetail(string addressDetail, int stackIndex, int entryInStackIndex)
        {
            if (String.IsNullOrEmpty(addressDetail))
            {
                (addressesStacks[stackIndex].Children[entryInStackIndex] as AddAddressView).LabelIncorrectRedStar.IsVisible = true;
                throw new ValidationException(TEXT_ADDRESS_DETAIL_EXCEPTION);
            }

            Match match = Regex.Match(addressDetail, @"[0-9]+-[0-9]+");
            Match detailMatch = Regex.Match(addressDetail, DETAIL_DATA_PATTERN);

            if (!match.Success && !detailMatch.Success)
            {
                (addressesStacks[stackIndex].Children[entryInStackIndex] as AddAddressView).LabelIncorrectRedStar.IsVisible = true;
                throw new ValidationException(TEXT_ADDRESS_DETAIL_EXCEPTION);
            }

            //if data is correct make red start view invisible (maybe it was visible from a before inccorect save logic) 
            (addressesStacks[stackIndex].Children[entryInStackIndex] as AddAddressView).LabelIncorrectRedStar.IsVisible = false;

            if (match.Success)
                return 1;
            else if (detailMatch.Success)
                return 2;

            return -1;
        } // return 1 or 2 to know what match was succesful or -1 if something wrong is going on 
        #endregion

    }


    public class AddressEntryAndDetailEntry
    {
        public CustomEntry AddressEntry { get; set; }
        public CustomEntry DetailEntry { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as AddressEntryAndDetailEntry;
            if (item != null)
                return item.AddressEntry.Equals(AddressEntry) && item.DetailEntry.Equals(DetailEntry);

            return false;
        }

        public override int GetHashCode()
        {
            return AddressEntry.GetHashCode() + DetailEntry.GetHashCode();
        }
    }
}