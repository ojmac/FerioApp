using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace FerioApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        // Obtiene un usuario
        public async Task<string> GetUsuarioAsync(int id)
        {
            try
            {
                
                var token = Preferences.Get("AuthToken", null);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("No estás autenticado.");

                
                token = token.Trim();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"/api/usuarios/{id}");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error en la solicitud: {response.StatusCode}");

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                
                return $"Error: {ex.Message}";
            }
        }

        // Autencica y recibe un token
        public async Task<string> AuthenticateAsync(string email, string password)
        {
            try
            {   
                var credentials = new { Email = email, Contrasena = password };
         
                var json = JsonSerializer.Serialize(credentials);
                Console.WriteLine($"Error en autenticación: {json}");
                Debug.WriteLine($"Error en autenticación: {json}");
                var content = new StringContent(json,Encoding.UTF8,"application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error en autenticación: {response.StatusCode}");

                var responseBody = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody);

                if (authResponse?.Token == null)
                    throw new Exception("Token inválido.");
     
                var token = authResponse.Token.Trim();
                Preferences.Set("AuthToken", token);

                return token;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private class AuthResponse
        {
            public string Token { get; set; }
        }
    }
}
