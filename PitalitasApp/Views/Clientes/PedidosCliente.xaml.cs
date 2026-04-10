using PitalitasApp.Models;
using PitalitasApp.Controllers;
using System.Collections.ObjectModel;

namespace PitalitasApp.Views.Clientes;

public partial class PedidosCliente : ContentPage
{
    public ObservableCollection<PedidoExtended> ListaPedidos { get; set; } = new();

    public PedidosCliente()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarHistorial();
    }

    private async Task CargarHistorial()
    {
        try
        {
            var userController = new Usuarios(Login.GetClient());
            var usuarioActual = await userController.ObtenerUsuarioActual();

            if (usuarioActual != null)
            {
                var cliente = Login.GetClient();

                // Pedimos a Supabase SOLO los pedidos de este usuario, ordenados del más nuevo al más viejo
                var resultados = await cliente.From<Pedido>()
                    .Where(x => x.id_cliente == (int)usuarioActual.Id)
                    .Order(x => x.fecha, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                ListaPedidos.Clear();
                foreach (var p in resultados.Models)
                {
                    // Envolvemos el pedido normal en nuestra clase extendida
                    ListaPedidos.Add(new PedidoExtended(p));
                }

                // Refrescamos el Grid
                dataGridHistorial.ItemsSource = null;
                dataGridHistorial.ItemsSource = ListaPedidos;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No pudimos cargar tu historial: " + ex.Message, "OK");
        }
    }

    private async void OnCancelarPedido_Clicked(object sender, EventArgs e)
    {
        var boton = (Button)sender;
        var pedido = (PedidoExtended)boton.CommandParameter;

        // Doble check de seguridad por si acaso
        if (pedido.estado != "Pendiente")
        {
            await DisplayAlert("Aviso", "Este pedido ya está en preparación y no puede ser cancelado.", "OK");
            return;
        }

        bool confirmar = await DisplayAlert("Confirmar", "¿Estás seguro de que deseas cancelar este pedido?", "Sí, cancelar", "Volver");

        if (confirmar)
        {
            try
            {
                var cliente = Login.GetClient();
                // Actualizamos solo el estado en Supabase
                await cliente.From<Pedido>()
                    .Where(x => x.id == pedido.id)
                    .Set(x => x.estado, "Cancelado")
                    .Update();

                await DisplayAlert("Cancelado", "Tu pedido ha sido cancelado correctamente.", "OK");

                // Volvemos a descargar la lista para que desaparezca el botón
                await CargarHistorial();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo cancelar el pedido: " + ex.Message, "OK");
            }
        }
    }
}
public class PedidoExtended : Pedido
{
    // Esta es la magia: Le dice al botón de cancelar si debe mostrarse o no
    public bool PuedeCancelar => estado == "Pendiente";

    public PedidoExtended(Pedido p)
    {
        this.id = p.id;
        this.id_cliente = p.id_cliente;
        this.fecha = p.fecha;
        this.total = p.total;
        this.estado = p.estado;
        this.comentario = p.comentario;
        this.tipo_pago = p.tipo_pago;
        this.tipo_entrega = p.tipo_entrega;
        this.id_domicilio = p.id_domicilio;
    }
}