using Microsoft.Maui.Controls;
using System;


namespace FerioApp.Models
{
    public partial class UsuarioPage : ContentPage
    {
        private readonly ApiService _apiService;

        public UsuarioPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        // Se ejecutará cuando la página esté completamente cargada
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Obtener información del usuario con ID 5
            var usuarioInfo = await _apiService.GetUsuarioAsync(5);

            // Mostrar la información en el Label
            UsuarioLabel.Text = usuarioInfo;
        }
    }
}
