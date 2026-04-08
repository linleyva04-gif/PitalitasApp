namespace PitalitasApp.Views.Clientes;

public partial class LogoutModal : ContentPage
{
    public LogoutModal()
    {
        InitializeComponent();
    }

    private async void OnCancelar_Clicked(object sender, EventArgs e)
    {
        // Solo quitamos el popup de la pantalla
        await Navigation.PopModalAsync();
    }

    private async void OnConfirmar_Clicked(object sender, EventArgs e)
    {
        // 1. Cerramos sesión en Supabase
        var cliente = PitalitasApp.Controllers.Login.GetClient();
        if (cliente != null)
        {
            await cliente.Auth.SignOut();
        }

        // 2. Mandamos al usuario a la pantalla de login
        Application.Current.MainPage = new PitalitasApp.Views.login.LoginView();
    }
}