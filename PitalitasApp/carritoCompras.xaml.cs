using PitalitasApp.Models;
using System.Collections.ObjectModel;
using PitalitasApp.Models;

namespace PitalitasApp;

public partial class carritoCompras : ContentPage
{

	public carritoCompras()
	{
		InitializeComponent();
	}

    public static ObservableCollection<carrito> Items { get; set; } = new ObservableCollection<carrito>();

    public static void AgregarProducto(Producto producto)
    {
        var itemExistente = Items.FirstOrDefault(x => x.Producto.id == producto.id);

        if (itemExistente != null)
        {
            itemExistente.Cantidad++;
        }
        else
        {
            Items.Add(new carrito { Producto = producto, Cantidad = 1 });
        }
    }

    public static int TotalItems => Items.Sum(x => x.Cantidad);
}