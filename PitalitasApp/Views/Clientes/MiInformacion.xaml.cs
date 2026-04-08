namespace PitalitasApp.Views.Clientes;

public partial class MiInformacion : ContentPage
{
    public MiInformacion()
    {
        InitializeComponent();

        // CargarDatosUsuario();
    }

    private async void OnGuardarCambios_Clicked(object sender, EventArgs e)
    {
        // Simulamos una carga o validación
        if (string.IsNullOrWhiteSpace(EntNombre.Text) || string.IsNullOrWhiteSpace(EntTelefono.Text))
        {
            await DisplayAlert("Atención", "Por favor llena los campos principales.", "OK");
            return;
        }

        // Aquí iría el código para actualizar en Supabase

        await DisplayAlert("¡Éxito!", "Tu información se ha actualizado correctamente.", "Excelente");
    }
}