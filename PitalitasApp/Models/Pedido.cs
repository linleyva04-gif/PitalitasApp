using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitalitasApp.Models
{
    [Table("pedidos")]
    public class Pedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }
        public int id_cliente { get; set; }
        public DateTime fecha { get; set; }
        public double total {  get; set; }
        public string tipo_pago { get; set; }
        public string estado {  get; set; }
        public string comentario { get; set; }

        [Column("tipo_entrega")]
        public string tipo_entrega { get; set; } // "Domicilio", "Recoger", "Comedor"

        [Column("id_domicilio")]
        public int? id_domicilio { get; set; } // Puede ser null si es para recoger o comedor

        [Column("numero_mesa")]
        public string numero_mesa { get; set; } // Puede ser null si es a domicilio
    }
}
