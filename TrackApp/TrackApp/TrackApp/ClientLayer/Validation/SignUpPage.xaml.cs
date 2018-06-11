using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Save;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Amazon.Runtime;
using Plugin.Geolocator;
using Plugin.Permissions.Abstractions;

namespace TrackApp.ClientLayer.Validation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        //TODO add custom renderer for the progress bar 
        //TODO add a second password entry for validation
        //TODO add placeholders that go up when you write something

        public SignUpPage()
        {
            InitializeComponent();
            BtnSaveUser.Clicked += async (source, args) =>
            {
                //reset the progess bar 
                ProgBarSaveBtn.Progress = 0d;
                ProgBarSaveBtn.IsVisible = true;

                await BtnSaveUserListener();

                // make the prog bar invisible
                ProgBarSaveBtn.IsVisible = false;
            };


            //to avoid internal null pointer exception
            EntryUsername.Text = "";
            EntryFirstName.Text = "";
            EntryLastName.Text = "";
            EntryPassword.Text = "";
            EntryPhoneNumber.Text = "";
            EntryEmail.Text = "";
            // EntryCountry.Text = "";
            EntryRegion.Text = "";
            EntryCity.Text = "";
            EntryStreet.Text = "";
            EntryNr.Text = "";
            EntryBlock.Text = "";

        }
        #region Username
        private async Task<string> GetEntryUsername()
        {
            string username = EntryUsername.Text.Trim();
            await ValidateUsername(username);
            return username;
        }

        public async static Task ValidateUsername(string username)
        {
            if (String.IsNullOrEmpty(username))
                throw new ValidationException("Nume cont incorent!");

            Match match = Regex.Match(username, @"^[^$#]+$"); // those special chars are needed
            // for further concatenations so they have to be unique in the logic 
            if (!match.Success)
                throw new ValidationException("Exista '$' sau '#' in nume cont!");

            QueryUser query = new QueryUser();
            var user = await query.LoadData<TrackUser>(username);
            if (user != null) // username is unique 
                throw new ValidationException("Acest nume de cont exista deja!");
        }

        #endregion

        #region FirstName

        private string GetEntryFirstName()
        {
            string firstName = EntryFirstName.Text.Trim();
            ValidateFirstName(firstName);
            return firstName;
        }

        public static void ValidateFirstName(string firstName)
        {
            if (String.IsNullOrEmpty(firstName))
                throw new ValidationException("Prenume incorent!");
        }

        #endregion

        #region LastName
        private string GetEntryLastName()
        {
            string lastName = EntryLastName.Text.Trim();
            ValidateLastName(lastName);
            return lastName;
        }

        public static void ValidateLastName(string lastName)
        {
            if (String.IsNullOrEmpty(lastName))
                throw new ValidationException("Nume familie incorent!");
        }

        #endregion

        #region Password
        private string GetEntryPassword()
        {
            string password = EntryPassword.Text?.Trim();
            string password2 = EntryPassword2.Text?.Trim();
            ValidatePassword(password, password2);
            return password;
        }

        public static void ValidatePassword(string password, string password2)
        {
            if (String.IsNullOrEmpty(password))
                throw new ValidationException("Parola incorenta!");

            if (String.IsNullOrEmpty(password2))
                throw new ValidationException("Parola incorenta!");

            if (password.Length < ClientConsts.PASSWORD_LENGTH_MINIMUM)
                throw new ValidationException("Parola de minim " + ClientConsts.PASSWORD_LENGTH_MINIMUM + "!");

            if (!password.Equals(password2))
                throw new ValidationException("Parolele nu coincid!");
        }
        #endregion

        #region PhoneNumber
        private string GetEntryPhoneNumber()
        {
            string phoneNumber = EntryPhoneNumber.Text.Trim();
            ValidatePhoneNumber(phoneNumber);
            return phoneNumber;
        }

        public static void ValidatePhoneNumber(string phoneNumber)
        {
            if (String.IsNullOrEmpty(phoneNumber)) //length validation
                throw new ValidationException("Introduce numar telefon!");

            Match match = Regex.Match(phoneNumber, @"(\+4)?([0-9]{4})\s?([0-9]{3})\s?([0-9]{3})");

            if (!match.Success) //regex validation
                throw new ValidationException("Numar telefon incorect!");
        }
        #endregion

        #region Email
        private string GetEntryEmail()
        {
            string email = EntryEmail.Text.Trim();
            ValidateEmail(email);
            return email;
        }

        public static void ValidateEmail(string email)
        {
            if (String.IsNullOrEmpty(email)) //length validation
                throw new ValidationException("Introduce email!");

            Match match = Regex.Match(email.ToLower(), @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");
            if (!match.Success) // regex validation
                throw new ValidationException("Email introdus incorect!");
        }
        #endregion


        public static void NonNullValidation(string entryFieldText, string messageTypeField)
        {
            if (String.IsNullOrEmpty(entryFieldText))
                throw new ValidationException($"Campul despre {messageTypeField} e gol!");
        }

        #region Country
        private string GetCountry()
        {
            var country = EntryCountry.Text?.Trim();
            NonNullValidation(country, "tara");
            return country;
        }
        #endregion

        #region Region
        private string GetRegion()
        {
            var region = EntryRegion.Text?.Trim();
            NonNullValidation(region, "regiune");
            return region;
        }
        #endregion

        #region City
        private string GetCity()
        {
            var city = EntryCity.Text?.Trim();
            NonNullValidation(city, "oras");
            return city;
        }
        #endregion

        #region Address
        private string GetAddress()
        {
            var address = EntryStreet.Text?.Trim();
            NonNullValidation(address, "adresa");
            return address;
        }
        #endregion

        #region NumberAddress
        private string GetNumberAddress()
        {
            var nr = EntryNr.Text?.Trim();
            NonNullValidation(nr, "numar adresa");
            return nr;
        }
        #endregion

        #region Block
        private string GetBlock()
        {
            var block = EntryBlock.Text?.Trim();
            NonNullValidation(block, "bloc");
            return block;
        }
        #endregion

        public async Task BtnSaveUserListener()
        {

            BtnSaveUser.IsEnabled = false;

            //first ask for permissions
            var status = await PermissionsCaller.PermissionLocationCaller(this);
            await PermissionsCaller.PermissionStorageCaller(this);

            double progBarIncrementRate = 1d / 5d;
            double progBarCurentValue = 0d;

            if (status.Equals(PermissionStatus.Granted))
            {
                try
                {
                    string username, firstName, lastName, password, phoneNumber, Email;
                    string country, region, city, street, nr, block;

                    // grab the data and animate the progress bar 
                    username = await GetEntryUsername();
                    progBarCurentValue += progBarIncrementRate;
                    await InscreaseProgBar(progBarCurentValue);

                    firstName = GetEntryFirstName();

                    lastName = GetEntryLastName();
                    progBarCurentValue += progBarIncrementRate;
                    await InscreaseProgBar(progBarCurentValue);

                    password = GetEntryPassword();

                    phoneNumber = GetEntryPhoneNumber();

                    Email = GetEntryEmail();
                    progBarCurentValue += progBarIncrementRate;
                    await InscreaseProgBar(progBarCurentValue);

                    country = GetCountry();
                    region = GetRegion();
                    city = GetCity();
                    street = GetAddress();
                    nr = GetNumberAddress();
                    block = GetBlock();

                    progBarCurentValue += progBarIncrementRate;
                    await InscreaseProgBar(progBarCurentValue);

                    // create user object
                    TrackUser user = new TrackUser()
                    {
                        Email = Email,
                        FirstName = firstName,
                        LastName = lastName,
                        Password = password,
                        Phone = phoneNumber,
                        Username = username,
                        Location = new DataFormat.Location
                        {
                            Country = country,
                            Region = region,
                            City = city,
                            Street = street,
                            Nr = nr,
                            Block = block
                        },
                        Latitude = TrackUser.NO_POSITION_VALUE,
                        Longitude = TrackUser.NO_POSITION_VALUE
                    };

                    //if the validation was ok save the user with his current location
                    var locator = CrossGeolocator.Current;
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1), null, true);
                    user.Latitude = position.Latitude;
                    user.Longitude = position.Longitude;

                    var saver = new SaveUser { TrackUser = user }; // create saver object
                    await saver.SaveData(); // save data async

                    progBarCurentValue += progBarIncrementRate;
                    await InscreaseProgBar(progBarCurentValue);

                    Application.Current.Properties[ClientConsts.LOGIN_KEY_FLAG] = user.Username; // keep the user logged in
                    await Application.Current.SavePropertiesAsync();

                    DependencyService.Get<IMessage>().LongAlert(ClientConsts.ACCOUNT_CREATED_MESSAGE);
                    Thread.Sleep(100); // display message for 100 millisecond
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new MainPage(user); // go to the main page of the app
                    });

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
                    BtnSaveUser.IsEnabled = true;
                }
            }
            else
            {
                await DisplayAlert("Atentie", "Fara a obtine aprobarea dumneavoastra de a avea acces asupra locatiei nu va putem lasa sa continuati", "Ok");
            }
        }

        private async Task InscreaseProgBar(double currentValue)
        {
            await ProgBarSaveBtn.ProgressTo(currentValue, 250, Easing.Linear);
        }
    }
}