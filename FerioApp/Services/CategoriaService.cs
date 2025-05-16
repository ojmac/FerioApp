using FerioApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FerioApp.Services
{
    public class CategoriaService
    {
        private readonly HttpClient _httpClient;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            var response = await _httpClient.GetAsync("/api/Stands/categorias");
            response.EnsureSuccessStatusCode(); // lanza excepción si no es 2xx
            return await response.Content.ReadFromJsonAsync<List<Categoria>>();
        }
    }
}
