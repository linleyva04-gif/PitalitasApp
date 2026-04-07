
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
    async void CerrarCarrito(object sender, EventArgs e)
    {
        // Baja el panel de regreso (Y=0 a Y=800)
        await PanelCarrito.TranslateTo(0, 800, 300, Easing.SinIn);
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
            // 1. Buscamos si el platillo ya está en el carrito
            var itemExistente = carrito.FirstOrDefault(x => x.Producto.id == productoSeleccionado.id);
            if (itemExistente != null)
            {
                // Si ya existe, solo le sumamos 1 a la cantidad
                itemExistente.Cantidad++;

                // TRUCO MAUI: Para que la pantalla note el cambio de cantidad, 
                // a veces hay que "refrescar" el elemento en la lista.
                var index = carrito.IndexOf(itemExistente);
                carrito[index] = itemExistente;
            }
            else
            {
                // Si es un platillo nuevo, lo agregamos a la lista
                carrito.Add(new carrito
                {
                    Producto = productoSeleccionado,
                    Cantidad = 1
                });
            }

            // 2. Actualizamos el globito rojo sumando TODAS las cantidades
            // (No solo la cantidad de items diferentes, sino el total de platillos)
            int totalArticulos = carrito.Sum(x => x.Cantidad);
            LblContadorCarrito.Text = totalArticulos.ToString();
        

        }

    }

}

