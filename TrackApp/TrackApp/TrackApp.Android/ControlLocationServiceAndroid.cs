using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TrackApp.ClientLayer.CustomUI;
using TrackApp.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(ControlLocationServiceAndroid))]
namespace TrackApp.Droid
{
    public class ControlLocationServiceAndroid : IControlLocationService
    {
        private IControlLocationService instance;
        private LocationServiceConnection serviceConnection;

        public IControlLocationService GetInstance()
        {
            if (instance == null)
                instance = this;

            return instance;
        }

        public void StartLocationLoop(string driverUsername)
        {
            if (serviceConnection == null || instance == null)
                return;

            serviceConnection?.LocationLooper(driverUsername);
        }

        public void StartService()
        {
            if (instance == null)
                return;

            if (serviceConnection == null)
            {
                serviceConnection = new LocationServiceConnection();
            }

            Intent serviceToStart = new Intent(MainActivity.Instance, typeof(LocationService));
            MainActivity.Instance.BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
        }

        public bool IsLoopRunning()
        {
            return serviceConnection.IsLoopRunning();
        }

        public void StopLocationLooper()
        {
            if (serviceConnection != null)
                serviceConnection.StopLocationLooper();
        }

        public void StopService()
        {
            MainActivity.Instance.UnbindService(serviceConnection);
        }
    }
}