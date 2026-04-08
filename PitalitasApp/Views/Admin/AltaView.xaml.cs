using PitalitasApp.Controllers;
using PitalitasApp.Models;

namespace PitalitasApp.Admin;

public partial class AltaView : ContentPage
{
    private FileResult _fotoSeleccionada; 

    public AltaView()
	{
		InitializeComponent();
    }

    private async void Guardar_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(PrecioEntry.Text, out double precio)) return;

        if (_fotoSeleccionada == null)
        {
            await DisplayAlert("Aviso", "Por favor selecciona una imagen", "OK");
            return;
        }


        try
        {
            var controller = new PlatillosController();
            string urlImagen = string.Empty;

            using (var stream = await _fotoSeleccionada.OpenReadAsync())
            {
                string nombreArchivo = $"{Guid.NewGuid()}.jpg";
                urlImagen = await controller.SubirImagen(stream, nombreArchivo);
            }

            //se crea un nuevo producto
            var nuevoProducto = new Producto
            {
                nombre = NameEntry.Text,
                precio = precio,
                seccion = CategoriaPicker.SelectedItem?.ToString(),
                descripcion = EditorDescripcion.Text,
                imagen_url = urlImagen 
            };

            await controller.AgregarPlatillo(nuevoProducto);

            await DisplayAlert("Éxito", "Guardado en Supabase", "OK");

            _fotoSeleccionada = null;
            ImgPreview.Source = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.ToString(), "OK");
        }
    }

    private async void SelectImageTapped_Clicked(object sender, TappedEventArgs e)
    {
        _fotoSeleccionada = await MediaPicker.Default.PickPhotoAsync();

        if (_fotoSeleccionada != null)
        {
            var stream = await _fotoSeleccionada.OpenReadAsync();

            ImgPreview.Source = ImageSource.FromStream(() => stream);

            PlaceholderStack.IsVisible = false;
            ImgPreview.IsVisible = true;
        }
    }



}