using System.Collections.ObjectModel;
using System.Linq;

namespace PitalitasApp.Models
{
    public static class CarritoGlobal
    {
        public static ObservableCollection<carrito> Articulos { get; set; } = new();

        public static double TotalCuenta => Articulos.Sum(item => item.Subtotal);
    }
}