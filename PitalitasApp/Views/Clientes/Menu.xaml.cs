using PitalitasApp.Models;
using System.Collections.ObjectModel; // Necesario para la lista dinámica

namespace PitalitasApp;

public partial class MenuCliente : FlyoutPage
{
    // Aquí guardaremos los platillos que se mostrarán en la pantalla
    public ObservableCollection<Producto> CatalogoProductos { get; set; } = new();

    public MenuCliente()
    {
        InitializeComponent();

        // 1. Cargamos datos de prueba (Simulando lo que bajará de Supabase después)
        CatalogoProductos.Add(new Producto { id = 1, nombre = "Hamburguesa Clásica", descripcion = "Carne, queso, lechuga", precio = 120.00, seccion = "Hamburguesas" });
        CatalogoProductos.Add(new Producto { id = 2, nombre = "Boneless BBQ", descripcion = "Bañados en salsa BBQ, incluye aderezo", precio = 145.50, seccion = "Boneless" });
        CatalogoProductos.Add(new Producto { id = 3, nombre = "Dedos de Queso", descripcion = "Orden de 6 piezas con salsa marinara", precio = 85.00, seccion = "Entradas" });
        CatalogoProductos.Add(new Producto { id = 4, nombre = "Limonada Mineral", descripcion = "Bebida refrescante de 500ml", precio = 35.00, seccion = "Bebidas" });

        // 2. Conectamos la lista con la vista (asegúrate de ponerle x:Name="ListaPlatillos" en el XAML)
        ListaPlatillos.ItemsSource = CatalogoProductos;
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

        // Buscamos si el cliente ya tenía este producto en su lista
        var itemExistente = CarritoGlobal.Articulos.FirstOrDefault(x => x.Producto.id == productoSeleccionado.id);

        if (itemExistente != null)
        {
            // Si ya lo tenía, solo le sumamos 1 a la cantidad
            itemExistente.Cantidad++;
        }
        else
        {
            // Si es nuevo, usamos la clase "carrito"
            CarritoGlobal.Articulos.Add(new carrito
            {
                Producto = productoSeleccionado,
                Cantidad = 1
            });
        }

        // Alerta visual para saber que funcionó
        DisplayAlert("¡Listo!", $"Agregaste {productoSeleccionado.nombre} al carrito.", "OK");
    }
}