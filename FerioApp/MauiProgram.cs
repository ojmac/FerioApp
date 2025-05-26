using FerioApp.Services;
using FerioApp;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Maui.Core.Hosting;


namespace FerioApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var backendBaseUri = new Uri("https://localhost:7117");
            var backendTimeout = TimeSpan.FromSeconds(30);

            builder
                .UseMauiApp<App>() // Registra la clase App correctamente
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Black.ttf", "PoppinsBlack");
                    fonts.AddFont("Poppins-semiBold.ttf", "PoppinsSemiBold");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                })
                .ConfigureSyncfusionCore(); 

            builder.Services.AddHttpClient("FerioBackend", client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });

            // Registrar ApiService
            builder.Services.AddHttpClient<ApiService>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });
            builder.Services.AddHttpClient<StandService>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });
            
            builder.Services.AddHttpClient<MensajeService>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });
            builder.Services.AddHttpClient<CategoriaService>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });
            builder.Services.AddHttpClient<PerfilPage>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });
            builder.Services.AddHttpClient<ControlPage>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });
            builder.Services.AddHttpClient<ChatPage>(client =>
            {
                client.BaseAddress = backendBaseUri;
                client.Timeout = backendTimeout;
            });

            builder.Services.AddTransient<UsuarioService>();
           
            builder.Services.AddTransient<UsuarioPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<RegisterPage>();
           
            builder.Services.AddTransient<StandsPage>();
            builder.Services.AddTransient<AddStandPage>();
            builder.Services.AddTransient<ModifyStandPage>();
            builder.Services.AddTransient<MensajesPage>();
            builder.Services.AddTransient<MapPage>();

            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                fonts.AddFont("Poppins-Italic.ttf", "PoppinsItalic");
                fonts.AddFont("Poppins-BoldItalic.ttf", "PoppinsBoldItalic");
            });
            
            return builder.Build();
        }
    }
}
