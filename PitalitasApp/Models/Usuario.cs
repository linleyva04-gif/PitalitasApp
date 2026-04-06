using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PitalitasApp.Models
{
    [Table("Usuarios")]
    public class Usuario : BaseModel
    {

        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("Name")] 
        public string Name { get; set; }

        [Column("telefono")]
        public string telefono { get; set; }

        [Column("Domicilio")]
        public string Domicilio { get; set; }

        [Column("Correo")]
        public string Correo { get; set; }

        [Column("rol")]
        public string rol { get; set; }

    }
}
