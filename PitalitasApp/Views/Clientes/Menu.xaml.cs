using PitalitasApp.Models;
using System.Collections.ObjectModel; // Necesario para la lista dinámica

namespace PitalitasApp;

public partial class MenuCliente : FlyoutPage
{
    private List<Producto> _listaMaestraPlatillos = new();
    // Aquí guardaremos los platillos que se mostrarán en la pantalla
    public ObservableCollection<Producto> CatalogoProductos { get; set; } = new();

    public MenuCliente()
    {
        InitializeComponent();

        // 1. Llenamos la lista maestra 
        _listaMaestraPlatillos.Add(new Producto { id = 1, nombre = "Hamburguesa Clásica", descripcion = "Carne, queso, lechuga", precio = 120.00, seccion = "Hamburguesas" });
        _listaMaestraPlatillos.Add(new Producto { id = 2, nombre = "Boneless BBQ", descripcion = "Bañados en salsa BBQ, incluye aderezo", precio = 145.50, seccion = "Boneless" });
        _listaMaestraPlatillos.Add(new Producto { id = 3, nombre = "Dedos de Queso", descripcion = "Orden de 6 piezas con salsa marinara", precio = 85.00, seccion = "Entradas" });
        _listaMaestraPlatillos.Add(new Producto { id = 4, nombre = "Limonada Mineral", descripcion = "Bebida refrescante de 500ml", precio = 35.00, seccion = "Bebidas" });

        // 2. Pasamos esos datos a la lista visual del catálogo
        CatalogoProductos = new ObservableCollection<Producto>(_listaMaestraPlatillos);
        ListaPlatillos.ItemsSource = CatalogoProductos;

        // 3.  Conectamos el diseño del carrito con nuestra memoria global
        ListaCarrito.ItemsSource = CarritoGlobal.Articulos;
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
        // Extraemos la palabra clave que mandamos desde el XAML (ej. "Hamburguesas")
        string categoriaSeleccionada = e.Parameter.ToString();

        if (categoriaSeleccionada == "Todo")
        {
            // Si le pica a "Todo", regresamos la lista completa
            ListaPlatillos.ItemsSource = _listaMaestraPlatillos;
        }
        else
        {
            // Filtramos usando LINQ: solo los platillos cuya sección coincida
            var platillosFiltrados = _listaMaestraPlatillos
                .Where(p => p.seccion == categoriaSeleccionada)
                .ToList();

            ListaPlatillos.ItemsSource = platillosFiltrados;
        }
    }
}