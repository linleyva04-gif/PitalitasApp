using PitalitasApp.Models;

namespace PitalitasApp;

public partial class MenuCliente : FlyoutPage
{
	public MenuCliente()
	{
		InitializeComponent();

    }

    private void OnSelectedItem(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item == null)
            return;

        Detail = new NavigationPage((Page)Activator.CreateInstance(item.ClientTargetType));
        IsPresented = false;
    }
}