using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackApp.ClientLayer.Exceptions;
using TrackApp.DataFormat.UserData;
using TrackApp.DataFormat;
using TrackApp.ServerLayer.Query;
using TrackApp.ServerLayer.Save;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using Amazon.Runtime;
using TrackApp.ClientLayer.CustomUI;
using System.Net;

namespace TrackApp.ClientLayer.Maper
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateGroupPage : ContentPage
    {
        private TrackUser currentUser;

        public const string GROUP_CREATED_MESSAGE = "Grupul a fost creat";

        public CreateGroupPage(TrackUser currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;

            BtnSaveData.Clicked += async (source, args) =>
            {
                // show activity indicator
                ActIndSaveData.IsVisible = true;
                ActIndSaveData.IsRunning = true;
                BtnSaveData.IsEnabled = false;

                await BtnCreateGroupListener(source, args);

                // hide activity indicator
                ActIndSaveData.IsVisible = false;
                ActIndSaveData.IsRunning = false;
                BtnSaveData.IsEnabled = true;
            };
        }

        public async Task BtnCreateGroupListener(object source, EventArgs args)
        {
            try
            {
                //validate and create group
                var groupName = EntryGroupName.Text?.Trim();
                await ValidateGroupName(groupName);

                var newGroup = new TrackApp.DataFormat.Group
                {
                    Name = groupName,
                    Admins = new List<string> { currentUser.Username }
                };

                //create group reference to the UserFriends Table to be saved
                var userFriends = await QueryHashLoader.LoadData<UserFriends>(currentUser.Username);

                var groupNameSignaled = groupName + ClientConsts.CONCAT_SPECIAL_CHARACTER +
                                        ClientConsts.ADMINISTRATOR_SIGNAL;

                if (userFriends == null)
                    userFriends = new UserFriends
                    {
                        Username = currentUser.Username,
                        Groups = new List<string> { groupNameSignaled }
                    };
                else if (userFriends != null && userFriends.Groups == null)
                    userFriends.Groups = new List<string> { groupNameSignaled };
                else
                    userFriends.Groups.Add(groupNameSignaled);


                //save all data
                var multiBatchSaver = new MultiTableBatchGroupsUserFriends
                {
                    Group = newGroup,
                    UserFriends = userFriends
                };

                await multiBatchSaver.SaveData();

                //return to main page
                await Navigation.PopAsync();

                DependencyService.Get<IMessage>().ShortAlert(GROUP_CREATED_MESSAGE);
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
                Console.WriteLine(e.Message);
                DependencyService.Get<IMessage>().ShortAlert(ClientConsts.INTERNET_EXCEPTION_MESSAGE);
            }
        }

        private async Task ValidateGroupName(string groupName)
        {
            if (String.IsNullOrEmpty(groupName))
                throw new ValidationException("Nume grup incorent!");

            Match match = Regex.Match(groupName, @"^[^$#]+$"); // those special chars are needed
            // for further concatenations so they have to be unique in the logic 
            if (!match.Success)
                throw new ValidationException("Exista '$' sau '#' in nume grup!");

            var group = await QueryHashLoader.LoadData<DataFormat.Group>(groupName);
            if (group != null) // username is unique 
                throw new ValidationException("Acest nume de grup exista deja!");

        }

    }
}