using Microsoft.Maui.Controls;
using System;
using FerioApp.Services;

namespace FerioApp
{
    public partial class UsuarioPage : ContentPage
    {
        private readonly ApiService _apiService;

    

        // Constructor
        public UsuarioPage(ApiService apiService)
        {

            InitializeComponent(); 
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
             // Este método es generado automáticamente cuando usas XAML
        }

        // Se ejecutará cuando la página esté completamente cargada
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Puedes agregar estos chequeos para ver cuál es nulo:
            if (_apiService == null)
            {
                System.Diagnostics.Debug.WriteLine("ApiService es null");
            }
            if (UsuarioLabel == null)
            {
                System.Diagnostics.Debug.WriteLine("UsuarioLabel es null");
            }

            var usuarioInfo = await _apiService.GetUsuarioAsync(5);
            UsuarioLabel.Text = usuarioInfo;
        }
        public async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//mainPage");
        }
    }
}
