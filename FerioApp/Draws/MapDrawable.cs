using FerioApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerioApp.Draws
{
    public class MapDrawable : IDrawable
    {
        public List<Stand> Stands { get; set; } = new();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (var stand in Stands)
            {
                // Elegimos color según la categoría
                canvas.FillColor = GetColorByCategorias(stand.CategoriaIds);

                // Dibujamos el Stand como rectángulo
                canvas.FillRectangle(
                    (float)stand.PosX * 40,  // Escalamos un poco
                    (float)stand.PosY * 40,
                    (float)stand.Width * 40,
                    (float)stand.Height * 40
                );

                // Opcional: dibujamos el nombre
                canvas.FontColor = Colors.White;
                canvas.FontSize = 12;
                canvas.DrawString(
                    stand.Nombre,
                    (float)stand.PosX * 40 + 5,
                    (float)stand.PosY * 40 + 5,
                    (float)stand.Width * 40 - 10,
                    (float)stand.Height * 40 - 10,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Top
                );
            }
        }
        private Color GetColorByCategorias(List<Categoria> categorias)
        {
            if (categorias.Any(c => c.Nombre == "Baño"))
                return Colors.LightBlue;
            if (categorias.Any(c => c.Nombre == "Cafetería"))
                return Colors.Orange;
            if (categorias.Any(c => c.Nombre == "Información"))
                return Colors.Green;

            return Colors.Gray;
        }


    }
}
