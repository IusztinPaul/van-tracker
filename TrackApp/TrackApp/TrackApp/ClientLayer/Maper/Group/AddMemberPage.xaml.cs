using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackApp.DataFormat.UserData;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddMemberPage : ContentPage
	{

        public const string DISPLAY_ALERT_TITLE = "Alegere";
        public const string DISPLAY_ALERT_CONTENT = "Esti sigur ca vrei sa il adaugi ca si {0}?";
        public const string DISPLAY_ALERT_YES_MESSAGE = "Da";
        public const string DISPLAY_ALERT_NO_MESSAGE = "Nu";
        public const string DISPLAY_ALERT_ATTENTION_TITLE= "Atentiune";
        public const string DISPLAY_ALERT_ATTENTION_CONTENT = "Acest utilizator a primit deja o invitatie!";


        public const string ADMINISTRATOR_STATE_DISPLAY = "administrator"; //used ass request logic signals
        public const string DRIVER_STATE_DISPLAY = "sofer";

        public const string ADMINISTRATOR_STATE_DB = "administrator";
        public const string DRIVER_STATE_DB = "driver";

        private string currentState = null; // to know what button was clicked ( Driver or Administrator)


        // keep the state of the page 
        private bool _viewButtons;
        private bool _viewUserList;

		public AddMemberPage (RoledTrackUser currentUser, string groupName)
		{
			InitializeComponent ();
            BindingContext = new AddMemberModelView(currentUser, groupName, this);

            _viewButtons = true;
            _viewUserList = false;

            BtnAdministrator.Clicked += AdministratorBtnListener;
            BtnDriver.Clicked += DriverBtnListener;
            BtnGoBack.Clicked += GoBackBtnListener;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate view
            var bindcont = BindingContext as AddMemberModelView;
            Device.BeginInvokeOnMainThread(async () =>
            {
                await bindcont.PopulateAsync();
            });

            SrcBarNames.Text = "";
        }

        public void DriverBtnListener(object source, EventArgs args)
        {
            currentState = DRIVER_STATE_DB;
            SwitchDisplayView();
        }

        public void AdministratorBtnListener(object source, EventArgs args)
        {
            currentState = ADMINISTRATOR_STATE_DB;
            SwitchDisplayView();
        }

        public void GoBackBtnListener(object source, EventArgs args)
        {
            SwitchDisplayView();
        }


        public void SwitchDisplayView()
        {
            AcIndicatorOnAdd.IsVisible = false;
            AcIndicatorOnAdd.IsRunning = false;

            if (_viewButtons)
            {
                StackUserLists.IsVisible = true;
                ScrViewButtons.IsVisible = false;

                _viewButtons = false;
                _viewUserList = true;

            } else if (_viewUserList)
            {
                StackUserLists.IsVisible = false;
                ScrViewButtons.IsVisible = true;

                _viewButtons = true;
                _viewUserList = false;

            } else
            {
                throw new Exception("There are some problems in your logic");
            }
        }

        public void OnTextChanged(object source, TextChangedEventArgs e)
        {
            var viewmodel = BindingContext as AddMemberModelView;
            viewmodel?.SearchBarListener(e.NewTextValue);
        }

        public void OnAddButtonClicked() // this will be called from the AddMemberModelView
        {
            StackUserLists.IsVisible = false;

            AcIndicatorOnAdd.IsVisible = true;
            AcIndicatorOnAdd.IsRunning = true;
        }

        public async Task<bool> DisplayAlertOnAddButtonClicked()
        {
            return await DisplayAlert(DISPLAY_ALERT_TITLE, String.Format(DISPLAY_ALERT_CONTENT, 
                                        currentState == ADMINISTRATOR_STATE_DB ? ADMINISTRATOR_STATE_DISPLAY : DRIVER_STATE_DISPLAY),
                                        DISPLAY_ALERT_YES_MESSAGE, DISPLAY_ALERT_NO_MESSAGE);
        }

        public string GetCurrentState()
        {
            return currentState;
        }

	}

}