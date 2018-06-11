using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrackApp.DataFormat;
using System.Text.RegularExpressions;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.ServerLayer.Save;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using System.Net;

namespace TrackApp.ClientLayer.Maper.Group
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAddressPage : ContentPage
    {
        public const string TEXT_WRONG_ADDRESS_NAME = "Nume adresa introdusa gresit!";
        public const string TEXT_WRONG_ADDRESS_DETAIL = "Detalii adresa introduse gresit!";
        public const string TEXT_DATA_SAVED = "Datele au fost salvate cu succes!";
        public const string TEXT_DATA_DELETED = "Datele au fost sterse cu succes!";

        private Route route;

        public EditAddressPage(Route route)
        {
            InitializeComponent();
            this.route = route;

            EntryAddressName.Text = route.Location.Street;
            EntryAddressDetail.Text = route.BottomRouteText;

            BtnSaveData.Clicked += async (s, a) => await BtnSaveListener();
            BtnDeleteData.Clicked += async (s, a) => await ButtonDeleteDataListener();
        }

        private async Task BtnSaveListener()
        {
            var actInd = new ActivityIndicator
            {
                IsEnabled = true,
                IsRunning = true
            };
            contentStackLayout.Children.Add(actInd);
            BtnSaveData.IsEnabled = false;

            try
            {
                ValidateData();
                await RouteSaver.SaveSingleRoute(this.route);

                DependencyService.Get<IMessage>().ShortAlert(TEXT_DATA_SAVED);
                Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
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
                Console.WriteLine("EXCEPTION {0}", e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
            finally
            {
                contentStackLayout.Children.Remove(actInd);
                BtnSaveData.IsEnabled = true;
            }
        }

        private async Task ButtonDeleteDataListener()
        {
            var actInd = new ActivityIndicator
            {
                IsEnabled = true,
                IsRunning = true
            };
            contentStackLayout.Children.Add(actInd);
            BtnDeleteData.IsEnabled = false;

            try
            {
                await RouteSaver.DeleteSingleRoute(this.route);

                DependencyService.Get<IMessage>().ShortAlert(TEXT_DATA_DELETED);
                Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
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
                Console.WriteLine("EXCEPTION {0}", e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
            finally
            {
                contentStackLayout.Children.Remove(actInd);
                BtnDeleteData.IsEnabled = true;
            }
        }

        private void ValidateData()
        {
            var addressName = EntryAddressName.Text?.Trim();
            var addressDetails = EntryAddressDetail.Text?.Trim();

            if (String.IsNullOrEmpty(addressName))
                throw new ValidationException(TEXT_WRONG_ADDRESS_NAME);

            if (String.IsNullOrEmpty(addressDetails))
                throw new ValidationException(TEXT_WRONG_ADDRESS_DETAIL);

            Match detailMatch = Regex.Match(addressDetails, CreateRoutePage.DETAIL_DATA_PATTERN);

            if (!detailMatch.Success)
                throw new ValidationException(TEXT_WRONG_ADDRESS_DETAIL);

            GroupCollection groups = detailMatch.Groups;
            this.route.Location.Street = addressName;
            this.route.Location.Nr = groups[2].Value?.Trim();
            this.route.Location.Block = groups[3].Value?.Trim() + " " + groups[5].Value?.Trim();
        }
    }
}