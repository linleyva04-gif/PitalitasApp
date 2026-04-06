using PitalitasApp.Admin;
using PitalitasApp.Views.login;

namespace PitalitasApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LoginView();
        }
    }
}
