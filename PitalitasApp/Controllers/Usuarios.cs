using PitalitasApp.Models;
using System.Threading.Tasks;

namespace PitalitasApp.Controllers
{
    public class Usuarios
    {
        private readonly Supabase.Client _supabase;

        public Usuarios(Supabase.Client supabase)
        {
            _supabase = supabase;
        } 

        //obtener usuarios para ver si son admin o clientes
        public async Task<Usuario?> ObtenerUsuarioActual()
        {
            var user = _supabase.Auth.CurrentUser;

            if (user == null)
                return null;

            var resultado = await _supabase
                .From<Usuario>()
                .Where(x => x.Correo == user.Email)
                .Get();

            return resultado.Models.FirstOrDefault();
        }


        //evento para crear usuarios si no existe uno con ese corrweo
        public async Task CrearUsuarioSiNoExiste()
        {
            var user = _supabase.Auth.CurrentUser;

            if (user == null)
                return;

            //revisar si existe el suaurio
            var existe = await _supabase
                .From<Usuario>()
                .Where(x => x.Correo == user.Email)
                .Get();


            //si no existe se inserta
            if (existe.Models.Count == 0)
            {
                string nombre = "";

                if (user.UserMetadata.ContainsKey("full_name"))
                {
                    nombre = user.UserMetadata["full_name"]?.ToString();
                }

                //se crean en automatico como clientes
                var nuevo = new Usuario
                {
                    Name = nombre,
                    Correo = user.Email,
                    rol = "cliente"
                };


                //insertar el nuevo cliente en el supabase en la tabla usuarios con el modelo Usuario
                await _supabase
                    .From<Usuario>()
                    .Insert(nuevo);
            }
        }
    }
}