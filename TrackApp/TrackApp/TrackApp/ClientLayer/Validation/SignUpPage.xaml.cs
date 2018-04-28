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

namespace TrackApp.ClientLayer.Validation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignUpPage : ContentPage
	{
        //TODO add custom renderer for the progress bar 
        //TODO add a second password entry for validation
        //TODO add placeholders that go up when you write something

		public SignUpPage ()
		{
			InitializeComponent ();
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

		}

        private async Task<string> GetEntryUsername()
        {
            string username = EntryUsername.Text.Trim();

            if (String.IsNullOrEmpty(username))
                throw new ValidationException("Nume cont incorent!");

            Match match = Regex.Match(username, @"^[^$#]+$"); // those special chars are needed
            // for further concatenations so they have to be unique in the logic 
            if (!match.Success)
                throw new ValidationException("Exista '$' sau '#' in nume cont!");

            QueryUser query = new QueryUser();
            var user = await query.LoadData<TrackUser>(username);
            if(user != null) // username is unique 
                throw new ValidationException("Acest nume de cont exista deja!");

            return username;
        }

        private string GetEntryFirstName()
        {
            string firstName = EntryFirstName.Text.Trim();
            if (String.IsNullOrEmpty(firstName))
                throw new ValidationException("Prenume incorent!");

            return firstName;
        }

        private string GetEntryLastName()
        {
            string lastName = EntryLastName.Text.Trim();
            if (String.IsNullOrEmpty(lastName))
                throw new ValidationException("Nume familie incorent!");

            return lastName;
        }

        private string GetEntryPassword()
        {
            string password = EntryPassword.Text.Trim();
            if(String.IsNullOrEmpty(password))
                throw new ValidationException("Parola incorenta!");

            if (password.Length < ClientConsts.PASSWORD_LENGTH)
                throw new ValidationException("Parola de minim " + ClientConsts.PASSWORD_LENGTH + "!");

            return password;
        }

        private string GetEntryPhoneNumber()
        {
            string phoneNumber = EntryPhoneNumber.Text.Trim();

            if (String.IsNullOrEmpty(phoneNumber)) //length validation
                throw new ValidationException("Introduce numar telefon!");

            Match match = Regex.Match(phoneNumber, @"(\+4)?([0-9]{4})\s?([0-9]{3})\s?([0-9]{3})");

            if (!match.Success) //regex validation
                throw new ValidationException("Numar telefon incorect!");

            return phoneNumber;
        }

        private string GetEntryEmail()
        {
            string email = EntryEmail.Text.Trim();

            if (String.IsNullOrEmpty(email)) //length validation
                throw new ValidationException("Introduce email!");

            Match match = Regex.Match(email.ToLower(), @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");
            if (!match.Success) // regex validation
                throw new ValidationException("Email introdus incorect!");

            return email;
        }

        public async Task BtnSaveUserListener()
        {
            double progBarIncrementRate = 1d / 4d;
            double progBarCurentValue = 0d;

            try
            {
                string username, firstName, lastName, password, phoneNumber, Email;

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

                // create user object
                TrackUser user = new TrackUser()
                {
                    Email = Email,
                    FirstName = firstName,
                    LastName = lastName,
                    Password = password,
                    Phone = phoneNumber,
                    Username = username
                };
                var saver = new SaveUser {TrackUser = user}; // create saver object
                await saver.SaveData(); // save data async

                progBarCurentValue += progBarIncrementRate;
                await InscreaseProgBar(progBarCurentValue);


                DependencyService.Get<IMessage>().LongAlert(ClientConsts.ACCOUNT_CREATED_MESSAGE);
                Thread.Sleep(100); // display message for 100 millisecond
                Application.Current.MainPage = new MainPage(user); // go to the main page of the app

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
        }

	    private async Task InscreaseProgBar(double currentValue)
	    {
	        await ProgBarSaveBtn.ProgressTo(currentValue, 250, Easing.Linear);
	    }
    }
}