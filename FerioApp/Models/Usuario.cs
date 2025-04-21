using System.Text.Json.Serialization;

namespace FerioApp.Models
{
    public class Usuario
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("contrasena")]
        public string? Contrasena { get; set; }

        [JsonPropertyName("tipoUsuario")]
        public TipoUsuario TipoUsuario { get; set; }

        [JsonPropertyName("fotoPerfil")]
        public string? FotoPerfil { get; set; }

        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("empresa")]
        public string? Empresa { get; set; }

        [JsonPropertyName("standId")]
        public int? StandId { get; set; }

        [JsonPropertyName("standsFavoritos")]
        public List<int>? StandsFavoritos { get; set; } = new List<int>();

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("tokenExpiration")]
        public string? TokenExpiracion { get; set; }

        [JsonPropertyName("lastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [JsonIgnore]
        public bool IsExpositorOrOrganizer => TipoUsuario == TipoUsuario.Expositor;

        [JsonIgnore]
        public bool IsOrganizer => TipoUsuario == TipoUsuario.Organizador;
    }

    public enum TipoUsuario
    {
        Visitante = 0,
        Expositor = 1,
        Organizador = 2
    }
}
