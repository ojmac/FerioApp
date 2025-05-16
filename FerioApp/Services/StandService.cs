using FerioApp.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FerioApp.Services
{
    public class StandService
    {
        private readonly HttpClient _httpClient;
        CategoriaService _catService;

        public StandService(HttpClient httpClient, CategoriaService categoriaService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _catService = categoriaService;
            var token = UsuarioService.UsuarioActual?.Token;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private void AgregarToken()
        {
            var token = UsuarioService.UsuarioActual?.Token;
            if (!string.IsNullOrEmpty(token))
            {
                // Borra primero para evitar duplicados
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }


        // Obtiene la lista de stands desde la API
        public async Task<List<Stand>> GetStands()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7117/api/Stands");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine("Respuesta de la API:");
                    Debug.WriteLine(responseString);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var standsCM = JsonSerializer.Deserialize<List<StandUpdateModel>>(responseString, options);

                    if (standsCM == null)
                    {
                        Debug.WriteLine("Error: No se pudo deserializar la lista de stands.");
                        return new List<Stand>();
                    }

                    // Verificación de datos cargados (debug)
                    foreach (var s in standsCM)
                    {
                        Debug.WriteLine($"Stand: {s.Id}, {s.Nombre}, {s.Ubicacion}, {s.Contacto}");
                    }

                    var stands = await ConvertToStand(standsCM);
                    return stands;
                }
                else
                {
                    Debug.WriteLine($"Error obteniendo stands: {response.ReasonPhrase}");
                    return new List<Stand>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo stands: {ex.Message}");
                return new List<Stand>();
            }
        }


        public async Task<List<Stand>> ConvertToStand(List<StandUpdateModel> models)
        {
            var categoriasDisponibles = await _catService.GetCategoriasAsync();
            Debug.WriteLine($"Total categorías disponibles: {categoriasDisponibles.Count}");
            var result = new List<Stand>();
            foreach (var model in models)
            {
                var stand = new Stand
                {
                    Id = model.Id,
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Logo = model.Logo,
                    Ubicacion = model.Ubicacion,
                    EnlaceWeb = model.EnlaceWeb,
                    Contacto = model.Contacto,
                    UsuarioId = model.UsuarioId,
                    HorarioAtencion = model.HorarioAtencion,
                    PosX = model.PosX,
                    PosY = model.PosY,
                    Width = model.Width,
                    Height = model.Height,
                    CategoriaIds = model.CategoriaIds != null
                        ? categoriasDisponibles.Where(c => model.CategoriaIds.Contains(c.Id)).ToList()
                        : new List<Categoria>()

                };

                result.Add(stand);
            }

            return result;
        }

        // Agrega un nuevo stand
        public async Task<Boolean> AddStand(StandCreateModel stand)
        {
            try
            {

                AgregarToken();

                var json = JsonSerializer.Serialize(stand);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Stands/", content);

                var responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    return false;
                }
            }
            catch (Exception ex)
            {


                return false;
            }
        }

        // Elimina un stand
        public async Task DeleteStand(int id)
        {

            AgregarToken();

            var response = await _httpClient.DeleteAsync($"/api/Stands/{id}");


            var errorContent = await response.Content.ReadAsStringAsync();

        }
        // Obtiene un stand por su ID
        public async Task<StandUpdateModel> GetStandById(int id)
        {

            AgregarToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<StandUpdateModel>($"/api/Stands/{id}");
            }
            catch
            {
                return null;
            }
        }

        //  Actualiza un  Stand
        public async Task<bool> UpdateStand(int id, StandUpdateModel updatedStand)
        {
            AgregarToken();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/Stands/{id}", updatedStand);
                Debug.WriteLine($"Respuesta de la API: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}



