using PitalitasApp.Controllers;
using PitalitasApp.Models;
using System.Collections.ObjectModel; // Necesario para la lista dinámica

namespace PitalitasApp;

public partial class MenuCliente : FlyoutPage
{
    private List<Producto> _listaMaestraPlatillos = new();
    // Aquí guardaremos los platillos que se mostrarán en la pantalla
    public ObservableCollection<Producto> CatalogoProductos { get; set; } = new();



    //uso de controlador de platillos
    private readonly PlatillosController _controller = new PlatillosController();

    public ObservableCollection<carrito> carrito = new();


    public MenuCliente()
    {
        InitializeComponent();

        ListaCarrito.ItemsSource = CarritoGlobal.Articulos;
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
                _listaMaestraPlatillos = productos.ToList();

                ListaPlatillos.ItemsSource = _listaMaestraPlatillos;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No pudimos traer el menú: " + ex.Message, "OK");

        }

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

    // 3. El evento que se disparará al darle al botón naranja "+"
    private void OnAgregarAlCarrito_Clicked(object sender, EventArgs e)
    {
        var boton = (Button)sender;
        var productoSeleccionado = (Producto)boton.BindingContext;

        var itemExistente = CarritoGlobal.Articulos.FirstOrDefault(x => x.Producto.id == productoSeleccionado.id);

        if (itemExistente != null)
        {
            itemExistente.Cantidad++;

            // Refrescamos la posición para que la interfaz se entere del cambio
            var index = CarritoGlobal.Articulos.IndexOf(itemExistente);
            CarritoGlobal.Articulos[index] = itemExistente;
        }
        else
        {
            CarritoGlobal.Articulos.Add(new carrito
            {
                Producto = productoSeleccionado,
                Cantidad = 1
            });
        }

        // Actualizamos el número en la burbuja naranja 
        LblContadorCarrito.Text = CarritoGlobal.Articulos.Sum(x => x.Cantidad).ToString();
    }
    private void OnCategoriaTapped(object sender, TappedEventArgs e)
    {
        string categoriaSeleccionada = e.Parameter.ToString();

        ResetCategorias();

        if (categoriaSeleccionada == "Todo")
        {
            FrameTodo.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblTodo.TextColor = Colors.Black;

            ListaPlatillos.ItemsSource = _listaMaestraPlatillos;
        }
        else if (categoriaSeleccionada == "Entradas")
        {
            FrameEntradas.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblEntradas.TextColor = Colors.Black;
        }
        else if (categoriaSeleccionada == "Hamburguesas")
        {
            FrameHamburguesas.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblHamburguesas.TextColor = Colors.Black;
        }
        else if (categoriaSeleccionada == "Bebidas")
        {
            FrameBebidas.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblBebidas.TextColor = Colors.Black;
        }

        if (categoriaSeleccionada != "Todo")
        {
            ListaPlatillos.ItemsSource = _listaMaestraPlatillos
                .Where(p => p.seccion == categoriaSeleccionada)
                .ToList();
        }
    }

    private void ResetCategorias()
    {
        FrameTodo.BackgroundColor = Color.FromArgb("#1E1E1E");
        FrameEntradas.BackgroundColor = Color.FromArgb("#1E1E1E");
        FrameHamburguesas.BackgroundColor = Color.FromArgb("#1E1E1E");
        FrameBebidas.BackgroundColor = Color.FromArgb("#1E1E1E");

        LblTodo.TextColor = Colors.White;
        LblEntradas.TextColor = Colors.White;
        LblHamburguesas.TextColor = Colors.White;
        LblBebidas.TextColor = Colors.White;
    }

    private void OnEliminarItemCarrito_Invoked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var itemABorrar = swipeItem.CommandParameter as carrito;

        if (itemABorrar != null)
        {
            CarritoGlobal.Articulos.Remove(itemABorrar);

            ActualizarContador();
        }
    }

    private void ActualizarContador()
    {
        int totalItems = CarritoGlobal.Articulos.Sum(i => i.Cantidad);

        LblContadorCarrito.Text = totalItems.ToString();

    }

}