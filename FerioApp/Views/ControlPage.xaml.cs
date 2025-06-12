using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FerioApp.Services;
using FerioApp.Models;
using System.Diagnostics;
using System.Data;
using System.Net.Http.Headers;
using System;
using System.Linq;

namespace FerioApp
{
    public partial class ControlPage : ContentPage
    {
        private Usuario _userProfile = UsuarioService.UsuarioActual;
        private readonly HttpClient _httpclient;


        public List<Usuario> Usuarios { get; set; }
        public List<Stand> Stands { get; set; }
        public List<Mensaje> Mensajes { get; set; }

        public ControlPage(HttpClient httpClient)
        {
            InitializeComponent();
            _httpclient = httpClient;

            var token = UsuarioService.UsuarioActual?.Token;
            if (!string.IsNullOrEmpty(token))
            {
                _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
            }

            _ = CargarUsuarios();
            _ = CargarStands();
            _ = CargarMensajes();
        }

        //private async Task CargarUsuarios()
        //{
        //    var response = await _httpclient.GetAsync("/api/usuarios");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var json = await response.Content.ReadAsStringAsync();
        //        Usuarios = JsonSerializer.Deserialize<List<Usuario>>(json);
        //        usuariosCollection.ItemsSource = Usuarios;
        //    }
        //}
        private async Task CargarUsuarios()
        {
            var token = UsuarioService.UsuarioActual?.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                Debug.WriteLine("[Usuarios] No hay token disponible. El usuario no está autenticado.");
                return;
            }
            _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpclient.GetAsync("/api/usuarios");
            Debug.WriteLine($"[Usuarios] Código: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[Usuarios] JSON: {json}");

                try
                {
                    Usuarios = JsonSerializer.Deserialize<List<Usuario>>(json);
                    Debug.WriteLine($"[Usuarios] Total cargados: {Usuarios?.Count}");
                    usuariosCollection.ItemsSource = Usuarios;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Usuarios] Error de deserialización: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("[Usuarios] Error al obtener datos del servidor.");
            }
        }


        private async Task CargarStands()
        {
            var response = await _httpclient.GetAsync("/api/stands");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Stands = JsonSerializer.Deserialize<List<Stand>>(json);
                standsCollection.ItemsSource = Stands;
            }
        }

        private async Task CargarMensajes()
        {
            var response = await _httpclient.GetAsync("/api/mensajes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Mensajes = JsonSerializer.Deserialize<List<Mensaje>>(json, opciones);
                mensajesCollection.ItemsSource = Mensajes;
            }
        }


        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var textoBusqueda = e.NewTextValue?.ToLower();
            if (Usuarios != null && !string.IsNullOrWhiteSpace(textoBusqueda))
            {
                var filtrado = Usuarios.Where(u => u.Nombre.ToLower().Contains(textoBusqueda)).ToList();
                usuariosCollection.ItemsSource = filtrado;
            }
            else
            {
                usuariosCollection.ItemsSource = Usuarios;
            }
        }
        private void OnStandSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var texto = e.NewTextValue?.ToLower();
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var filtrados = Stands
                    .Where(s => s.Nombre?.ToLower().Contains(texto) == true)
                    .ToList();
                standsCollection.ItemsSource = filtrados;
            }
            else
            {
                standsCollection.ItemsSource = Stands;
            }
        }

        private void OnMensajeSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var texto = e.NewTextValue?.ToLower();
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var filtrados = Mensajes
                    .Where(m => m.Empresa?.ToLower().Contains(texto) == true)
                    .ToList();
                mensajesCollection.ItemsSource = filtrados;
            }
            else
            {
                mensajesCollection.ItemsSource = Mensajes;
            }
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            int index = tabsPicker.SelectedIndex;

            usuariosView.IsVisible = index == 0;
            standsView.IsVisible = index == 1;
            mensajesView.IsVisible = index == 2;
        }


        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//mainPage");
        }
    }


}
