using Android.App;
using Android.Util;
using Firebase.Messaging;


namespace PitalitasApp.Platforms.Android
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "FCM";

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            Log.Debug(TAG, "Mensaje recibido");

            if (message.GetNotification() != null)
            {
                var title = message.GetNotification().Title;
                var body = message.GetNotification().Body;

                Log.Debug(TAG, $"Titulo: {title}");
                Log.Debug(TAG, $"Mensaje: {body}");

                //esta parte de aqui es para la noti dentro de la app
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var activity = Platform.CurrentActivity;
                    if (activity != null)
                    {
                        var view = activity.Window.DecorView.FindViewById(global::Android.Resource.Id.Content);

                        var snack = global::Google.Android.Material.Snackbar.Snackbar.Make(view, $"{title}: {body}", global::Google.Android.Material.Snackbar.Snackbar.LengthLong);

                        var snackView = snack.View;
                        var lp = new global::Android.Widget.FrameLayout.LayoutParams(snackView.LayoutParameters);

                        lp.Gravity = global::Android.Views.GravityFlags.Top;

                        lp.TopMargin = 50;

                        snackView.LayoutParameters = lp;

                        snack.SetAction("OK", v => { }).Show();
                    }
                });
            }
        }

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);

            Log.Debug(TAG, $"Nuevo Token: {token}");
        }
    }
}