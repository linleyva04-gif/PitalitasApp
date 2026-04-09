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
            }
        }

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);

            Log.Debug(TAG, $"Nuevo Token: {token}");
        }
    }
}