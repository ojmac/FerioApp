using System.Net.Http;
using System.Text;
using System.Text.Json;
using FerioApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;


namespace FerioApp.Services
{
    public class MensajeService
    {
        private readonly HttpClient _httpclient;
        private readonly string _baseUrl = "/api/mensajes"; 

        public MensajeService(HttpClient httpClient)
        {
            _httpclient =  httpClient;
        }
        private void AgregarToken()
        {
            var token = UsuarioService.UsuarioActual?.Token;
            if (!string.IsNullOrEmpty(token))
            {
                
                _httpclient.DefaultRequestHeaders.Authorization = null;
                _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Obtiene mensajes
        public async Task<List<Mensaje>> ObtenerMensajesAsync()
        {
            AgregarToken();

            try
            {
                var tipoUsuario = UsuarioService.UsuarioActual.TipoUsuario;
                var url = $"{_baseUrl}?tipoUsuario={tipoUsuario}";

                var response = await _httpclient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(response))
                {
                    return new List<Mensaje>();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<Mensaje>>(response, options);
            }
            catch (Exception)
            {
                return new List<Mensaje>();
            }
        }




        // Envia mensaje
        public async Task<bool> EnviarMensajeAsync(Mensaje mensaje)
        {
            AgregarToken();
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(mensaje), Encoding.UTF8, "application/json");
                var response = await _httpclient.PostAsync(_baseUrl, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;  
            }
        }
    }
}

