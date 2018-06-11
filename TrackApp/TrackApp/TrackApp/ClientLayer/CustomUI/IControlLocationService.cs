using System;
using System.Collections.Generic;
using System.Text;

namespace TrackApp.ClientLayer.CustomUI
{
    public interface IControlLocationService
    {
        IControlLocationService GetInstance();
        void StartService();
        void StopService();
        void StartLocationLoop(string driverUsername);
        void StopLocationLooper();
        bool IsLoopRunning();
    }
}
