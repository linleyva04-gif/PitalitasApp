using PitalitasApp.Controllers;
using Firebase.Messaging;
 
namespace PitalitasApp.Views.login;

public partial class LoginView : ContentPage
{
    Login login = new Login();

    private bool estaProcesandoLogin = false;

    public LoginView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await login.Conectar();

        var clienteSupabase = Login.GetClient();
        if (clienteSupabase != null && clienteSupabase.Auth.CurrentSession != null)
        {
            await ProcesarRedireccionUsuario();
        }
    }

    private async void OnGoogleLoginClicked(object sender, TappedEventArgs e)
    {
        if (estaProcesandoLogin) return;
        estaProcesandoLogin = true;

        await GoogleButton.ScaleTo(0.95, 100);
        await GoogleButton.ScaleTo(1.0, 100);

        bool exito = await login.IniciarSesionConGoogle();

        if (exito)
        {
            await ProcesarRedireccionUsuario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo iniciar sesión con Google", "OK");
        }

        estaProcesandoLogin = false;
    }

    private async Task ProcesarRedireccionUsuario()
    {
        var userController = new Usuarios(Login.GetClient());
        await userController.CrearUsuarioSiNoExiste();

        var usuario = await userController.ObtenerUsuarioActual();

        if (usuario != null)
        {
            if (usuario.rol == "admin")
            {
                //suscribir a las notis de nuevos pedidos
                FirebaseMessaging.Instance.SubscribeToTopic("nuevos_pedidos");
                Application.Current.MainPage = new MenuAdmin();
            }
            else if (usuario.rol == "cliente")
            {
                Application.Current.MainPage = new MenuCliente();

                //unsubscribe para que se quiten los clientes que ya estaban en nuevos pedidos
                FirebaseMessaging.Instance.UnsubscribeFromTopic("nuevos_pedidos");
                FirebaseMessaging.Instance.SubscribeToTopic("estado_pedidos");

            }
        }
    }
} 