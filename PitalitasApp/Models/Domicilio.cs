using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PitalitasApp.Models
{
    [Table("domicilios")]
    public class Domicilio : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("id_usuario")]
        public int id_usuario { get; set; } // Enlaza con el ID del cliente

        [Column("calle_numero")]
        public string calle_numero { get; set; }

        [Column("colonia")]
        public string colonia { get; set; }

        [Column("referencia")]
        public string referencia { get; set; }
    }
}