﻿using FerioApp.Draws;
using FerioApp.Services;
using System.Diagnostics;

namespace FerioApp
{
    public partial class MapPage : ContentPage
    {
        private readonly StandService _standService;
        public MapDrawable MapDrawable { get; set; }

        public MapPage(StandService standService)
        {
            InitializeComponent();
            _standService = standService;
            MapDrawable = new MapDrawable();
            BindingContext = this;

            Shell.SetBackgroundColor(this, Color.FromArgb("#8338EC"));
            Shell.SetTitleColor(this, Colors.White);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStands();

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnMapTapped;
            MapView.GestureRecognizers.Clear();
            MapView.GestureRecognizers.Add(tapGesture);
        }

        private async Task LoadStands()
        {
            try
            {
                var stands = await _standService.GetStands();
                if (stands.Any())
                {
                    MapDrawable.Stands = stands;
                    MapView.Drawable = MapDrawable;
                    MapView.Invalidate(); // Forzar redibujado
                    Debug.WriteLine($"Se cargaron {stands.Count} stands.");
                }
                else
                {
                    Debug.WriteLine("No se encontraron stands.");
                    await DisplayAlert("Error", "No se encontraron stands para mostrar.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar los stands: {ex.Message}");
                await DisplayAlert("Error", "Hubo un problema al cargar los stands.", "OK");
            }
        }

        private async void OnMapTapped(object sender, TappedEventArgs e)
        {
            var point = e.GetPosition(MapView);
            if (point == null) return;

            float px = (float)point.Value.X;
            float py = (float)point.Value.Y;

            float x = (px - MapDrawable.OffsetX) / MapDrawable.Scale;
            float y = (py - MapDrawable.OffsetY) / MapDrawable.Scale;

            var stand = MapDrawable.Stands.FirstOrDefault(s =>
                x >= s.PosX && x < s.PosX + s.Width &&
                y >= s.PosY && y < s.PosY + s.Height);

            if (stand != null)
            {
                await DisplayAlert("Stand seleccionado", $"{stand.Nombre}\n{stand.Descripcion}", "Cerrar");
            }
        }

        private void OnVerRutaClicked(object sender, EventArgs e)
        {
            var nombreDestino = StandSearchEntry.Text?.Trim().ToLower();
            Debug.WriteLine("🟢 Se hizo clic en el botón Ver Ruta");

            if (string.IsNullOrWhiteSpace(nombreDestino))
            {
                DisplayAlert("Error", "Introduce un nombre de stand válido.", "OK");
                return;
            }

            var origen = MapDrawable.Stands.FirstOrDefault(s =>
                s.CategoriaIds.Any(c => c.Nombre != null && c.Nombre.ToLower().Contains("información")));

            var destino = MapDrawable.Stands.FirstOrDefault(s =>
                s.Nombre != null && s.Nombre.ToLower().Contains(nombreDestino.ToLower()));

            if (origen == null)
            {
                DisplayAlert("Error", "No se encontró el stand de información.", "OK");
                return;
            }

            if (destino == null)
            {
                DisplayAlert("No encontrado", $"No se encontró el stand que contiene: \"{nombreDestino}\"", "OK");
                return;
            }

            Debug.WriteLine($"🟡 Origen: {origen.Nombre}, Destino: {destino.Nombre}");
            MapDrawable.DefinirRuta(origen, destino);
            MapView.Invalidate();
            SearchPanel.IsVisible = false;
            StandSearchEntry.Text = string.Empty;
        }

        private void OnOpenSearchClicked(object sender, EventArgs e)
        {
            SearchPanel.IsVisible = !SearchPanel.IsVisible;
        }

        private void OnClearRouteClicked(object sender, EventArgs e)
        {
            MapDrawable._camino = null; // Limpiar la ruta
            MapView.Invalidate(); // Redibujar el mapa
        }

   

        public async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//mainPage");
        }
    }
}