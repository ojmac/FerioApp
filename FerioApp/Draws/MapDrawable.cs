using FerioApp.Models;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace FerioApp.Draws
{
    public class MapDrawable : IDrawable
    {
        public List<Stand> Stands { get; set; } = new();

        public float Scale { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
      
        private const float CellSize = 0.5f;
        private List<GridCell> _camino;


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (Stands == null || Stands.Count == 0)
                return;

            float canvasWidth = dirtyRect.Width;
            float canvasHeight = dirtyRect.Height;

            float mapaAncho = Stands.Max(s => s.PosX + s.Width);
            float mapaAlto = Stands.Max(s => s.PosY + s.Height);

            // Añadir espacio para pasillos grandes (importante en feria de coches)
            float pasilloExtra = 2f;
            mapaAncho += pasilloExtra;
            mapaAlto += pasilloExtra;

            // Calcular el factor de escala para que quepa todo y esté centrado
            Scale = Math.Min(canvasWidth / mapaAncho, canvasHeight / mapaAlto);

            // Centrado dinámico
            OffsetX = (canvasWidth - mapaAncho * Scale) / 2;
            OffsetY = (canvasHeight - mapaAlto * Scale) / 2;

            foreach (var stand in Stands)
            {
                Debug.WriteLine($"Stand {stand.Nombre} tiene {stand.CategoriaIds.Count} categorías.");

                canvas.FillColor = GetColorByCategorias(stand.CategoriaIds);

                float x = OffsetX + stand.PosX * Scale;
                float y = OffsetY + stand.PosY * Scale;
                float width = stand.Width * Scale;
                float height = stand.Height * Scale;

                // Dibujar rectángulo
                canvas.FillRectangle(x, y, width, height);

                // Texto
                canvas.FontColor = Colors.White;
                canvas.FontSize = 12;
                canvas.DrawString(
                    stand.Nombre,
                    x + 5,
                    y + 5,
                    width - 10,
                    height - 10,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Top
                );
                //    // 🔍 Dibuja el grid con colores según walkable o no
                //    float gridSizeX = (int)Math.Ceiling(Stands.Max(s => s.PosX + s.Width) / CellSize);
                //    float gridSizeY = (int)Math.Ceiling(Stands.Max(s => s.PosY + s.Height) / CellSize);

                //    var dummyOrigen = Stands.FirstOrDefault(s => s.CategoriaIds.Any(c => c.Nombre.ToLower().Contains("información")));
                //    var dummyDestino = Stands.FirstOrDefault(s => s.Nombre.ToLower().Contains("audi"));

                //    if (dummyOrigen != null && dummyDestino != null)
                //    {
                //        var tempGrid = CrearGridDePasillos(dummyOrigen, dummyDestino);
                //        for (int x1 = 0; x1 < tempGrid.GetLength(0); x1++)
                //        {
                //            for (int y1 = 0; y1 < tempGrid.GetLength(1); y1++)
                //            {
                //                var cell = tempGrid[x1, y1];
                //                canvas.FillColor = cell.Walkable ? Colors.LightGreen.WithAlpha(0.3f) : Colors.DarkGray.WithAlpha(0.5f);

                //                float cx = OffsetX + x1 * CellSize * Scale;
                //                float cy = OffsetY + y1 * CellSize * Scale;
                //                canvas.FillRectangle(cx, cy, CellSize * Scale, CellSize * Scale);
                //            }
                //        }
                //    }

                //}
                if (_camino != null)
                {
                    DibujarCamino(canvas, _camino);
                }

                // Dibujar el texto "Mapa de la Feria" en la parte derecha
                string textoMapa = "Mapa de la Feria";
                

                // Calcular el ancho del texto basado en la longitud del texto y el tamaño de la fuente
                float fontSize = 32; 
                float textoWidth = textoMapa.Length * fontSize * 0.6f; // Estimación del ancho
                float textoHeight = fontSize; // Estimación de la altura

                // Posicionar el texto en la parte derecha
                float textoX = canvasWidth - textoWidth - 20; // 20px de margen desde el borde derecho
                float textoY = (canvasHeight - textoHeight) / 2;

                // Dibujar el texto
                canvas.FontColor = Colors.DarkBlue;
                canvas.FontSize = fontSize;
                canvas.DrawString(
                    textoMapa,
                    textoX,
                    textoY,
                    textoWidth,
                    textoHeight,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center
                );
            }
        }

        private Color GetColorByCategorias(List<Categoria> categorias)
        {
            var categoria = categorias.FirstOrDefault();

            Debug.WriteLine(categoria != null
                ? $"Primera categoría recibida: Id={categoria.Id}, Nombre={categoria.Nombre}"
                : "No se recibió ninguna categoría.");

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
                return Colors.Gray;

            var nombre = categoria.Nombre.ToLower();

            if (nombre.Contains("baño"))
                return Colors.LightBlue;

            if (nombre.Contains("cafetería"))
                return Colors.Orange;

            if (nombre.Contains("información"))
                return Colors.Green;

            if (nombre.Contains("organización"))
                return Colors.Blue;

            return Colors.Gray;
        }
        private GridCell[,] CrearGridDePasillos(Stand origen, Stand destino)
        {
            if (Stands == null || Stands.Count == 0)
                throw new InvalidOperationException("No hay stands para crear el grid.");

            float maxX = Stands.Max(s => s.PosX + s.Width);
            float maxY = Stands.Max(s => s.PosY + s.Height);

            if (maxX <= 0 || maxY <= 0)
                throw new InvalidOperationException("Dimensiones inválidas de stands (posiciones o tamaños negativos/cero).");

            int gridWidth = Math.Max(1, (int)Math.Ceiling(maxX / CellSize));
            int gridHeight = Math.Max(1, (int)Math.Ceiling(maxY / CellSize));

            var grid = new GridCell[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y] = new GridCell { X = x, Y = y, Walkable = true };
                }
            }

            foreach (var stand in Stands)
            {
                if (stand.Width <= 0 || stand.Height <= 0) continue;

                int startX = Math.Clamp((int)(stand.PosX / CellSize), 0, gridWidth - 1);
                int endX = Math.Clamp((int)(((stand.PosX + stand.Width - 0.01f)) / CellSize), 0, gridWidth - 1);
                int startY = Math.Clamp((int)(stand.PosY / CellSize), 0, gridHeight - 1);
                int endY = Math.Clamp((int)(((stand.PosY + stand.Height - 0.01f)) / CellSize), 0, gridHeight - 1);


                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        grid[x, y].Walkable = false;
                    }
                }
            }
            var origenCell = ObtenerCeldaCentrada(origen, grid);
            var destinoCell = ObtenerCeldaCentrada(destino, grid);

            grid[origenCell.X, origenCell.Y].Walkable = true;
            grid[destinoCell.X, destinoCell.Y].Walkable = true;

            Debug.WriteLine($"Grid creado: {grid.GetLength(0)}x{grid.GetLength(1)}");
            Debug.WriteLine($"Origen Walkable: {grid[origenCell.X, origenCell.Y].Walkable}, Destino Walkable: {grid[destinoCell.X, destinoCell.Y].Walkable}");


            return grid;
        }

        //private List<GridCell> BuscarRuta(string destinoNombre)
        //{
        //    var origen = Stands.FirstOrDefault(s => s.CategoriaIds.Any(c => c.Nombre.ToLower().Contains("información")));
        //    var destino = Stands.FirstOrDefault(s => s.Nombre.ToLower().Contains(destinoNombre.ToLower()));

        //    if (origen == null || destino == null)
        //        return new();

        //    var grid = CrearGridDePasillos(origen, destino);

        //    var origenCell = ObtenerCeldaCentrada(origen, grid);
        //    var destinoCell = ObtenerCeldaCentrada(destino, grid);

        //    // Añadir debug para verificar las coordenadas y el tamaño del grid
        //    Debug.WriteLine($"GridSize: {grid.GetLength(0)} x {grid.GetLength(1)}");
        //    Debug.WriteLine($"Origen: {origen.PosX},{origen.PosY}, Destino: {destino.PosX},{destino.PosY}");
        //    Debug.WriteLine($"OrigenCell: {origenCell.X},{origenCell.Y} - DestinoCell: {destinoCell.X},{destinoCell.Y}");

        //    // Verificar si las coordenadas están fuera del rango
        //    if (origenCell.X < 0 || origenCell.X >= grid.GetLength(0) || origenCell.Y < 0 || origenCell.Y >= grid.GetLength(1) ||
        //        destinoCell.X < 0 || destinoCell.X >= grid.GetLength(0) || destinoCell.Y < 0 || destinoCell.Y >= grid.GetLength(1))
        //    {
        //        Debug.WriteLine("⚠️ Coordenadas fuera de rango, abortando ruta.");
        //        return new(); // Si está fuera de rango, devolvemos una lista vacía
        //    }

        //    var pathfinder = new PathFinder(grid);
        //    return pathfinder.FindPath(origenCell, destinoCell);
        //}

        private void DibujarCamino(ICanvas canvas, List<GridCell> camino)
        {
            if (camino == null || camino.Count < 2)
                return;

            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 3;
            Debug.WriteLine($"🟤 Dibujando ruta con {camino.Count} puntos");
            for (int i = 0; i < camino.Count - 1; i++)
            {
                var a = camino[i];
                var b = camino[i + 1];

                float ax = OffsetX + a.X * CellSize * Scale + (CellSize * Scale / 2);
                float ay = OffsetY + a.Y * CellSize * Scale + (CellSize * Scale / 2);
                float bx = OffsetX + b.X * CellSize * Scale + (CellSize * Scale / 2);
                float by = OffsetY + b.Y * CellSize * Scale + (CellSize * Scale / 2);

                canvas.DrawLine(ax, ay, bx, by);
            }
        }

        public void DefinirRuta(Stand origen, Stand destino)
        {
            var grid = CrearGridDePasillos(origen, destino);
            var origenCell = ObtenerCeldaCentrada(origen, grid);
            var destinoCell = ObtenerCeldaCentrada(destino, grid);

            var pathfinder = new PathFinder(grid);
            Debug.WriteLine($"🔵 DefinirRuta: origenCell=({origenCell.X},{origenCell.Y}), destinoCell=({destinoCell.X},{destinoCell.Y})");

            _camino = pathfinder.FindPath(origenCell, destinoCell);
            Debug.WriteLine($"🟣 Ruta generada con {_camino?.Count ?? 0} celdas");

        }
        private GridCell ObtenerCeldaCentrada(Stand stand, GridCell[,] grid)
        {
            int gridX = (int)((stand.PosX + stand.Width / 2) / CellSize);
            int gridY = (int)((stand.PosY + stand.Height / 2) / CellSize);

            // Evitar reventar el array
            int maxX = grid.GetLength(0) - 1;
            int maxY = grid.GetLength(1) - 1;

            gridX = Math.Clamp(gridX, 0, maxX);
            gridY = Math.Clamp(gridY, 0, maxY);

            if (grid[gridX, gridY].Walkable)
                return grid[gridX, gridY];

            // Buscar celda walkable más cercana en espiral
            int maxDist = Math.Max(grid.GetLength(0), grid.GetLength(1));
            for (int dist = 1; dist < maxDist; dist++)
            {
                for (int dx = -dist; dx <= dist; dx++)
                {
                    for (int dy = -dist; dy <= dist; dy++)
                    {
                        int nx = gridX + dx;
                        int ny = gridY + dy;

                        if (nx >= 0 && ny >= 0 && nx < grid.GetLength(0) && ny < grid.GetLength(1))
                        {
                            if (grid[nx, ny].Walkable)
                                return grid[nx, ny];
                        }
                    }
                }
            }

            // Si nada sirve, devolvemos el centro forzado (aunque sea bloqueado)
            return grid[gridX, gridY];

        }

    }

    public class GridCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Walkable { get; set; }
        public GridCell? Parent { get; set; } 
    }
    public class PathFinder
    {
        private readonly GridCell[,] grid;

        public PathFinder(GridCell[,] grid)
        {
            this.grid = grid;
        }

        public List<GridCell> FindPath(GridCell start, GridCell end)
        {
            var openList = new List<GridCell> { start };
            var closedList = new HashSet<GridCell>();


            while (openList.Count > 0)
            {
                var current = openList.OrderBy(n => GetDistance(n, end)).First();

                if (current.X == end.X && current.Y == end.Y)
                    return ReconstructPath(current);

                openList.Remove(current);
                closedList.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!neighbor.Walkable || closedList.Contains(neighbor))
                        continue;

                    if (!openList.Contains(neighbor))
                    {
                        neighbor.Parent = current;
                        openList.Add(neighbor);
                    }
                }
            }

            return new(); 
        }

        private List<GridCell> ReconstructPath(GridCell end)
        {
            var path = new List<GridCell>();
            var current = end;
            while (current != null)
            {
                path.Insert(0, current);
                current = current.Parent;
            }
            return path;
        }

        private IEnumerable<GridCell> GetNeighbors(GridCell cell)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

            foreach (var (dx, dy) in dirs)
            {
                int nx = cell.X + dx;
                int ny = cell.Y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    yield return grid[nx, ny];
            }
        }

        private int GetDistance(GridCell a, GridCell b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }

}
