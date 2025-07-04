using System.Collections.Generic;

namespace FerioApp.Models
{
    public class StandUpdateModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Logo { get; set; }
        public string Ubicacion { get; set; }
        public string HorarioAtencion { get; set; }
        public string EnlaceWeb { get; set; }
        public string Contacto { get; set; }
        public int UsuarioId { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; } = 2;
        public int Height { get; set; } = 2;
        public List<int> CategoriaIds { get; set; }
    }
}
