using PitalitasApp.Controllers;

namespace PitalitasApp.Views.login;

public partial class LoginView : ContentPage
{
    //el controlador
    Login login = new Login();
    public LoginView()
	{
		InitializeComponent();
        InicializarSupabase();
    }

    //evento para inicializar supabase cuando se abra la pagina
    private async void InicializarSupabase()
    {
        await login.Conectar();
    }

    //evento para ingresar con google 
    private async void OnGoogleLoginClicked(object sender, TappedEventArgs e)
    {
        bool exito = await login.IniciarSesionConGoogle();

        if (exito)
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
                    Application.Current.MainPage = new MenuCliente();
                }
            }
        }
        else
        {
            await DisplayAlert("Error", "No se pudo iniciar sesión con Google", "OK");
        }
    }


    //esto no funciona no se por que se supone que es como un hover
    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        GoogleButton.BackgroundColor = Color.FromArgb("#E8E6DA");
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        GoogleButton.BackgroundColor = Color.FromArgb("#F5F3E9");
    }
}
