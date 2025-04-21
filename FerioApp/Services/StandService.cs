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
        private readonly HttpClient _httpclient ;

        public StandService(HttpClient httpClient)
        {
            _httpclient = httpClient;
            var token = UsuarioService.UsuarioActual?.Token;

            if (!string.IsNullOrEmpty(token))
            {
                _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private void AgregarToken()
        {
            var token = UsuarioService.UsuarioActual?.Token;
            if (!string.IsNullOrEmpty(token))
            {
                // Borra primero para evitar duplicados
                _httpclient.DefaultRequestHeaders.Authorization = null;
                _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }


        // Obtiene la lista de stands desde la API
        public async Task<List<Stand>> GetStands()
        {
            try
            {
                var response = await _httpclient.GetAsync("/api/Stands");


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();


                    Debug.WriteLine("Respuesta de la API:");
                    Debug.WriteLine(responseString);

                    var jsonArray = JsonSerializer.Deserialize<List<JsonElement>>(responseString);


                    var stands = new List<Stand>();


                    foreach (var element in jsonArray)
                    {
                        var stand = JsonSerializer.Deserialize<Stand>(element.GetRawText());
                        Debug.WriteLine($"Stand: {stand.Nombre}, {stand.Descripcion}, {stand.Ubicacion}, {stand.Logo}");
                        stands.Add(stand);
                    }

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


        // Agrega un nuevo stand
        public async Task<Boolean> AddStand(StandCreateModel stand)
        {
            try
            {

                AgregarToken();

                var json = JsonSerializer.Serialize(stand);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpclient.PostAsync("/api/Stands/", content);
               
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

            var response = await _httpclient.DeleteAsync($"/api/Stands/{id}");


            var errorContent = await response.Content.ReadAsStringAsync();

        }
        // Obtiene un stand por su ID
        public async Task<StandUpdateModel> GetStandById(int id)
        {
           
            AgregarToken();
            try
            {
                return await _httpclient.GetFromJsonAsync<StandUpdateModel>($"/api/Stands/{id}");
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
                var response = await _httpclient.PutAsJsonAsync($"/api/Stands/{id}", updatedStand);
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



