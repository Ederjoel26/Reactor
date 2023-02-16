using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Views;
using Firebase.Messaging;
using Microsoft.Maui.Storage;
using ReactorMaui.Pages;
using System;

namespace ReactorMaui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        internal static readonly string Channel_ID = "TestChannel";
        internal static readonly int NotificationID = 101;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetStatusBarColor(Android.Graphics.Color.Rgb(0, 0, 0));

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key == "NavigationID")
                    {
                        string idValue = Intent.Extras.GetString(key);
                        if (Preferences.ContainsKey("NavigationID"))
                            Preferences.Remove("NavigationID");

                        Preferences.Set("NavigationID", idValue);
                    }
                }
            }
            CreateNotificationChannel();
            
        }

        private void CreateNotificationChannel()
        {
            if (OperatingSystem.IsOSPlatformVersionAtLeast("android", 26))
            {
                var channel = new NotificationChannel(Channel_ID, "Test Notfication Channel", NotificationImportance.Default);

                var notificaitonManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
                notificaitonManager.CreateNotificationChannel(channel);
            }
        }
    }
}