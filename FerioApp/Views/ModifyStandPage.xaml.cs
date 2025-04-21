using FerioApp.Models;
using FerioApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FerioApp
{
    public partial class ModifyStandPage : ContentPage, INotifyPropertyChanged
    {
        private readonly StandService _standService;
        private List<Categoria> categoriasDisponibles;
        private int standId;

        public ModifyStandPage(int idStand, StandService standService)
        {
            InitializeComponent();
            standId = idStand;
            _standService = standService;
            categoriasDisponibles = GenerarCategoriasFijas();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarDatosStand(standId);
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

        private async Task CargarDatosStand(int id)
        {
            StandUpdateModel stand = await _standService.GetStandById(id);
            if (stand == null)
            {
                await DisplayAlert("Error", "No se encontró el stand", "OK");
                await Navigation.PopAsync();
                return;
            }

           
            nombreEntry.Text = stand.Nombre;
            descripcionEditor.Text = stand.Descripcion;
            ubicacionEntry.Text = stand.Ubicacion;
            horariosEntry.Text = stand.HorarioAtencion;
            enlaceWebEntry.Text = stand.EnlaceWeb;
            contactoEntry.Text = stand.Contacto;
            logoEntry.Text = stand.Logo;
            usuarioEntry.Text = stand.UsuarioId.ToString();
            usuarioEntry.IsEnabled = false;

            if (stand?.CategoriaIds != null && categoriasDisponibles != null)
            {
                var categoriaIdsDelStand = stand.CategoriaIds;
                    

                foreach (var categoria in categoriasDisponibles)
                {
                    if (categoriaIdsDelStand.Contains(categoria.Id))
                    {
                        categoria.IsSelected = true;
                    }
                }

                categoriasCollection.ItemsSource = categoriasDisponibles;
            }
            else
            {
                Debug.WriteLine("❗StandCategorias o categoriasDisponibles vienen como null");
            }

        }

        private async void OnModifyStandClicked(object sender, EventArgs e)
        {
            var categoriasSeleccionadas = categoriasDisponibles
                .Where(c => c.IsSelected)
                .Select(c => c.Id)
                .ToList();

            if (!categoriasSeleccionadas.Any())
            {
                await DisplayAlert("Error", "Debes seleccionar al menos una categoría.", "OK");
                return;
            }

            var standActualizado = new StandUpdateModel
            {
                Id = standId,
                Nombre = nombreEntry.Text,
                Descripcion = descripcionEditor.Text,
                Logo = logoEntry.Text,
                Ubicacion = ubicacionEntry.Text,
                HorarioAtencion = horariosEntry.Text,
                EnlaceWeb = enlaceWebEntry.Text,
                Contacto = contactoEntry.Text,
                UsuarioId = int.Parse(usuarioEntry.Text),
                CategoriaIds = categoriasSeleccionadas
            };

            var success = await _standService.UpdateStand(standId, standActualizado);

            if (success)
            {
                await DisplayAlert("Éxito", "Stand actualizado correctamente", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar el stand", "OK");
            }
        }
    }
}
