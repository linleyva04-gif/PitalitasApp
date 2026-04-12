using Android.Content.PM;
using PitalitasApp.Controllers;
using PitalitasApp.Models;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Helper;
using System.Collections.ObjectModel;

namespace PitalitasApp.Views.Admin;
public partial class Pedidos : ContentPage
{
    public ObservableCollection<Pedido> ListaPedidos { get; set; } = new ObservableCollection<Pedido>();

    private readonly PedidosController _controller = new PedidosController();
    public Pedidos()
	{
		InitializeComponent();
        BindingContext = this;
    }

    private async void OnEstadoChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var nuevoEstado = picker.SelectedItem?.ToString();

        var pedido = (Pedido)picker.BindingContext;

        if (pedido != null && !string.IsNullOrEmpty(nuevoEstado))
        {
            try
            {
                pedido.estado = nuevoEstado;
                await _controller.ActualizarEstadoPedido(pedido.id, nuevoEstado);

                string topico = "estado_pedidos";
                string tituloNoti = $"Actualización de Pedido #{pedido.id}";
                string mensajeNoti = $"Tu pedido ahora está: {nuevoEstado}";

                await _controller.EnviarV1(topico, tituloNoti, mensajeNoti);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo actualizar el estado: " + ex.Message, "OK");
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CargarPedidos();

    }

    private async Task CargarPedidos()
    {
        try
        {
            var pedidosDb = await _controller.ObtenerPedidos();

            ListaPedidos.Clear();
            foreach (var p in pedidosDb)
            {
                ListaPedidos.Add(p);
            }

            dataGridPedidos.ItemsSource = null;
            dataGridPedidos.ItemsSource = ListaPedidos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los pedidos: {ex.Message}", "OK");
        }
    }


    private void dataGridPedidos_QueryRowHeight(object sender, DataGridQueryRowHeightEventArgs e)
    {
        if (e.RowIndex > 0)
        {
            e.Height = dataGridPedidos.GetRowHeight(e.RowIndex);
            e.Handled = true;
        }
    }

}