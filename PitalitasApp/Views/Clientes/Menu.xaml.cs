using PitalitasApp.Controllers;
using PitalitasApp.Models;
using System.Collections.ObjectModel; // Necesario para la lista dinámica

namespace PitalitasApp;

public partial class MenuCliente : FlyoutPage
{
    private List<Producto> _listaMaestraPlatillos = new();


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

        // Refrescamos el carrito por si venimos de otra pantalla
        LblContadorCarrito.Text = CarritoGlobal.Articulos.Sum(x => x.Cantidad).ToString();
        ActualizarTotal();
    }

    private async Task CargarPlatillos()
    {
        try
        {
            // Le pedimos los datos a Supabase usando el controlador de tu compañera
            var productos = await _controller.ObtenerPlatillos();

            if (productos != null && productos.Any())
            {
                // Guardamos la lista completa para poder usar los filtros ("Entradas", "Bebidas", etc.) sin volver a descargar
                _listaMaestraPlatillos = productos.ToList();

                // Pintamos la lista en la pantalla
                ListaPlatillos.ItemsSource = _listaMaestraPlatillos;

                // Reiniciamos los filtros para que "Todo" esté seleccionado visualmente
                ResetCategorias();
                FrameTodo.BackgroundColor = Color.FromArgb("#F5F3E9");
                LblTodo.TextColor = Colors.Black;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No pudimos descargar el menú: " + ex.Message, "OK");
        }

    }


    async void AbrirCarrito(object sender, EventArgs e)
    {
        await PanelCarrito.TranslateTo(0, 0, 300, Easing.SinOut);
        await CargarDomiciliosCliente();

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

        // Actualizamos el número en la burbuja naranja y el total
        LblContadorCarrito.Text = CarritoGlobal.Articulos.Sum(x => x.Cantidad).ToString();
        ActualizarTotal();

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
        // 1. Validamos que hayan elegido el tipo de entrega
        string tipoEntrega = PickerTipoEntrega.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(tipoEntrega))
        {
            await DisplayAlert("Aviso", "Por favor elige cómo quieres recibir tu pedido.", "OK");
            return;
        }

        // 2. Si es a domicilio, validamos que hayan elegido una dirección
        int? idDomicilioSeleccionado = null;
        if (tipoEntrega == "Envío a domicilio")
        {
            var domicilioSeleccionado = PickerDireccion.SelectedItem as Domicilio;
            if (domicilioSeleccionado == null)
            {
                await DisplayAlert("Aviso", "Por favor selecciona una dirección de entrega.", "OK");
                return;
            }
            idDomicilioSeleccionado = domicilioSeleccionado.id;
        }

        try
        {
            var userController = new PitalitasApp.Controllers.Usuarios(Login.GetClient());
            var usuarioActual = await userController.ObtenerUsuarioActual();
            double totalCarrito = CarritoGlobal.Articulos.Sum(x => x.Producto.precio * x.Cantidad);

            var pedido = new Pedido
            {
                id_cliente = (int)usuarioActual.Id,
                fecha = DateTime.Now,
                total = totalCarrito,
                tipo_pago = PickerMetodoPago.SelectedItem?.ToString() ?? "Efectivo",
                estado = "Pendiente",
                comentario = EditorComentario.Text,
                tipo_entrega = tipoEntrega,               
                id_domicilio = idDomicilioSeleccionado    
            };

            var controller = new PitalitasApp.Controllers.PedidosController();
            await controller.CrearPedido(pedido);

            await DisplayAlert("¡Listo!", "Tu pedido ha sido enviado a la cocina.", "Excelente");

            // Limpiamos todo
            CarritoGlobal.Articulos.Clear();
            LblContadorCarrito.Text = "0";
            ActualizarTotal();
            PickerTipoEntrega.SelectedItem = null;
            PickerDireccion.SelectedItem = null;
            EditorComentario.Text = "";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Ocurrió un problema: " + ex.Message, "OK");
        }
    }
    // Cargamos las direcciones para tenerlas listas en el Picker
    private async Task CargarDomiciliosCliente()
    {
        var userController = new Usuarios(Login.GetClient());
        var usuarioActual = await userController.ObtenerUsuarioActual();

        if (usuarioActual != null)
        {
            var cliente = Login.GetClient();
            var resultados = await cliente.From<Domicilio>()
                .Where(x => x.id_usuario == (int)usuarioActual.Id)
                .Get();

            PickerDireccion.ItemsSource = resultados.Models;
        }
    }

    // Este evento se dispara cuando eligen "Envío" o "Recoger"
    private void OnTipoEntrega_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (PickerTipoEntrega.SelectedItem?.ToString() == "Envío a domicilio")
        {
            FrameDireccion.IsVisible = true; // Mostramos las direcciones
        }
        else
        {
            FrameDireccion.IsVisible = false; // Ocultamos
            PickerDireccion.SelectedItem = null; // Limpiamos la selección
        }
    }

    void ActualizarTotal()
    {
        // CORRECCIÓN: Sumamos desde CarritoGlobal
        double total = CarritoGlobal.Articulos.Sum(i => i.Subtotal);
        LblTotal.Text = $"${total:F2}";
    }

}