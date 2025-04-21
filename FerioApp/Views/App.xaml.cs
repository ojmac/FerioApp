namespace FerioApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("App constructor reached");

            MainPage = new AppShell();
        }
    }
}
