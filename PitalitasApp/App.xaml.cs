using PitalitasApp.Admin;
using PitalitasApp.Views.login;

namespace PitalitasApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl / VkV + XU9AclRHQmpWfFN0Q3NbdVpxflBPcDwsT3RfQFtjTX5ad0FnWn5beHVRTmtfUg ==");
            MainPage = new LoginView();
        }
    }
}
