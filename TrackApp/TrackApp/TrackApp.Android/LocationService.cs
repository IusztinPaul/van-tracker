using System;
using System.Net;
using System.Threading;
using Amazon.Runtime;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using TrackApp.ClientLayer;
using TrackApp.DataFormat;
using TrackApp.ServerLayer.Save;

namespace TrackApp.Droid
{
    [Service(Exported = true, Name = "com.companyname.LocationService")]
    public class LocationService : Service
    {
        static readonly string TAG = typeof(LocationService).FullName;
        public const int LOOP_WAIT_TIME_SECONDS = ClientConsts.LOCATION_LOOP_WAIT_TIME_MILISECONDS;

        public bool IsLoopRunning { get; set; } = false;
        public IBinder Binder { get; private set; }


        public override void OnCreate()
        {
            // This method is optional to implement
            base.OnCreate();
            Log.Debug(TAG, "OnCreate");
        }

        public override IBinder OnBind(Intent intent)
        {
            // This method must always be implemented
            Log.Debug(TAG, "OnBind");
            this.Binder = new LocationSBinder(this);
            return this.Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            // This method is optional to implement
            Log.Debug(TAG, "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            // This method is optional to implement
            Log.Debug(TAG, "OnDestroy");
            Binder = null;
            // timestamper = null;
            base.OnDestroy();
        }

        public void LocationLooper(string driverUsername)
        {
            Position lastPosition = null;
            IsLoopRunning = true;

            Log.Debug(TAG, "LOCATION THREAD AND LOOP STARTED");

            var loopThread = new Thread(async () =>
          {
              try
              {
                  var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                  if (status.Equals(PermissionStatus.Granted))
                  {

                      while (IsLoopRunning)
                      {
                          var locator = CrossGeolocator.Current;
                          var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(LOOP_WAIT_TIME_SECONDS), null, true);

                          if (lastPosition == null || (position != null && (position.Latitude != lastPosition.Latitude || position.Longitude != lastPosition.Longitude)))
                          {
                              
                              PositionDB pos = new PositionDB
                              {
                                  Username = driverUsername,
                                  Latitude = position.Latitude,
                                  Longitude = position.Longitude,
                                  DateTime = DateTime.Now
                              };

                              //save it
                              var saver = new PositionSaver(pos);
                              await saver.SaveData();
                          }

                          lastPosition = position;
                          Log.Debug(TAG, "LOOP REPEATING");
                          Thread.Sleep(LOOP_WAIT_TIME_SECONDS + 1);  
                       }
                  }
                  else
                      Log.Debug(TAG, "LOCATION PERMISSION NOT GRANTED!");
              }
              catch (AmazonServiceException e) // if there are problems with the service or with the internet
               {
                  Log.Debug(TAG, e.Message);
                  IsLoopRunning = false;
              }
              catch (WebException e)
              {
                  Log.Debug(TAG, e.Message);
                  IsLoopRunning = false;
              }
              catch (Exception e) // in case of unexpected error 
               {
                  Log.Debug(TAG, "EXCEPTION COUGHT: " + e.Message);
                  Log.Debug(TAG, "TYPE: " + e.GetType());
                  IsLoopRunning = false;
              }
          });

            loopThread.Start();
        }
    }
}