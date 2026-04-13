using PitalitasApp.Models;
using PitalitasApp.Controllers;

namespace PitalitasApp.Views.Clientes;

public partial class MiInformacion : ContentPage
{
    public MiInformacion()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var userController = new PitalitasApp.Controllers.Usuarios(Login.GetClient());
        var usuarioActual = await userController.ObtenerUsuarioActual();

        if (usuarioActual != null)
        {
            // Llenamos los campos de texto automáticamente
            EntryNombre.Text = usuarioActual.Name;
            EntryTelefono.Text = usuarioActual.telefono;
        }
        await CargarDirecciones();
    }

    private async void OnGuardarCambios_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNombre.Text) || string.IsNullOrWhiteSpace(EntryTelefono.Text))
        {
            await DisplayAlert("Atención", "Por favor llena los campos principales.", "OK");
            return;
        }

        // Aquí iría el código para actualizar en Supabase (pendiente)

        await DisplayAlert("¡Éxito!", "Tu información se ha actualizado.", "Excelente");
    }

    private async Task CargarDirecciones()
    {
        try
        {
            var userController = new Usuarios(Login.GetClient());
            var usuarioActual = await userController.ObtenerUsuarioActual();

            if (usuarioActual != null)
            {
                var cliente = Login.GetClient();
                var resultados = await cliente
                    .From<Domicilio>()
                    .Where(x => x.id_usuario == (int)usuarioActual.Id)
                    .Get();

                ListaDirecciones.ItemsSource = resultados.Models;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No pudimos cargar tus direcciones: " + ex.Message, "OK");
        }
    }

    private void OnMostrarFormularioDireccion_Clicked(object sender, EventArgs e)
    {
        FormularioNuevaDireccion.IsVisible = true;
    }

    private void OnCancelarDireccion_Clicked(object sender, EventArgs e)
    {
        LimpiarFormularioDireccion();
    }

    private async void OnGuardarDireccion_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryCalle.Text) || string.IsNullOrWhiteSpace(EntryColonia.Text))
        {
            await DisplayAlert("Aviso", "Por favor llena la calle y la colonia.", "OK");
            return;
        }

        try
        {
            var userController = new Usuarios(Login.GetClient());
            var usuarioActual = await userController.ObtenerUsuarioActual();

            var nuevoDomicilio = new Domicilio
            {
                id_usuario = (int)usuarioActual.Id,
                calle_numero = EntryCalle.Text,
                colonia = EntryColonia.Text,
                referencia = EntryReferencia.Text
            };

            var cliente = Login.GetClient();
            await cliente.From<Domicilio>().Insert(nuevoDomicilio);

            await DisplayAlert("Éxito", "Dirección guardada correctamente", "OK");

            LimpiarFormularioDireccion();
            await CargarDirecciones(); // Refrescamos la lista
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo guardar: " + ex.Message, "OK");
        }
    }

    private void LimpiarFormularioDireccion()
    {
        EntryCalle.Text = "";
        EntryColonia.Text = "";
        EntryReferencia.Text = "";
        FormularioNuevaDireccion.IsVisible = false;
    }
    private async void OnActualizarDatos_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNombre.Text))
        {
            await DisplayAlert("Aviso", "El nombre no puede estar vacío", "OK");
            return;
        }

        try
        {
            var userController = new PitalitasApp.Controllers.Usuarios(Login.GetClient());
            var usuarioActual = await userController.ObtenerUsuarioActual();

            if (usuarioActual != null)
            {
                var clienteSupabase = Login.GetClient();

                // 1. Descargamos tu registro exacto de la base de datos
                var respuesta = await clienteSupabase.From<Usuario>()
                                    .Where(x => x.Id == usuarioActual.Id)
                                    .Get();

                var usuarioDb = respuesta.Models.FirstOrDefault();

                if (usuarioDb != null)
                {
                    // 2. Le cambiamos el nombre
                    usuarioDb.Name = EntryNombre.Text;

                    // 3. Subimos el registro actualizado
                    await clienteSupabase.From<Usuario>().Update(usuarioDb);

                    await DisplayAlert("¡Éxito!", "Tus datos han sido actualizados.", "Genial");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudieron actualizar los datos: " + ex.Message, "OK");
        }
    }
    private async void OnEliminarDireccion_Clicked(object sender, EventArgs e)
    {
        // 1. Obtenemos el domicilio que el usuario seleccionó
        var boton = (ImageButton)sender;
        var domicilioABorrar = (Domicilio)boton.CommandParameter;

        if (domicilioABorrar == null) return;

        // 2. Pedimos confirmación (siempre es buena práctica)
        bool confirmar = await DisplayAlert("Confirmar", $"¿Estás seguro de eliminar la dirección en {domicilioABorrar.calle_numero}?", "Sí, eliminar", "Cancelar");

        if (confirmar)
        {
            try
            {
                // 3. Llamamos a Supabase para borrar por ID
                var cliente = Login.GetClient();
                await cliente
                    .From<Domicilio>()
                    .Where(x => x.id == domicilioABorrar.id)
                    .Delete();

                // 4. Refrescamos la lista automáticamente
                await CargarDirecciones();

                // Un pequeño aviso de éxito (opcional)
                // await DisplayAlert("Éxito", "Dirección eliminada.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo eliminar: " + ex.Message, "OK");
            }
        }
    }
}