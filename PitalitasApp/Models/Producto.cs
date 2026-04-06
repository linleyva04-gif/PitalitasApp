using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PitalitasApp.Models
{
    [Table("Platillos")]
    public class Producto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public double precio { get; set; }
        public string seccion { get; set; }
    }
}
