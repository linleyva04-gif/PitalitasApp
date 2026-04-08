using PitalitasApp.Models;
using PitalitasApp.Views.Clientes;

namespace PitalitasApp.Views.Clientes;

public partial class contentFlyoutCliente : ContentPage
{
    // Esta línea es clave para que el menú principal pueda escuchar los clics
    public CollectionView MenuList => navMenu;

    public contentFlyoutCliente()
    {
        InitializeComponent();

        // Llenamos la lista manualmente para evitar bugs de MAUI
        navMenu.ItemsSource = new List<FlyoutPageItem>
        {
            new FlyoutPageItem { Title = "Menú Principal", IconSource = "menu.png" },
            new FlyoutPageItem { Title = "Mis Pedidos", IconSource = "report.png" },
            new FlyoutPageItem { Title = "Mi Información", IconSource = "customer.png" }
        };
    }
    private async void OnCerrarSesion_Tapped(object sender, TappedEventArgs e)
    {
        // Abrimos nuestra pantalla modal por encima de todo
        await Navigation.PushModalAsync(new LogoutModal());
    }

    private void navMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;

        if (item == null)
            return;

        Page pagina = null;

        // Evaluamos qué botón presionó el cliente
        switch (item.Title)
        {
            case "Menú Principal":
                pagina = new MenuCliente();
                break;

            case "Mis Pedidos":
                pagina = new PedidosCliente();
                break;

            case "Mi Información":
                pagina = new MiInformacion();
                break;
        }

        if (pagina != null)
        {
            var flyout = (FlyoutPage)Application.Current.MainPage;
            flyout.Detail = new NavigationPage(pagina);
            flyout.IsPresented = false;
        }

        navMenu.SelectedItem = null;
    }
}