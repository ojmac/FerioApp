using System.Net.Http.Json;
using FerioApp.Services;
using FerioApp.Models;
using System.Net.Http.Headers;

namespace FerioApp;

public partial class ChatPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public ChatPage(HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient;

        var token = UsuarioService.UsuarioActual?.Token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
        }
        Shell.SetBackgroundColor(this, Color.FromArgb("#FF6F61"));
        Shell.SetTitleColor(this, Colors.White);
    }

    private async void Enviar_Clicked(object sender, EventArgs e)
    {
        var pregunta = PreguntaInput.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pregunta)) return;

        MostrarMensaje(pregunta, true); // 👉 Mostrar pregunta del usuario
        PreguntaInput.Text = "";         // 👉 Limpiar input

        try
        {
            var request = new { Message = pregunta };
            var response = await _httpClient.PostAsJsonAsync("/api/chatbot/ask", request);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<ChatResponse>();
                MostrarMensaje(resultado?.Response ?? "Sin respuesta.", false);
            }
            else
            {
                MostrarMensaje("Error al conectar con el asistente.", false);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error inesperado: " + ex.Message, false);
        }
    }

    private void MostrarMensaje(string texto, bool esUsuario)
    {
        var mensajeLabel = new Label
        {
            Text = texto,
            TextColor = Colors.Black,
            BackgroundColor = esUsuario ? Color.FromArgb("#DCF8C6") : Color.FromArgb("#ECECEC"),
            Padding = 10,
            Margin = new Thickness(esUsuario ? 50 : 0, 2, esUsuario ? 0 : 50, 2),
            HorizontalOptions = esUsuario ? LayoutOptions.End : LayoutOptions.Start,
            MaximumWidthRequest = 300,
            FontSize = 14,
            LineBreakMode = LineBreakMode.WordWrap
            
        };

        ChatContainer.Children.Add(mensajeLabel);

        // 👉 Scroll al último mensaje (opcional)
        _ = ChatContainer.Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(100);
            await (this.Content as ScrollView)?.ScrollToAsync(mensajeLabel, ScrollToPosition.End, true);
        });
    }

    private void LimpiarConversacion_Clicked(object sender, EventArgs e)
    {
        ChatContainer.Children.Clear();
    }

    public class ChatResponse
    {
        public string Response { get; set; }
    }

    public async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//mainPage");
    }
}
