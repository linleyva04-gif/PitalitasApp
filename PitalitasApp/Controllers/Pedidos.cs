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
                await Task.Delay(2500);

                await EnviarV1("¡Nuevo pedido!", "Pedido recibido, rapido que era para ayer");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }


        //obtener los pedidos de la base de datos en una lista para ponerlos en el grid
        public async Task<List<Pedido>> ObtenerPedidos()
        {
            try
            {
                var resultado = await _supabase
                    .From<Pedido>()
                    .Select("*, cliente_info:Usuarios(*)") 
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
            try
            {
                var token = await GetAccessToken();
                var client = new HttpClient();

                var url = "https://fcm.googleapis.com/v1/projects/pitalitasapp-cdb72/messages:send";

                var body = new
                {
                    message = new
                    {
                        topic = "nuevos_pedidos",
                        notification = new
                        {
                            title = titulo,
                            body = mensaje
                        },
                        android = new
                        {
                            priority = "high",
                            notification = new
                            {
                                channel_id = "default_channel_id",
                                sound = "default",
                                click_action = "TOP_LEVEL_ACCESSIBILITY_ACTION" 
                            }
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                System.Diagnostics.Debug.WriteLine("Notificación enviada con éxito a Firebase");
            }
            catch (Exception ex)
            {
                throw new Exception("Error en Firebase: " + ex.Message);
            }
        }

    }
}
