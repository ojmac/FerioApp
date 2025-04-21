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

        
        public async Task<List<Categoria>> GetCategoriasAsync()
        {
            var url = "/api/Stands/Categoria"; 
            return await _httpClient.GetFromJsonAsync<List<Categoria>>(url);
        }
    }
}
