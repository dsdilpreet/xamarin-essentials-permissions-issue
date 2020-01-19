using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;

namespace LocationTestShell.ViewModels
{
    class LocationViewModel : BaseViewModel
    {

        // Monitor location Task 
        CancellationTokenSource monitorLocationTokenSource;
        CancellationToken monitorLocationToken;
        Task monitorLocationTask;


        public LocationViewModel()
        {
            RequestLocationPermissionsCommand = new Command(async() => await RequestLocation());

            StartTaskCommand = new Command(async() => await StartTask());

            StartTaskCommand.Execute(null);
        }

        private async Task StartTask()
        {
            await StartMonitorLocationTask();
        }

        private async Task RequestLocation()
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            //MainThread.BeginInvokeOnMainThread(async() =>
            //{
            //    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            //});
        }

        private async Task StartMonitorLocationTask()
        {

            monitorLocationTokenSource = new CancellationTokenSource();
            monitorLocationToken = monitorLocationTokenSource.Token;

            Action action = new Action(async () =>
            {
                while (!monitorLocationToken.IsCancellationRequested)
                {

                    if (!locationPermissionView)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            DisplayLocationView();
                        });
                        Debug.WriteLine("Displaying Location View");
                    }

                    Debug.WriteLine($"Task Status: {monitorLocationTask.Status}");
                    await Task.Delay(1);
                }
            });

            monitorLocationTask = Task.Factory.StartNew(action, monitorLocationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            await Task.Delay(1);
        }

        private void DisplayLocationView()
        {
            LocationPermissionView = true;
        }

        private bool locationPermissionView;
        public bool LocationPermissionView
        {
            get => locationPermissionView;
            set
            {
                locationPermissionView = value;
                OnPropertyChanged();
            }
        }

        public Command RequestLocationPermissionsCommand { get; }
        public Command StartTaskCommand { get; }
    }
}
