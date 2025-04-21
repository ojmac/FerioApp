using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerioApp.Models
{
    public class Mensaje
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
        public int UsuarioId { get; set; }
        public int? StandId { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public string? Empresa { get; set; }
    }

}
