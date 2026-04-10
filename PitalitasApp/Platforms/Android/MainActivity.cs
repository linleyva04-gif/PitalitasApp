using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase.Messaging;
using Android.Gms.Tasks;

namespace PitalitasApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Permission.Granted)
                {
                    RequestPermissions(new string[] { Android.Manifest.Permission.PostNotifications }, 0);
                }
            }
            base.OnCreate(savedInstanceState);

            FirebaseMessaging.Instance.GetToken().AddOnCompleteListener(new MyOnCompleteListener((task) =>
            {
                if (task.IsSuccessful)
                {
                    var token = task.Result.ToString();
                    System.Diagnostics.Debug.WriteLine($"[FIREBASE_TOKEN] {token}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener el token de Firebase");
                }
            }));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = "default_channel_id";
                var channelName = "Notificaciones de Pedidos";
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.High);
                notificationManager.CreateNotificationChannel(channel);
            }
           
        }
    }

    public class MyOnCompleteListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
    {
        private readonly System.Action<Android.Gms.Tasks.Task> _callback;

        public MyOnCompleteListener(System.Action<Android.Gms.Tasks.Task> callback)
        {
            _callback = callback;
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            _callback?.Invoke(task);
        }
    }
}