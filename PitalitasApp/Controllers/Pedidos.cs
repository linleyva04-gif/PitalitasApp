using PitalitasApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Supabase;
using System.Threading.Tasks;

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
            await _supabase
                .From<Pedido>()
                .Insert(pedido);
        }
    }
}
