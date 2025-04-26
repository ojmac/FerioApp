using System.Text.Json.Serialization;

namespace FerioApp.Models
{
    public class StandCreateModel
    {
        

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; }

        [JsonPropertyName("ubicacion")]
        public string Ubicacion { get; set; }

        [JsonPropertyName("horarioAtencion")]
        public string HorarioAtencion { get; set; }

        [JsonPropertyName("enlaceWeb")]
        public string EnlaceWeb { get; set; }

        [JsonPropertyName("usuarioId")]
        public int UsuarioId { get; set; } 

        [JsonPropertyName("contacto")]
        public string Contacto { get; set; }
        [JsonPropertyName("posX")]
        public int PosX { get; set; }
        [JsonPropertyName("posY")]
        public int PosY { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; } = 2;
        [JsonPropertyName("height")]
        public int Height { get; set; } = 2;

        [JsonPropertyName("categoriaIds")]
        public List<int> CategoriaIds { get; set; }
    }
}
