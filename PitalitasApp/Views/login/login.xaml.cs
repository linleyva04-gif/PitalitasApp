using PitalitasApp.Controllers;

namespace PitalitasApp.Views.login;

public partial class LoginView : ContentPage
{
    Login login = new Login();

    // Variable para evitar que el usuario pique el botón dos veces rápido
    private bool estaProcesandoLogin = false;

    public LoginView()
    {
        InitializeComponent();
    }

    // OnAppearing se ejecuta SIEMPRE que esta pantalla está a punto de mostrarse
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await login.Conectar();

        // Verificamos si Supabase ya tiene una sesión guardada en el celular
        var clienteSupabase = Login.GetClient();
        if (clienteSupabase != null && clienteSupabase.Auth.CurrentSession != null)
        {
            // Si ya hay sesión, nos saltamos el login y evaluamos a dónde mandarlo
            await ProcesarRedireccionUsuario();
        }
    }

    private async void OnGoogleLoginClicked(object sender, TappedEventArgs e)
    {
        // 1. Bloqueo anti doble-clic
        if (estaProcesandoLogin) return;
        estaProcesandoLogin = true;

        // 2. Animación táctil (Reemplaza al Hover que no funciona en celulares)
        await GoogleButton.ScaleTo(0.95, 100);
        await GoogleButton.ScaleTo(1.0, 100);

        // 3. Proceso de Login
        bool exito = await login.IniciarSesionConGoogle();

        if (exito)
        {
            await ProcesarRedireccionUsuario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo iniciar sesión con Google", "OK");
        }

        // Liberamos el botón por si falló y quiere volver a intentar
        estaProcesandoLogin = false;
    }

    // Separamos esta lógica para poder reusarla si el usuario ya tenía sesión
    private async Task ProcesarRedireccionUsuario()
    {
        var userController = new Usuarios(Login.GetClient());
        await userController.CrearUsuarioSiNoExiste();

        var usuario = await userController.ObtenerUsuarioActual();

        if (usuario != null)
        {
            if (usuario.rol == "admin")
            {
                Application.Current.MainPage = new MenuAdmin();
            }
            else if (usuario.rol == "cliente")
            {
                // Como MenuCliente es un FlyoutPage, lo creamos directamente
                Application.Current.MainPage = new MenuCliente();
            }
        }
    }
}