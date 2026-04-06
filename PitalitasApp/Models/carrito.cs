using System;
using System.Collections.Generic;
using System.Text;

namespace PitalitasApp.Models
{
    public class carrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal => Producto.precio * Cantidad;
    }
}
