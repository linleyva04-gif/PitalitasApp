
using PitalitasApp.Controllers;
using PitalitasApp.Models;
using System.Collections.ObjectModel;

namespace PitalitasApp;

public partial class MenuAdmin : FlyoutPage
{

    private readonly PlatillosController _controller = new PlatillosController();

    public ObservableCollection<carrito> carrito = new();

    public MenuAdmin()
    {
        InitializeComponent();

        ListaCarrito.ItemsSource = carrito;
    }

    async void AbrirCarrito(object sender, EventArgs e)
    {
        await PanelCarrito.TranslateTo(0, 0, 300, Easing.SinOut);

    }

    private void OnSelectedItem(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;

        if (item == null)

            return;



        Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));

        IsPresented = false;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CargarPlatillos();

    }

    private async Task CargarPlatillos()
    {
        try

        {

            var productos = await _controller.ObtenerPlatillos();



            if (productos != null && productos.Any())

            {

                ListaPlatillos.ItemsSource = productos;

            }

        }

        catch (Exception ex)

        {

            await DisplayAlert("Error", "No pudimos traer el men�: " + ex.Message, "OK");

        }

    }

    private void OnAgregarAlCarritoClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        var productoSeleccionado = (Producto)button.CommandParameter;



        if (productoSeleccionado != null)

        {

            carrito.Add(new carrito

            {

                Producto = productoSeleccionado,

                Cantidad = 1

            });



            LblContadorCarrito.Text = carrito.Count.ToString();

        }

    }

}

