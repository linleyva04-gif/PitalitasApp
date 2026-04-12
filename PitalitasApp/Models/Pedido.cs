using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitalitasApp.Models
{
    [Supabase.Postgrest.Attributes.Table("pedidos")]
    public class Pedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }
        public int id_cliente { get; set; }
        public DateTime fecha { get; set; }
        public double total {  get; set; }
        public string tipo_pago { get; set; }

        [Supabase.Postgrest.Attributes.Column("estado")]
        public string estado {  get; set; }
        public string comentario { get; set; }

        public string tipo_entrega { get; set; } // "Domicilio", "Recoger", "Comedor"

        public int? id_domicilio { get; set; } // Puede ser null si es para recoger o comedor


        //info del cliente para colocarla en la tabla de pedidos
        [Reference(typeof(Usuario))]
        public Usuario cliente_info { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string NombreCliente => cliente_info?.Name ?? "Cargando...";

    }
}
