using System;
using System.Collections.Generic;
using System.Text;

namespace TrackApp.ClientLayer.CustomUI
{
    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
