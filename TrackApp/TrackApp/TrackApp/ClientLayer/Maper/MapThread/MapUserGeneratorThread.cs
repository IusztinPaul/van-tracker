using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using TrackApp.DataFormat.UserData;
using TrackApp.ServerLayer.Query;
using Xamarin.Forms;
using TK.CustomMap;
using TK.CustomMap.Overlays;
using System.Threading.Tasks;
using TrackApp.DataFormat;
using TrackApp.ClientLayer.Maper.Group.MapN;
using Plugin.Geolocator;
using System.Linq;

namespace TrackApp.ClientLayer.Maper.MapThread
{
    public class MapUserGeneratorThread 
    {
        public const int SLEEP_TIME_LOOP_MILISECONDS = ClientConsts.UPDATE_CIRCLE_LOCATION_LOOP_TIME_MILISECONDS;

        public const long START_ADMINISTRATOR_LOOK_UP_POSITION_TIME_HOURS = 1;
        public const long END_ADMINISTRATOR_LOOK_UP_POSITION_TIME_HOURS = 12; 

        private string _username;
        private MapPageModelView modelView;
        private int circleIndex;
        private bool singleDriver;
        private Color userColor;

        private TKCircle lastCircle;
        private bool isLoopRunning = false;
        private CancellationTokenSource cancellationTokenSource;
        public Task Task { get; private set; }

        public MapUserGeneratorThread(string username, bool singleDriver, MapPageModelView modelView, int cirlceIndex, Color userColor) 
        {
            _username = username;
            this.singleDriver = singleDriver;
            this.modelView = modelView;
            this.circleIndex = cirlceIndex;
            this.userColor = userColor;

            lastCircle = null;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void StopLoop()
        {
            isLoopRunning = false;
            cancellationTokenSource.Cancel();
            Console.WriteLine("Keep location loop stopped");
        }

        public Task RunTask()
        {

            var cancellationToken = cancellationTokenSource.Token;

            Task = Task.Run(async () =>
            {
                try
                {
                    await RunTaskMethod(cancellationToken);
                }
                catch(OperationCanceledException)
                {
                    Console.WriteLine("Keep location loop token cancel exception thrown!");
                }
                catch (AmazonServiceException e) // if there are problems with the service or with the internet
                {
                    Console.WriteLine("AMAZON SERIVCE EXCEPTION: {0}", e.Message);
                }
                catch (WebException e)
                {
                    Console.WriteLine("WEB EXCEPTION: {0}", e.Message);
                }
                catch (Exception e) // in case of unexpected error 
                {
                    Console.WriteLine("EXCEPTION COUGHT:\n {0} ", e.StackTrace);
                    Console.WriteLine("TYPE: " + e.GetType());
                }
            }, cancellationToken);

            return Task;
        }

        private async Task RunTaskMethod(CancellationToken cancellationToken)
        {
            Console.WriteLine("Keep location driver loop started");
            isLoopRunning = true;
            

            while (isLoopRunning)
            {
                var position = await GetPositionForCurrentType(singleDriver, _username);

                if (position != null)
                {
                    if (lastCircle == null || (lastCircle.Center.Latitude != position.Latitude || lastCircle.Center.Longitude != position.Longitude))
                    {

                        if (lastCircle != null)
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                modelView.RemoveCircle(circleIndex);
                            });


                        var circle = new TKCircle
                        {
                            Center = new Position(position.Latitude, position.Longitude),
                            Color = userColor,
                            StrokeColor = Color.White,
                            StrokeWidth = ClientConsts.CIRCLE_STROKE_WIDTH,
                            Radius = ClientConsts.CIRCLE_RADIUS,
                        };

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            modelView.AddCircle(circleIndex, circle);
                        });

                        lastCircle = circle;
                    }
                }
                else if (lastCircle != null) // if something went wrong show the last known position
                {
                    var circle = modelView.GetCircleAtIndex(circleIndex);
                    if (circle == null || (circle != null && (circle.Center.Latitude != lastCircle.Center.Latitude || circle.Center.Longitude != lastCircle.Center.Longitude)))
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            modelView.AddCircle(circleIndex, lastCircle);
                        });
                }
                else // add dummy circle if there is no data so it will fill the array
                {
                    var circle = new TKCircle
                    {
                        Center = MapPage.DEFAULT_MAP_POSITION,
                        Color = userColor,
                        StrokeColor = Color.White,
                        StrokeWidth = ClientConsts.CIRCLE_STROKE_WIDTH,
                        Radius = ClientConsts.CIRCLE_RADIUS,
                    };

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        modelView.AddCircle(circleIndex, circle);
                    });

                    lastCircle = circle;
                }

                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(SLEEP_TIME_LOOP_MILISECONDS);
                Console.WriteLine("CALIBRATING circle for {0}", _username);
            }
        }

        public static async Task<Plugin.Geolocator.Abstractions.Position> GetPositionForCurrentType(bool singleDriver, string username)
        {
            // isDriver -> true if will take the phone position, otherwise it will take the position from the db

            if(singleDriver)
            {
                var locator = CrossGeolocator.Current;
                return await locator.GetPositionAsync(TimeSpan.FromMilliseconds(SLEEP_TIME_LOOP_MILISECONDS), null, true);
            } else
            {
                // get last known position of the driver within END_ADMINISTRATOR_LOOK_UP_POSITION_TIME_MILISECONDS days
                long time = START_ADMINISTRATOR_LOOK_UP_POSITION_TIME_HOURS;
                IEnumerable<PositionDB> positions = null;
                List<PositionDB> positionsList = null;
                bool found = false;

                while(time <= END_ADMINISTRATOR_LOOK_UP_POSITION_TIME_HOURS)
                {
                    positions = await QueryPositions.QueryPositionsInLastHours(username, time);
                    if(positions != null)
                    {
                        // break at the first positions found ( test for Count == 1)
                        foreach(var pos in positions)
                        {
                            found = true;
                            break;
                        }

                        if (found) //if a position was found also break from the while loop
                            break;
                    }
                    time += 2;
                }

                if (positions != null && found)
                {
                    positionsList = positions.ToList();
                    positionsList.Sort((a, b) => -a.DateTime.CompareTo(b.DateTime)); //sort in descending order to get the biggest datetime item
                    return new Plugin.Geolocator.Abstractions.Position(positionsList[0].Latitude, positionsList[0].Longitude);
                }
            }

            return null;
        }

    }
}
