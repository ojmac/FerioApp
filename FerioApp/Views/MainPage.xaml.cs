using Microsoft.Maui.Controls;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FerioApp.Models;
using FerioApp.Services;


namespace FerioApp
{
    public partial class MainPage : ContentPage
    {
        private readonly PerfilPage _perfilPage;
        private Usuario _userProfile = UsuarioService.UsuarioActual;
        private readonly MensajesPage _mensajesPage;
        private readonly StandsPage _standsPage;
        private readonly StandService _standService;
        private readonly MapPage _mapPage;
        private readonly ControlPage _controlPage;  


        public MainPage(PerfilPage perfilPage, MensajesPage mensajesPage, StandsPage standsPage, StandService standService, MapPage mapPage, ControlPage controlPage)
        {
            InitializeComponent();
            _perfilPage = perfilPage;
            _mensajesPage = mensajesPage;
            _standsPage = standsPage;
            _standService = standService;
            _mapPage = mapPage;
            _controlPage = controlPage; 
            ConfigurePage();
            Shell.SetBackgroundColor(this, Colors.Grey);
            Shell.SetTitleColor(this, Colors.White);
        }
        

       
        private void ConfigurePage()
        {
            // para personalizar la vista según el tipo de usuario, no se si lo usaré
            // Ejemplo: Mostrar/ocultar botones basados en permisos, accesibilidad, etc.
        }

        // Navega al Chatbot
        private async void OnChatbotClicked(object sender, EventArgs e)
        {
            // await Navigation.PushAsync(new ChatbotPage()); 

        }
        // Navega al Perfil
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            UsuarioService.UsuarioActual.Token = token; 

            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Error", "Token no encontrado. Por favor, inicia sesión.", "OK");
                await Shell.Current.GoToAsync("//loginPage");
                return;
            }

            
            var userRole = GetUserRoleFromToken(token);
            _userProfile.Role = userRole; 

            await Navigation.PushAsync(_perfilPage);
        }

        private string GetUserRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token); 

           
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return roleClaim;
        }

        // Navega al Mapa Interactivo
        private async void OnMapClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_mapPage); 
        }

        // Navega a la Información de Expositores
        private async void OnStandsClicked(object sender, EventArgs e)
        {
           await Navigation.PushAsync(_standsPage); 
        }

            // Navega al Cuadro de Control (para organizadores)
        private async void OnControlPanelClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_controlPage); 
        }
        private async void OnBuzonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_mensajesPage); 
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cerrar Sesión", "¿Seguro que deseas cerrar sesión?", "Sí", "No");

            if (confirm)
            {
                try
                {
                    
                    SecureStorage.Remove("auth_token");
                    SecureStorage.Remove("user_data");

                    UsuarioService.LimpiarUsuario();
                    

                    await Shell.Current.GoToAsync("//loginPage");
                    Application.Current.MainPage = new AppShell();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo cerrar sesión correctamente: {ex.Message}", "OK");
                }
            }
        }
    

    }
}
