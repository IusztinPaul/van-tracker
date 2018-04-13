using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Validation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignUpPage : ContentPage
	{
		public SignUpPage ()
		{
			InitializeComponent ();
		    BtnSaveUser.Clicked += (source, args) => { BtnSaveUserListener(); };
        }

        private string GetEntryUsername()
        {
            string username = EntryUsername.Text.Trim();
            if (username.Length > 0)
                return username;

            throw new ValidationException("No valid username!");
        }

        private string GetEntryFirstName()
        {
            string firstName = EntryFirstName.Text.Trim();
            if (firstName.Length > 0)
                return firstName;

            throw new ValidationException("No valid first name!");
        }

        private string GetEntryLastName()
        {
            string lastName = EntryLastName.Text.Trim();
            if (lastName.Length > 0)
                return lastName;

            throw new ValidationException("No valid last name!");
        }

        private string GetEntryPassword()
        {
            string password = EntryPassword.Text.Trim();
            if (password.Length > ClientConsts.PASSWORD_LENGTH)
                return password;

            throw new ValidationException("No valid password!");
        }

        private string GetEntryPhoneNumber()
        {
            string phoneNumber = EntryPhoneNumber.Text.Trim();

            if (phoneNumber.Length == 0) //length validation
                throw new ValidationException("No valid phone number");

            Match match = Regex.Match(phoneNumber, @"(\+4)?([0-9]{4})(\s?)([0-9]{3})\3([0-9]{3})");

            if (!match.Success) //regex validation
                throw new ValidationException("No valid phone number");

            return phoneNumber;
        }

        private string GetEntryEmail()
        {
            string email = EntryEmail.Text.Trim();

            if (email.Length == 0) //length validation
                throw new ValidationException("No valid email");

            Match match = Regex.Match(email, @"^[0-9A-Za-z]*?@[0-9A-Za-z]*?\.[0-9A-Za-z]*?$");
            if (match.Success) // regex validation
                throw new ValidationException("No valid email");

            return email;
        }

        public async void BtnSaveUserListener()
        {
            string username, firstName, lastName, password, phoneNumber, Email;

            try
            {
                username = GetEntryUsername();
                firstName = GetEntryFirstName();
                lastName = GetEntryLastName();
                password = GetEntryPassword();
                phoneNumber = GetEntryPhoneNumber();
                Email = GetEntryEmail();
            }
            catch (ValidationException e)
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
                return;
            }

            

            TrackUser user = new TrackUser() {Email = Email, FirstName = firstName, LastName = lastName, Password = password, Phone = phoneNumber, Username = username };
            var saver = new SaveUser {TrackUser = user}; // create saver object
            await saver.SaveData(); // save data async
        }
    }
}