using FerioApp.Models;
using FerioApp.Services;
using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace FerioApp
{
    public partial class AddStandPage : ContentPage
    {
        private StandService _standService;
        private List<Categoria> categoriasDisponibles;


        public AddStandPage(StandService standService)
        {
            InitializeComponent();
            _standService = standService;
            categoriasDisponibles = GenerarCategoriasFijas();
            BindingContext = this;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            
            categoriasCollection.ItemsSource = categoriasDisponibles;
        }
        private List<Categoria> GenerarCategoriasFijas()
        {
            return new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Turismos" },
                new Categoria { Id = 2, Nombre = "Vehículos Comerciales" },
                new Categoria { Id = 3, Nombre = "Coches Eléctricos" },
                new Categoria { Id = 4, Nombre = "Accesorios y Piezas" },
                new Categoria { Id = 5, Nombre = "Servicios Financieros" },
                new Categoria { Id = 6, Nombre = "Seguros" },
                new Categoria { Id = 7, Nombre = "Recambios y Repuestos" },
                new Categoria { Id = 8, Nombre = "Movilidad Sostenible" },
                new Categoria { Id = 9, Nombre = "Vehículos de Alto Rendimiento" },
                new Categoria { Id = 10, Nombre = "Equipamiento Profesional" }
            };
        }
         

        private async void OnAddStandClicked(object sender, EventArgs e)
        {
            var categoriaIdsSeleccionadas = categoriasDisponibles
                .Where(c => c.IsSelected)
                .Select(c => c.Id)
                .ToList();

            if (!categoriaIdsSeleccionadas.Any())
            {
                await DisplayAlert("Error", "Debes seleccionar al menos una categoría.", "OK");
                return;
            }

            var newStand = new StandCreateModel
            {
                Nombre = nombreEntry.Text,
                Descripcion = descripcionEditor.Text,
                Logo = logoEntry.Text,
                Ubicacion = ubicacionEntry.Text,
                HorarioAtencion = horariosEntry.Text,
                EnlaceWeb = enlaceWebEntry.Text,
                Contacto = contactoEntry.Text,
                UsuarioId = int.Parse(usuarioEntry.Text),
                PosX = int.Parse(posXEntry.Text),
                PosY = int.Parse(posYEntry.Text),
                Width = int.Parse(widthEntry.Text),
                Height = int.Parse(heightEntry.Text),

                CategoriaIds = categoriaIdsSeleccionadas
            };

            var success = await _standService.AddStand(newStand);
            if (success)
            {
                await DisplayAlert("Éxito", "Stand agregado correctamente", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el stand", "OK");

            }
        }
        public async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//mainPage");
        }
    }
}
