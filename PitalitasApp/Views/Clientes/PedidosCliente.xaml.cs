using System.Collections.ObjectModel;
    
namespace PitalitasApp.Views.Clientes;

public partial class PedidosCliente : ContentPage
{
    // Clase temporal para probar el diseño
    public class PedidoFalso
    {
        public string Folio { get; set; }
        public string Estatus { get; set; }
        public string DescripcionResumen { get; set; }
        public DateTime Fecha { get; set; }
        public double Total { get; set; }
    }

    public ObservableCollection<PedidoFalso> MisPedidosHistoricos { get; set; } = new();

    public PedidosCliente()
    {
        InitializeComponent();

        // 1. Llenamos con datos de prueba
        MisPedidosHistoricos.Add(new PedidoFalso
        {
            Folio = "A-492",
            Estatus = "En Preparación",
            DescripcionResumen = "1x Hamburguesa Clásica, 1x Limonada",
            Fecha = DateTime.Now.AddMinutes(-15),
            Total = 155.00
        });

        MisPedidosHistoricos.Add(new PedidoFalso
        {
            Folio = "A-480",
            Estatus = "Entregado",
            DescripcionResumen = "2x Boneless BBQ, 1x Dedos de Queso",
            Fecha = DateTime.Now.AddDays(-2),
            Total = 376.00
        });

        // 2. Conectamos la lista visual con los datos
        ListaPedidos.ItemsSource = MisPedidosHistoricos;
    }
}