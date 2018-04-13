using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.iOS;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

namespace TrackApp.iOS
{
    class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                //TODO adapt colour to custom entry
                Control.BackgroundColor = UIColor.FromRGB(10, 10, 10);
                Control.BorderStyle = UITextBorderStyle.Line;
            }
        }
    }
}
