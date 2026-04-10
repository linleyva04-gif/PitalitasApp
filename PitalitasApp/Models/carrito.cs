using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PitalitasApp.Models
{
    // Agregamos INotifyPropertyChanged para que la pantalla reaccione a los cambios
    public class carrito : INotifyPropertyChanged
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal => Producto?.precio * Cantidad ?? 0;

        private string _comentarioEspecial;
        public string ComentarioEspecial
        {
            get => _comentarioEspecial;
            set { _comentarioEspecial = value; OnPropertyChanged(); }
        }

        // Esta variable controla si la caja se ve o está oculta
        private bool _mostrarComentario;
        public bool MostrarComentario
        {
            get => _mostrarComentario;
            set { _mostrarComentario = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}