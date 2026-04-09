using Google.Apis.Auth.OAuth2; 
using PitalitasApp.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PitalitasApp.Controllers
{
    public class PedidosController
    {
        private readonly Supabase.Client _supabase;

        public PedidosController()
        {
            _supabase = Login.GetClient();
        }

        public async Task CrearPedido(Pedido pedido)
        {
            try
            {
                await _supabase.From<Pedido>().Insert(pedido);
                await EnviarV1("¡Nuevo pedido!", "Pedido recibido");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error Real", ex.Message, "OK");
            }
        }


        //obtener los pedidos de la base de datos en una lista para ponerlos en el grid
        public async Task<List<Pedido>> ObtenerPlatillos()
        {
            try
            {
                var resultado = await _supabase
                    .From<Pedido>()
                    .Get();

                return resultado.Models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los pedidos: {ex.Message}");
                return new List<Pedido>();
            }
        }


        //NOTIFICACIONES
        public async Task<string> GetAccessToken()
        {
            using (var stream = await FileSystem.OpenAppPackageFileAsync("firebase_key.json"))
            {
                var credential = GoogleCredential.FromStream(stream)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            }
        }

        public async Task EnviarV1(string titulo, string mensaje)
        {
            var token = await GetAccessToken();
            var client = new HttpClient();
            var url = "https://fcm.googleapis.com/v1/projects/pitalitasapp-cdb72/messages:send";

            var body = new
            {
                message = new
                {
                    topic = "nuevos_pedidos",
                    notification = new { title = titulo, body = mensaje },
                    android = new
                    {
                        notification = new
                        {
                            channel_id = "default_channel_id", 
                            priority = "high",
                            sound = "default"
                        }
                    }
                }
            };

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
        }

    }
}
