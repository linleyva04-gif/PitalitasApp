
using PitalitasApp.Controllers;
using PitalitasApp.Models;
using System.Collections.ObjectModel;

namespace PitalitasApp;

public partial class MenuAdmin : FlyoutPage
{
    private List<Producto> _listaMaestraPlatillos = new();
    // Aquí guardaremos los platillos que se mostrarán en la pantalla
    public ObservableCollection<Producto> CatalogoProductos { get; set; } = new();

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
                _listaMaestraPlatillos = productos.ToList(); // ← IMPORTANTE

                CatalogoProductos.Clear();

                foreach (var p in productos)
                {
                    CatalogoProductos.Add(p);
                }

                ListaPlatillos.ItemsSource = CatalogoProductos;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No pudimos traer el menú: " + ex.Message, "OK");
        }
    }

    private void OnAgregarAlCarrito_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        var productoSeleccionado = (Producto)button.CommandParameter;

        if (productoSeleccionado != null)
        {
            var itemExistente = carrito.FirstOrDefault(x => x.Producto.id == productoSeleccionado.id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;

                var index = carrito.IndexOf(itemExistente);
                carrito[index] = itemExistente;
            }
            else
            {
                carrito.Add(new carrito
                {
                    Producto = productoSeleccionado,
                    Cantidad = 1
                });
            }

            int totalArticulos = carrito.Sum(x => x.Cantidad);
            ActualizarTotal();
            LblContadorCarrito.Text = totalArticulos.ToString();
        

        }

    }

    private void OnCategoriaTapped(object sender, TappedEventArgs e)
    {
        string categoriaSeleccionada = e.Parameter.ToString();

        ResetCategorias();

        if (categoriaSeleccionada == "Todo")
        {
            FrameTodoA.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblTodoA.TextColor = Colors.Black;

            ListaPlatillos.ItemsSource = _listaMaestraPlatillos;
        }
        else if (categoriaSeleccionada == "Entradas")
        {
            FrameEntradasA.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblEntradasA.TextColor = Colors.Black;
        }
        else if (categoriaSeleccionada == "Hamburguesas")
        {
            FrameHamburguesasA.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblHamburguesasA.TextColor = Colors.Black;
        }
        else if (categoriaSeleccionada == "Bebidas")
        {
            FrameBebidasA.BackgroundColor = Color.FromArgb("#F5F3E9");
            LblBebidasA.TextColor = Colors.Black;
        }

        if (categoriaSeleccionada != "Todo")
        {
            ListaPlatillos.ItemsSource = _listaMaestraPlatillos
                .Where(p => p.seccion == categoriaSeleccionada)
                .ToList();
        }
    }

    void ResetCategorias()
    {
        FrameTodoA.BackgroundColor = Color.FromArgb("#1E1E1E");
        FrameEntradasA.BackgroundColor = Color.FromArgb("#1E1E1E");
        FrameHamburguesasA.BackgroundColor = Color.FromArgb("#1E1E1E");
        FrameBebidasA.BackgroundColor = Color.FromArgb("#1E1E1E");

        LblTodoA.TextColor = Colors.White;
        LblEntradasA.TextColor = Colors.White;
        LblHamburguesasA.TextColor = Colors.White;
        LblBebidasA.TextColor = Colors.White;
    }

    private void OnEliminarItemCarrito_Invoked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var itemABorrar = swipeItem.CommandParameter as carrito;

        if (itemABorrar != null)
        {
            CarritoGlobal.Articulos.Remove(itemABorrar);

            ActualizarTotal();
            ActualizarContador();
        }
    }

    private void ActualizarContador()
    {
        int totalItems = CarritoGlobal.Articulos.Sum(i => i.Cantidad);

        LblContadorCarrito.Text = totalItems.ToString();

    }

    private async void RealizarPedido_Clicked(object sender, EventArgs e)
    {
        try
        {
            double totalCarrito = carrito.Sum(x => x.Producto.precio * x.Cantidad);

            var pedido = new Pedido
            {
                id_cliente = 1,
                fecha = DateTime.Now,
                total = totalCarrito,
                tipo_pago = PickerMetodoPago.SelectedItem?.ToString(),
                estado = "Pendiente",
                comentario = EditorComentario.Text
            };

            var controller = new PedidosController();

            await controller.CrearPedido(pedido);

            await DisplayAlert("Pedido", "Pedido realizado correctamente", "OK");

            carrito.Clear();
            LblContadorCarrito.Text = "0";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    void ActualizarTotal()
    {
        double total = carrito.Sum(i => i.Subtotal);
        LblTotal.Text = $"${total:F2}";
    }

}

