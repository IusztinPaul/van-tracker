using Amazon.Runtime;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ClientLayer.Validation;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Permissions.Abstractions;
using TrackApp.ClientLayer.Maper.Group.MapN;
using TK.CustomMap;

namespace TrackApp.ClientLayer.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {

        private TrackUser currentUser;
        private string userIcon;

        public ProfilePage(TrackUser currentUser)
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel(currentUser);

            this.currentUser = currentUser;
            userIcon = currentUser.Icon;
        }

        public async void ButtonSaveDataListener(object source, EventArgs args)
        {

            var bindCont = BindingContext as ProfileViewModel;
            bindCont.IsSaving = true; // start activityindicator 
            bindCont.ShowGridData = 0;

            try
            {
                var newUser = GetTrackUserWithValidatedData();

                //change view back to labels and save the data from the entryfields only if the data is valid
                LbFirstName.IsVisible = true;
                LbLastName.IsVisible = true;
                LbEmail.IsVisible = true;
                LbPhoneNumber.IsVisible = true;
                LbCountry.IsVisible = true;
                LbRegion.IsVisible = true;
                LbCity.IsVisible = true;
                LbAddress.IsVisible = true;
                LbAddressNumber.IsVisible = true;
                LbBlock.IsVisible = true;

                EtFirstName.IsVisible = false;
                EtLastName.IsVisible = false;
                EtEmail.IsVisible = false;
                EtPhoneNumber.IsVisible = false;
                EtCountry.IsVisible = false;
                EtRegion.IsVisible = false;
                EtCity.IsVisible = false;
                EtAddress.IsVisible = false;
                EtAddressNumber.IsVisible = false;
                EtBlock.IsVisible = false;

                LbChangePhoto.IsVisible = false;

                BtnSaveEdit.IsVisible = false;

                var saver = new SaveUser { TrackUser = newUser };
                await saver.SaveData();

                this.currentUser = newUser; //bind the context to the new user
                bindCont.CurrentUser = new TrackUser(newUser);

                //delete the editing gesture recognizers from the image
                ImgProfile.GestureRecognizers.Clear();

                //change menu profile picture
                NavigationMasterPage.Instance.ChangeProfilePhotoWithImgSource(currentUser.IconSource);

                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.EDIT_FINALIZE_MESSAGE);

            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert("Probleme cu servar-ul sau imaginea este prea mare!");
            }
            catch (ValidationException e)
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
                bindCont.IsSaving = false;
                bindCont.ShowGridData = -1;

                //after the edit finished restart looping for location
                App.locationServiceController.StartLocationLoop(currentUser.Username);
            }
        }

        public async void TbItemEditListener(object source, EventArgs args)
        {
            //stop loop while editing the profile
            App.locationServiceController.StopLocationLooper();

            //first ask for storage permission
            var result = await PermissionsCaller.PermissionStorageCaller(this);
            if (result.Equals(PermissionStatus.Granted))
            {
                // change view to entryfields and save button
                EtFirstName.IsVisible = true;
                EtLastName.IsVisible = true;
                EtEmail.IsVisible = true;
                EtPhoneNumber.IsVisible = true;
                EtCountry.IsVisible = true;
                EtRegion.IsVisible = true;
                EtCity.IsVisible = true;
                EtAddress.IsVisible = true;
                EtAddressNumber.IsVisible = true;
                EtBlock.IsVisible = true;

                LbFirstName.IsVisible = false;
                LbLastName.IsVisible = false;
                LbEmail.IsVisible = false;
                LbPhoneNumber.IsVisible = false;
                LbCountry.IsVisible = false;
                LbRegion.IsVisible = false;
                LbCity.IsVisible = false;
                LbAddress.IsVisible = false;
                LbAddressNumber.IsVisible = false;
                LbBlock.IsVisible = false;


                // add pick image gesture recognizer
                TapGestureRecognizer pickPictureRecognizer = new TapGestureRecognizer();
                pickPictureRecognizer.Tapped += ChangeProfilePhotoListener;
                ImgProfile.GestureRecognizers.Clear();
                ImgProfile.GestureRecognizers.Add(pickPictureRecognizer);

                //setup hint label
                LbChangePhoto.IsVisible = true;
                LbChangePhoto.GestureRecognizers.Clear();
                LbChangePhoto.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => ChangeProfilePhotoListener(null, null))
                });

                BtnSaveEdit.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Atentie", "Nu va putem lasa sa va editati account-ul fara a ne oferi permisiunea de a citi din spatiul de stocare al telefonului dumneavoastra", "Ok");
            }
        }


        private TrackUser GetTrackUserWithValidatedData()
        {

            //those methods throw ValidationException if something ain't as expected
            SignUpPage.ValidateFirstName(EtFirstName.Text?.Trim());
            SignUpPage.ValidateLastName(EtLastName.Text?.Trim());
            SignUpPage.ValidatePhoneNumber(EtPhoneNumber.Text?.Trim());
            SignUpPage.ValidateEmail(EtEmail.Text?.Trim());
            SignUpPage.NonNullValidation(EtCountry.Text?.Trim(), "tara");
            SignUpPage.NonNullValidation(EtRegion.Text?.Trim(), "regiune");
            SignUpPage.NonNullValidation(EtAddress.Text?.Trim(), "adresa");
            SignUpPage.NonNullValidation(EtAddressNumber.Text?.Trim(), "numar adresa");
            SignUpPage.NonNullValidation(EtCity.Text?.Trim(), "oras");
            SignUpPage.NonNullValidation(EtBlock.Text?.Trim(), "block");

            var newUser = new TrackUser
            {
                Username = this.currentUser.Username,
                Password = this.currentUser.Password,
                Icon = userIcon, //this parameter it is set in the tap gesture recognizer defined in the TbItemEditListener method
                FirstName = EtFirstName.Text?.Trim(),
                LastName = EtLastName.Text?.Trim(),
                Email = EtEmail.Text?.Trim(),
                Phone = EtPhoneNumber.Text?.Trim(),
                Location = new DataFormat.Location
                {
                    Region = EtRegion.Text?.Trim(),
                    Country = EtCountry.Text?.Trim(),
                    Street = EtAddress.Text?.Trim(),
                    Nr = EtAddressNumber.Text?.Trim(),
                    City = EtCity.Text?.Trim(),
                    Block = EtBlock.Text?.Trim()
                }
            };

            return newUser;
        }

        private async void ChangeProfilePhotoListener(object source, EventArgs args)
        {
            Stream stream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

            if (stream != null)
            {

                //convert image stream to serialized string

                //convert image stream to byte array
                var byteMemoryStream = new MemoryStream();
                stream.CopyTo(byteMemoryStream);
                byte[] imgBytes = byteMemoryStream.ToArray();

                //compress data to stream
                var memoryStream = new MemoryStream();
                using(DeflateStream dstream = new DeflateStream(memoryStream, CompressionLevel.Optimal))
                {
                    dstream.Write(imgBytes, 0, imgBytes.Length);
                }

                //convert stream to encoded string 
                string iconString = Convert.ToBase64String(memoryStream.ToArray());

                //update data to save
                userIcon = iconString;

                //update user so the binding will fire
                this.currentUser.Icon = iconString;
                (BindingContext as ProfileViewModel).CurrentUser = new TrackUser(currentUser); // fire the binding
                NavigationMasterPage.Instance?.ChangeProfilePhotoWithImgSource(currentUser.IconSource); // change mater detail photo
            }
        }


        public async void LogoutListener(object source, EventArgs args)
        {
            // stop the service method if the user logs out
            App.locationServiceController.StopLocationLooper();

            //put the login flag on no user
            Application.Current.Properties[ClientConsts.LOGIN_KEY_FLAG] = ClientConsts.LOGIN_NO_USER_FLAG;
            await Application.Current.SavePropertiesAsync();

            //reset map location
            MapPage.LastKnownLocation = MapSpan.FromCenterAndRadius(
                  MapPage.DEFAULT_MAP_POSITION, Distance.FromMiles(ClientConsts.FROM_KM_MAP_DISTANCE));

            Device.BeginInvokeOnMainThread(() =>
            {
                App.Current.MainPage = new NavigationPage(new LoginPage(currentUser));
            });
        }

        public async void ResetPasswordListener(object source, EventArgs args)
        {
            await Navigation.PushAsync(new ResetPasswordPage(currentUser));
        }

    }
}