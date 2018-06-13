using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TrackApp.ClientLayer.CustomUI
{
    public class AddAddressView : Grid
    {
        public Label LabelIncorrectRedStar { get; set; } = new Label();
        public CustomEntry EntryAddress { get; set; } = new CustomEntry();
        public Button BtnAdd { get; set; } = new Button();
        public Label LabelDelete { get; set; } = new Label();

        public AddAddressView(string entryPlaceholder)
        {
            HorizontalOptions = LayoutOptions.StartAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;

            RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            LabelIncorrectRedStar.Text = "*";
            LabelIncorrectRedStar.TextColor = Color.Red;
            LabelIncorrectRedStar.FontAttributes = FontAttributes.Bold;
            LabelIncorrectRedStar.FontSize = 15d;
            LabelIncorrectRedStar.IsVisible = false;
            Children.Add(LabelIncorrectRedStar, 0, 0);
            SetRowSpan(LabelIncorrectRedStar, 2);

            EntryAddress.Placeholder = entryPlaceholder;
            EntryAddress.HorizontalOptions = LayoutOptions.StartAndExpand;
            EntryAddress.WidthRequest = 250;
            Children.Add(EntryAddress, 1, 0);
            SetRowSpan(EntryAddress, 2);

            BtnAdd.Text = "+";
            BtnAdd.TextColor = Color.Black;
            BtnAdd.FontSize = 12d;
            BtnAdd.BorderWidth = 0.75d;
            BtnAdd.BorderColor = Color.Black;
            BtnAdd.BackgroundColor = Color.White;
            BtnAdd.CornerRadius = 10;
            BtnAdd.WidthRequest = 30;
            BtnAdd.HeightRequest = 30;
            BtnAdd.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Children.Add(BtnAdd, 3, 0);
            SetRowSpan(BtnAdd, 2);

            LabelDelete.Text = "Sterge";
            LabelDelete.FontSize = 10d;
            LabelDelete.TextColor = Color.Red;
            LabelDelete.FontAttributes = FontAttributes.Italic | FontAttributes.Bold;
            LabelDelete.HorizontalOptions = LayoutOptions.End;
            LabelDelete.IsVisible = false;
            Children.Add(LabelDelete, 2, 0);
        }
    }
}
