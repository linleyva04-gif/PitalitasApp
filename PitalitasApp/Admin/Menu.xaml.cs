namespace PitalitasApp;

public partial class Menu : FlyoutPage
{
	public Menu()
	{
		InitializeComponent();

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