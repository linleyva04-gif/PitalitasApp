namespace PitalitasApp.Views.Admin;

using PitalitasApp.Models;
using System.Collections.ObjectModel;
using PitalitasApp.Controllers;

public partial class Pedidos : ContentPage
{
    public ObservableCollection<Pedido> ListaPedidos { get; set; } = new ObservableCollection<Pedido>();

    private readonly PedidosController _controller = new PedidosController();
    public Pedidos()
	{
		InitializeComponent();
        BindingContext = this;
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
            var pedidosDb = await _controller.ObtenerPlatillos();

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



}