namespace PitalitasApp;

public partial class MenuAdmin : FlyoutPage
{
	public MenuAdmin()
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