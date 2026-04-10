using Android.Content.PM;
using PitalitasApp.Models;
using System.Collections.ObjectModel;
using PitalitasApp.Controllers;

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //aplicar solo el estado horizontal solo a pedidos, se veia muy feo vertical
        Platform.CurrentActivity.RequestedOrientation = ScreenOrientation.Landscape;

        await CargarPedidos();

    }

    //para quitar el estado horizontal
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Platform.CurrentActivity.RequestedOrientation = ScreenOrientation.Unspecified;
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



}