using PitalitasApp.Admin;
using PitalitasApp.Models;

namespace PitalitasApp;

public partial class NewPage1 : ContentPage
{
	public NewPage1()
	{
		InitializeComponent();
	}

    private void navMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;

        if (item == null)
            return;

        Page pagina = null;

        switch (item.ClientTitle)
        {
            case "Menú":
                pagina = new Menu();
                break;

            case "Dar de alta platillo":
                pagina = new AltaView();
                break;

            case "Clientes":
                pagina = new Clientes();
                break;

            case "Reportes pedidos":
                pagina = new ReportesView();
                break;

            case "Configuraci�n":
                pagina = new configuracionView();
                break;
        }

        if (pagina != null)
        {
            var flyout = (FlyoutPage)Application.Current.MainPage;
            flyout.Detail = new NavigationPage(pagina);
            flyout.IsPresented = false;
        }
    }
}