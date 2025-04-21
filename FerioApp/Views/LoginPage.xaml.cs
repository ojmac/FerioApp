using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using FerioApp.Models;
using FerioApp.Services;
using System.Diagnostics;

namespace FerioApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _httpClient = new(); 
        Usuario usuario = new Usuario();

        public LoginPage()
        {
            InitializeComponent();
            Shell.SetBackgroundColor(this, Color.FromArgb("#DEE1E6")); 
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var email = emailEntry.Text?.Trim();
            var password = passwordEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Por favor, ingresa tu correo y contraseña.", "OK");
                return;
            }

            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;

            bool success = await AuthenticateUser(email, password);
            string strValue = success.ToString();
            

            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;

            if (success)
            {
                await Shell.Current.GoToAsync("//mainPage");
                //await Shell.Current.GoToAsync("//MensajesPage");
            }
            else
            {
                await DisplayAlert("Error", "Credenciales incorrectas. Inténtalo de nuevo.", "OK");
            }
        }
        private async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                var json = JsonSerializer.Serialize(new { email, password });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7117/api/Auth/login", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", $"Error de autenticación ({(int)response.StatusCode}). Respuesta: {responseBody}", "OK");
                    return false;
                }

                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody);

                if (!string.IsNullOrEmpty(authResponse?.Token))
                {
                   
                    await SecureStorage.SetAsync("auth_token", authResponse.Token);

                    
                    var userId = ExtractUserIdFromToken(authResponse.Token);
                   

                    if (userId > 0)
                    {
                        
                        usuario = await FetchUserProfile(userId);

                        if (usuario != null)
                        {
                            
                            UsuarioService.ActualizarUsuario(usuario);
                           

                           
                            string ProfileJson = JsonSerializer.Serialize(usuario);
                            await SecureStorage.SetAsync("user_data", ProfileJson);
                        }
                        else
                        {
                            await DisplayAlert("Error", "No se pudo cargar el perfil del usuario.", "OK");
                            return false;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo extraer el ID del usuario del token.", "OK");
                        return false;
                    }

                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Error", $"No se pudo conectar con el servidor: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
            }

            return false;
        }

        private async Task<Usuario> FetchUserProfile(int userId)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    await DisplayAlert("Error", "No se encontró un token válido.", "OK");
                    return null;
                }
     
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7117/api/usuarios/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", $"No se pudo obtener los datos del usuario ({(int)response.StatusCode}).", "OK");
                    return null;
                }
                
                string responseBody = await response.Content.ReadAsStringAsync();
                
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var usuario = JsonSerializer.Deserialize<Usuario>(responseBody, options);
                

                return usuario;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener el perfil: {ex.Message}", "OK");
                
                return null;
            }
        }


        private int ExtractUserIdFromToken(string token)
        {
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                return int.TryParse(userIdClaim, out int userId) ? userId : 0;
            }
            catch
            {
                return 0; 
            }
        }


        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//register");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            
            emailEntry.Text = string.Empty;
            passwordEntry.Text = string.Empty;
        }
    }


    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty; 
        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; } = DateTime.UtcNow; 
    }
}
