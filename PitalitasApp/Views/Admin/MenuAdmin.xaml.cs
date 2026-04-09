
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

        ListaCarrito.ItemsSource = CarritoGlobal.Articulos;
    }

    async void AbrirCarrito(object sender, EventArgs e)
    {
        await PanelCarrito.TranslateTo(0, 0, 300, Easing.SinOut);
        CargarDomiciliosCliente();


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

        // Refrescamos el carrito por si venimos de otra pantalla
        LblContadorCarrito.Text = CarritoGlobal.Articulos.Sum(x => x.Cantidad).ToString();
        ActualizarTotal();

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

                ResetCategorias();
                FrameTodoA.BackgroundColor = Color.FromArgb("#F5F3E9");
                LblTodoA.TextColor = Colors.Black;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No pudimos descargar el menú: " + ex.Message, "OK");
        }

    }

    private void OnAgregarAlCarrito_Clicked(object sender, EventArgs e)
    {
        var boton = (Button)sender;
        var productoSeleccionado = (Producto)boton.BindingContext;

        var itemExistente = CarritoGlobal.Articulos.FirstOrDefault(x => x.Producto.id == productoSeleccionado.id);

        if (itemExistente != null)
        {
            itemExistente.Cantidad++;

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

        LblContadorCarrito.Text = CarritoGlobal.Articulos.Sum(x => x.Cantidad).ToString();
        ActualizarTotal();

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
        var clienteSeleccionado = PickerCliente.SelectedItem as Usuario;
        string tipoEntrega = PickerTipoEntrega.SelectedItem?.ToString();

        if (clienteSeleccionado == null)
        {
            await DisplayAlert("Aviso", "Selecciona un cliente.", "OK"); return;
        }
        if (string.IsNullOrEmpty(tipoEntrega))
        {
            await DisplayAlert("Aviso", "Selecciona tipo de entrega.", "OK"); return;
        }

        int? idDireccionFinal = null;

        if (tipoEntrega == "Envío a domicilio")
        {
            var direccionElegida = PickerDireccion.SelectedItem as Domicilio;
            if (direccionElegida == null)
            {
                await DisplayAlert("Aviso", "Selecciona una dirección de entrega.", "OK");
                return;
            }
            idDireccionFinal = direccionElegida.id;
        }

        try
        {
            var pedido = new Pedido
            {
                id_cliente = (int)clienteSeleccionado.Id, 
                fecha = DateTime.Now,
                total = CarritoGlobal.Articulos.Sum(x => x.Subtotal),
                tipo_pago = PickerMetodoPago.SelectedItem?.ToString() ?? "Efectivo",
                estado = "Pendiente",
                comentario = EditorComentario.Text + " (Admin)",
                tipo_entrega = tipoEntrega,
                id_domicilio = idDireccionFinal 
            };

            var controller = new PedidosController();
            await controller.CrearPedido(pedido);

            await DisplayAlert("¡Éxito!", "Pedido creado correctamente.", "OK");

            CarritoGlobal.Articulos.Clear();
            ActualizarTotal();
            ActualizarContador();
            await PanelCarrito.TranslateTo(0, 800, 300, Easing.SinIn);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnClienteSeleccionado_Changed(object sender, EventArgs e)
    {
        var clienteSeleccionado = PickerCliente.SelectedItem as Usuario;

        if (clienteSeleccionado != null)
        {
            FrameDireccion.IsVisible = true;
            PickerDireccion.ItemsSource = null;

            try
            {
                var clienteSupabase = Login.GetClient();
                var resultados = await clienteSupabase.From<Domicilio>()
                    .Where(x => x.id_usuario == (int)clienteSeleccionado.Id)
                    .Get();

                PickerDireccion.ItemsSource = resultados.Models;

                if (!resultados.Models.Any())
                {
                    await DisplayAlert("Aviso", "Este cliente no tiene direcciones registradas.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudieron cargar las direcciones: " + ex.Message, "OK");
            }
        }
    }

    // en realidad aqui se cargan los clientes(nombres)
    private async Task CargarDomiciliosCliente()
    {
        try
        {
            var clienteSupabase = Login.GetClient();

            //poner en el picker solo a los usuarios con el rol de cliente omitiendo a los afmins
            var resultados = await clienteSupabase.From<Usuario>()
             .Where(x => x.rol == "cliente")
             .Get();

            PickerCliente.ItemsSource = resultados.Models;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Error cargando clientes: " + ex.Message);
        }
    }

    private void OnTipoEntrega_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (PickerTipoEntrega.SelectedItem?.ToString() == "Envío a domicilio")
        {
            FrameCliente.IsVisible = true; 
        }
        else
        {
            FrameCliente.IsVisible = false; 
            PickerCliente.SelectedItem = null; 
        }
    }


    void ActualizarTotal()
    {
        double total = carrito.Sum(i => i.Subtotal);
        LblTotal.Text = $"${total:F2}";
    }

}

