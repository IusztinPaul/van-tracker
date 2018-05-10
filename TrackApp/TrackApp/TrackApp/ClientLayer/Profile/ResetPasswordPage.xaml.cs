using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Profile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ResetPasswordPage : ContentPage
	{
        private TrackUser currentUser;
		public ResetPasswordPage (TrackUser currentUser)
		{
            this.currentUser = currentUser;
			InitializeComponent ();
		}


        private string GetOldPassword()
        {
            var oldPass = EtOldPassword.Text?.Trim();

            if (String.IsNullOrEmpty(oldPass))
                throw new ValidationException("Parola veche nu a fost introdusa");

            if (!oldPass.Equals(currentUser.Password))
                throw new ValidationException("Parola veche nu e introdusa corect!");

            return oldPass;
        }

        private string GetNewPassword()
        {
            var newPass = EtNewPassword.Text?.Trim();
            var newPass2 = EtONewPassword2.Text?.Trim();

            if (String.IsNullOrEmpty(newPass) || String.IsNullOrEmpty(newPass2))
                throw new ValidationException("Introduce parola noua!");


            if (!newPass.Equals(newPass2))
                throw new ValidationException("Parolele noi nu coincid!");

            if(newPass.Length < ClientConsts.PASSWORD_LENGTH_MINIMUM)
                throw new ValidationException($"Parola prea scurta! (minim {ClientConsts.PASSWORD_LENGTH_MINIMUM} caractere)");

            return newPass;
        }


        public async void ButtonSaveDataListener(object source, EventArgs args)
        {
             ActivityIndiResetPass.IsRunning = true;
             StackLPasswords.HeightRequest = 0;

            try
            {
                // throws ValidationException if the data is incorrect
                GetOldPassword();
                var newPass = GetNewPassword();

                currentUser.Password = newPass;
                var saver = new SaveUser { TrackUser = currentUser };
                await saver.SaveData();

                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.EDIT_FINALIZE_MESSAGE);

                await Navigation.PopAsync(); //leave the page
            }
            catch (AmazonServiceException e) // if there are problems with the service or with the internet
            {
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.DYNAMODB_EXCEPTION_MESSAGE2);
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
                   ActivityIndiResetPass.IsRunning = false;
                  StackLPasswords.HeightRequest = -1;
            }

        }

    }
}