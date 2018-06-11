using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TrackApp.Droid
{
    public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        static readonly string TAG = typeof(LocationServiceConnection).FullName;
        public bool IsConnected { get; private set; }
        public LocationSBinder Binder { get; private set; }

        public LocationServiceConnection()
        {
            IsConnected = false;
            Binder = null;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as LocationSBinder;
            IsConnected = this.Binder != null;

            string message = "onServiceConnected - ";
            if (IsConnected)
            {
                message = message + " bound to service " + name.ClassName;
            }
            else
            {
                message = message + " not bound to service " + name.ClassName;
            }
            Log.Info(TAG, message);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
            IsConnected = false;
            Binder = null;
        }

        public void LocationLooper(string driverUsername)
        {
            if (!IsConnected)
                return;

            if (Binder != null && Binder.Service != null)
                Binder.Service.IsLoopRunning = true;
            else
                return;

            Binder?.Service?.LocationLooper(driverUsername);
        }

        public bool IsLoopRunning()
        {
            return Binder.Service.IsLoopRunning;
        }

        public void StopLocationLooper()
        {
            if (Binder != null && Binder.Service != null)
            {
                Binder.Service.IsLoopRunning = false;
            }
            else
                throw new Exception("Problems with your location service!!!");
        }
    }
}