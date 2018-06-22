using System;
using System.Collections.Generic;
using System.Text;
using TrackApp.iOS;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using TrackApp.ClientLayer.CustomUI;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace TrackApp.iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BackgroundColor = UIColor.FromRGB(10, 10, 10);
                Control.BorderStyle = UITextBorderStyle.Line;
            }
        }
    }
}
