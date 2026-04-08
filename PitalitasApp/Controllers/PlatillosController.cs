using System;
using System.Collections.Generic;
using System.Text;
using PitalitasApp.Models;


namespace PitalitasApp.Controllers
{
    public class PlatillosController
    {
        private readonly Supabase.Client _supabase;

        public PlatillosController()
        {
            _supabase = Login.GetClient();
        }

        //metodo para subir imagen a supabase
        public async Task<string> SubirImagen(Stream stream, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                await _supabase.Storage
                    .From("platillos")
                    .Upload(fileBytes, fileName, new Supabase.Storage.FileOptions
                    {
                        Upsert = true,
                        ContentType = "image/jpeg"
                    });
            }

            return _supabase.Storage
                .From("platillos")
                .GetPublicUrl(fileName);
        }

        //esto se usa en el Alta view para agregar platillos
        public async Task AgregarPlatillo(Producto platillo)
        {
            if (_supabase == null)
                throw new Exception("Primero debes iniciar sesión o conectar a Supabase");

            await _supabase.From<Producto>().Insert(platillo);
        }

        //esto se usa en el menu para mostrar los platillos
        public async Task<List<Producto>> ObtenerPlatillos()
        {
            try
            {
                var resultado = await _supabase
                    .From<Producto>()
                    .Get();

                return resultado.Models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener platillos: {ex.Message}");
                return new List<Producto>();
            }
        }
    }
}
