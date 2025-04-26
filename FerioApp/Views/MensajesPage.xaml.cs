using FerioApp.Models;
using FerioApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace FerioApp
{
    public partial class MensajesPage : ContentPage
    {
        private readonly MensajeService _mensajeService;
       

        public MensajesPage(MensajeService mensajeService)
        {
            InitializeComponent();
            _mensajeService = mensajeService;  
            CargarMensajes();  
            PuedeEnviarMensajes.IsVisible = UsuarioService.UsuarioActual.IsExpositorOrOrganizer || UsuarioService.UsuarioActual.IsOrganizer;

            Shell.SetBackgroundColor(this, Color.FromArgb("#06D6A0"));
            Shell.SetTitleColor(this, Colors.White);

        }

        // Cargar mensajes desde la API
        private async void CargarMensajes()
        {
            try
            {
                var tipoUsuario = UsuarioService.UsuarioActual.TipoUsuario; 
                var mensajes = await _mensajeService.ObtenerMensajesAsync();

                if (mensajes.Count == 0)
                {
                    await DisplayAlert("Información", "No se encontraron mensajes.", "OK");
                }
                else
                {
                    MensajesListView.ItemsSource = mensajes;
                }

                // Desactivar envío si el usuario es visitante
               // EnviarMensajeButton.IsEnabled = tipoUsuario != TipoUsuario.Visitante;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los mensajes: {ex.Message}", "OK");
            }
        }


        // Enviar un nuevo mensaje
        private async void EnviarMensaje(object sender, EventArgs e)
        {
            var usuario = UsuarioService.UsuarioActual;

            if (usuario.TipoUsuario == TipoUsuario.Visitante)
            {
                await DisplayAlert("Acceso Denegado", "Los visitantes no pueden enviar mensajes.", "OK");
                return;
            }

            
            var tipoDestino = usuario.TipoUsuario == TipoUsuario.Organizador
                ? await DisplayActionSheet("¿A quién va dirigido?", "Cancelar", null, "Visitante", "Expositor", "Todos")
                : "Todos";

            
            if (tipoDestino == "Cancelar" || tipoDestino == null)
                return;

            
            TipoUsuario tipoUsuarioDestino = tipoDestino switch
            {
                "Visitante" => TipoUsuario.Visitante,
                "Expositor" => TipoUsuario.Expositor,
                _ => TipoUsuario.Organizador 
            };

            var mensaje = new Mensaje
            {
                Titulo = TituloEntry.Text,
                Contenido = ContenidoEditor.Text,
                FechaEnvio = DateTime.Now,
                Leido = false,
                UsuarioId = usuario.Id,
                StandId = usuario.StandId,
                TipoUsuario = tipoUsuarioDestino
            };

            var exito = await _mensajeService.EnviarMensajeAsync(mensaje);

            if (exito)
            {
                await DisplayAlert("Éxito", "Mensaje enviado correctamente", "OK");
                TituloEntry.Text = string.Empty;
                ContenidoEditor.Text = string.Empty;
                CargarMensajes();
            }
            else
            {
                await DisplayAlert("Error", "Hubo un problema al enviar el mensaje", "OK");
            }
        }


        // Mostrar detalles del mensaje al hacer tap en un mensaje
        private async void OnMensajeTapped(object sender, ItemTappedEventArgs e)
        {
            var mensaje = e.Item as Mensaje;
            if (mensaje != null)
            {
                await DisplayAlert(mensaje.Empresa, $"{mensaje.Titulo}\n\n{mensaje.Contenido}","OK" );
            }
        }
    }
}
