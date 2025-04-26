using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace FerioApp.Models;

public class Stand
{
    public int? Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } 

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } 

    [JsonPropertyName("logo")]
    public string Logo { get; set; } 

    [JsonPropertyName("ubicacion")]
    public string Ubicacion { get; set; } 

    [JsonPropertyName("enlaceWeb")]
    public string EnlaceWeb { get; set; } 

    [JsonPropertyName("contacto")]
    public string Contacto { get; set; } 

    [JsonPropertyName("usuarioId")]
    public int UsuarioId { get; set; } 

    [JsonPropertyName("HorarioAtencion")]
    public string HorarioAtencion { get; set; } 

    [JsonPropertyName("categorias")]
    public List<Categoria> CategoriaIds { get; set; } = new List<Categoria>();
    [JsonPropertyName("posX")]
    public int PosX { get; set; }
    [JsonPropertyName("posY")]
    public int PosY { get; set; }
    [JsonPropertyName("width")]
    public int Width { get; set; } = 2;
    [JsonPropertyName("heigth")]
    public int Height { get; set; } = 2;


    public ICommand ViewStandCommand { get; set; }
    
    public List<StandCategoria> StandCategoria { get; set; }


    public Stand()
    {
        ViewStandCommand = new Command(OpenWebPage);
    }

    private async void OpenWebPage()
    {
        await Launcher.OpenAsync(EnlaceWeb); 
    }
}
