using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FerioApp
{
    public partial class RegisterPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://localhost:7117/api/Auth/register";

        public RegisterPage()
        {
            InitializeComponent(); // Conecta el XAML con el código
        }

        // Evento al hacer clic en "Registrarse"
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;

            // Validar los campos
            if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
                string.IsNullOrWhiteSpace(emailEntry.Text) ||
                string.IsNullOrWhiteSpace(passwordEntry.Text) ||
                string.IsNullOrWhiteSpace(confirmPasswordEntry.Text))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                activityIndicator.IsVisible = false;
                activityIndicator.IsRunning = false;
                return;
            }

            if (passwordEntry.Text != confirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                activityIndicator.IsVisible = false;
                activityIndicator.IsRunning = false;
                return;
            }

            // Crear objeto del usuario a registrar
            var newUser = new
            {
                // **CAMBIOS AQUÍ**: Ajustamos los nombres según el formato esperado por la API
                nombre = nameEntry.Text, // Antes: "Nombre"
                email = emailEntry.Text, // Antes: "Email"
                password = passwordEntry.Text, // Antes: "Contraseña"
                tipoUsuario = 0 // Visitante por defecto (mantiene igual)
            };

            try
            {
                // Serializar objeto a JSON
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(newUser),
                    Encoding.UTF8,
                    "application/json");

                // Enviar solicitud POST a la API
                var response = await _httpClient.PostAsync(ApiUrl, jsonContent);
                await DisplayAlert("Error", jsonContent.ToString(), "OK");
                await DisplayAlert("Error", response.ToString(), "OK");

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Usuario registrado correctamente.", "OK");

                    // Navegar al Login después de un registro exitoso
                    await Navigation.PushAsync(new LoginPage());
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Error al registrar usuario: {errorMessage}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se produjo un error: {ex.Message}", "OK");
            }
            finally
            {
                activityIndicator.IsVisible = false;
                activityIndicator.IsRunning = false;
            }
        }

        // Evento al hacer clic en "¿Ya tienes una cuenta? Inicia sesión"
        private async void OnNavigateToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}
