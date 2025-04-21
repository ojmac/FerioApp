using Microsoft.Maui.Controls;

namespace FerioApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Método que se ejecuta cuando se hace clic en el botón
        private async void OnNavigateButtonClicked(object sender, EventArgs e)
        {
            // Aquí se navega a UsuarioPage usando Shell
            await Shell.Current.GoToAsync(new UsuarioPage());
        }
    }
}
