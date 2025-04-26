using FerioApp.Services;
using FerioApp;
using Microsoft.Extensions.DependencyInjection;


namespace FerioApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>() // Registra la clase App correctamente
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Black.ttf", "PoppinsBlack");
                    fonts.AddFont("Poppins-semiBold.ttf", "PoppinsSemiBold");
                });

            
            builder.Services.AddHttpClient("FerioBackend", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7117");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Registrar ApiService
            builder.Services.AddHttpClient<ApiService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7117");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            builder.Services.AddHttpClient<StandService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7117");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            builder.Services.AddHttpClient<MensajeService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7117");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            builder.Services.AddTransient<UsuarioService>();

            builder.Services.AddTransient<UsuarioPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<PerfilPage>();
            builder.Services.AddTransient<StandsPage>();
            builder.Services.AddTransient<AddStandPage>();
            builder.Services.AddTransient<ModifyStandPage>();
            builder.Services.AddTransient<MensajesPage>();




            return builder.Build();
        }
    }
}
