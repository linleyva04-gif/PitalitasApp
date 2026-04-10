using PitalitasApp.Admin;
using PitalitasApp.Models;
using PitalitasApp.Views.Admin;
using PitalitasApp.Views.Clientes;

namespace PitalitasApp;

public partial class contentFlyoutAdmin : ContentPage
{
    public contentFlyoutAdmin()
	{
		InitializeComponent();
    }

    private async void OnCerrarSesion_Tapped(object sender, TappedEventArgs e)
    {
        // Abrimos nuestra pantalla modal por encima de todo
        await Navigation.PushModalAsync(new LogoutModal());
    }

    private void navMenu_SelectionChangedAdmin(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;

        if (item == null)
            return;

        Page pagina = null;

        switch (item.Title)
        {
            case "Menú":
                pagina = new MenuAdmin();
                break;

            case "Dar de alta platillo":
                pagina = new AltaView();
                break;

            case "Pedidos":
                pagina = new Pedidos();
                break;

        }

        if (pagina != null)
        {
            if (Parent is FlyoutPage flyout)
            {
                flyout.Detail = new NavigationPage(pagina);
                flyout.IsPresented = false;
            }
            else if (Application.Current.MainPage is FlyoutPage mainFlyout)
            {
                mainFlyout.Detail = new NavigationPage(pagina);
                mainFlyout.IsPresented = false;
            }
        }

        ((CollectionView)sender).SelectedItem = null;
    }
}