using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitalitasApp.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public double Total {  get; set; }
        public string TipoPago { get; set; }
        public string Estatus {  get; set; }
        public string Comentario { get; set; }


    }
}
