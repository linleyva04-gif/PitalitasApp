using System.Collections.ObjectModel;
using PitalitasApp.Models;
using PitalitasApp.Controllers;
namespace PitalitasApp;

public partial class MenuAdmin : FlyoutPage
{
    private readonly PlatillosController _controller = new PlatillosController();
    public ObservableCollection<Producto> productos = new();

    public MenuAdmin()
	{
		InitializeComponent();
        ListaCarrito.ItemsSource = productos;
    }

    async void AbrirCarrito(object sender, EventArgs e)
    {
        await PanelCarrito.TranslateTo(0, 0, 300, Easing.SinOut);
    }


    private void OnSelectedItem(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item == null)
            return;

        Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
        IsPresented = false;
    }
}