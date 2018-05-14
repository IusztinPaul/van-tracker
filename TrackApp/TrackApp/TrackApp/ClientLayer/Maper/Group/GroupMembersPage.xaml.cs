using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackApp.ClientLayer.Maper.Group
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupMembersPage : ContentPage
	{
		public GroupMembersPage (string groupName, BasicRefreshingModelView bindingContext)
		{
			InitializeComponent ();
            BindingContext = bindingContext;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //populate
            var bindCont = BindingContext as GroupPageViewModel;
            if (bindCont != null)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await bindCont.PopulateAsync();
                });

            SrcBarNames.Text = "";
        }

        public void OnTextChanged(object source, TextChangedEventArgs e)
        {
            var viewmodel = BindingContext as GroupPageViewModel;
            viewmodel?.SearchBarListener(e.NewTextValue);
        }
    }
}