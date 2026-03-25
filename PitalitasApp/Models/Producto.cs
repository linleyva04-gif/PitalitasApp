using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitalitasApp.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Name { get; set; }
        public string Descripccion { get; set; }
        public double Precio { get; set; }
        public string UrlFoto { get; set; }
        public bool Estado { get; set; }
    }
}
