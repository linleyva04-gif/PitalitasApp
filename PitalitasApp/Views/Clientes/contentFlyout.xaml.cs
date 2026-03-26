using PitalitasApp.Admin;

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

            case "Configuración":
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