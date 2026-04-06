using PitalitasApp.Controllers;
using PitalitasApp.Models;

namespace PitalitasApp.Admin;

public partial class AltaView : ContentPage
{

    public AltaView()
	{
		InitializeComponent();
    }

    private async void Guardar_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(PrecioEntry.Text, out double precio)) return;

        var nuevoProducto = new Producto
        {
            nombre = NameEntry.Text,
            precio = precio,
            seccion = CategoriaPicker.SelectedItem?.ToString(),
            descripcion = EditorDescripcion.Text
        };

        try
        {
            var controller = new PlatillosController();

            await controller.AgregarPlatillo(nuevoProducto);

            await DisplayAlert("Éxito", "Guardado en Supabase", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}