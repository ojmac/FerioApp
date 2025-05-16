using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using FerioApp.Models;
using FerioApp.Services;

namespace FerioApp
{
    public partial class PerfilPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private Usuario _userProfile = UsuarioService.UsuarioActual;

        private readonly StandService _standService;


        private readonly HttpClient _httpclient;

        private string _token;
        private string _role;

        public bool IsExpositorOrOrganizer { get; set; }
        public bool IsOrganizer { get; set; }

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set
            {
                if (_telefono != value)
                {
                    _telefono = value;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
        }

        private string _direccion;
        public string Direccion
        {
            get => _direccion;
            set
            {
                if (_direccion != value)
                {
                    _direccion = value;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }

        private string _empresa;
        public string Empresa
        {
            get => _empresa;
            set
            {
                if (_empresa != value)
                {
                    _empresa = value;
                    OnPropertyChanged(nameof(Empresa));
                }
            }
        }

        private int _standId;
        public int StandId
        {
            get => _standId;
            set
            {
                if (_standId != value)
                {
                    _standId = value;
                    OnPropertyChanged(nameof(StandId));
                }
            }
        }

        private string _fotoPerfil;
        public string FotoPerfil
        {
            get => _fotoPerfil;
            set
            {
                if (_fotoPerfil != value)
                {
                    _fotoPerfil = value;
                    OnPropertyChanged(nameof(FotoPerfil));
                }
            }
        }
        
        public PerfilPage(HttpClient httpClient, StandService standService)
        {
            InitializeComponent();

            _httpclient = httpClient;
            _standService = standService;
            BindingContext = this; 

            ConfigureProfile();
            Task.Run(async () => await LoadUserProfile());



            Shell.SetBackgroundColor(this, Color.FromArgb("#FFBE0B"));
            Shell.SetTitleColor(this, Colors.White);
        }

        // Configura la vista según el rol del usuario
        private void ConfigureProfile()
        {
            IsExpositorOrOrganizer = _userProfile.TipoUsuario != TipoUsuario.Visitante;
            IsOrganizer = _userProfile.TipoUsuario == TipoUsuario.Organizador;
            _token = _userProfile.Token;
            _role = _userProfile.Role;
        }
        public ICommand EditarStandCommand => new Command(async () =>
        {
            if (StandId == null)
                return;

            await Shell.Current.GoToAsync(nameof(AddStandPage), new Dictionary<string, object>
    {
        { "StandParaEditar", StandId }
    });
        });
        private async void OnModifyStandClicked(object sender, EventArgs e)
        {
            var standIdNullable = UsuarioService.UsuarioActual.StandId;

            if (!standIdNullable.HasValue)
            {
                await DisplayAlert("Error", "No tienes un stand asignado.", "OK");
                return;
            }

            await Navigation.PushAsync(new ModifyStandPage(standIdNullable.Value, _standService));
        }

        // Carga el perfil del usuario desde el almacenamiento seguro
        private async Task LoadUserProfile()
        {
            try
            {
                var userProfileJson = await SecureStorage.GetAsync("user_data");

                if (string.IsNullOrEmpty(userProfileJson))
                {
                    await DisplayAlert("Error", "No se encontraron datos de usuario. Inicia sesión nuevamente.", "OK");
                    return;
                }

                _userProfile = JsonSerializer.Deserialize<Usuario>(userProfileJson);

                if (_userProfile == null)
                {
                    await DisplayAlert("Error", "Los datos del usuario son inválidos.", "OK");
                    return;
                }

                
                Nombre = _userProfile.Nombre;
                Email = _userProfile.Email;

                Telefono = !string.IsNullOrEmpty(_userProfile.Telefono) && _userProfile.Telefono != "Ej. +34 123 456 789"
                    ? _userProfile.Telefono
                    : string.Empty;

                Direccion = !string.IsNullOrEmpty(_userProfile.Direccion) && _userProfile.Direccion != "Introduce tu dirección"
                    ? _userProfile.Direccion
                    : string.Empty;

                Empresa = !string.IsNullOrEmpty(_userProfile.Empresa) && _userProfile.Empresa != "Nombre de la Empresa"
                    ? _userProfile.Empresa
                    : string.Empty;

                StandId = IsExpositorOrOrganizer && _userProfile.StandId.HasValue && _userProfile.StandId > 0
                    ? _userProfile.StandId.Value
                    : 0;

                FotoPerfil = !string.IsNullOrEmpty(_userProfile.FotoPerfil)
                    ? _userProfile.FotoPerfil
                    : "default_photo.png";

                
                IsExpositorOrOrganizer = _userProfile.TipoUsuario != TipoUsuario.Visitante;

                OnPropertyChanged(nameof(IsExpositorOrOrganizer));
                IsOrganizer = _userProfile.TipoUsuario == TipoUsuario.Organizador;
                OnPropertyChanged(nameof(IsOrganizer));

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cargar el perfil: {ex.Message}", "OK");
            }
        }
 

        // Guarda los cambios realizados por el usuario
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_userProfile.Id == 0)
            {
                await DisplayAlert("Error", "No se pudo obtener el ID del usuario. Intenta iniciar sesión nuevamente.", "OK");
                return;
            }
            var nombre = _userProfile.Nombre;
            var email = _userProfile.Email;

            // Validación de campos para evitar enviar valores predeterminados (placeholders)
            var telefono = !string.IsNullOrEmpty(Telefono) && Telefono != "Ej. +34 123 456 789"
                ? Telefono
                : null;

            var direccion = !string.IsNullOrEmpty(Direccion) && Direccion != "Introduce tu dirección"
                ? Direccion
                : null;

            var empresa = !string.IsNullOrEmpty(Empresa) && Empresa != "Nombre de la Empresa"
                ? Empresa
                : null;



            var standId = StandId > 0
                ? StandId
                : (int?)null;


            _userProfile.Telefono = telefono;
            _userProfile.Direccion = direccion;
            _userProfile.Empresa = empresa;
            _userProfile.TipoUsuario = (TipoUsuario)GetUserTypeFromRole();
            _userProfile.FotoPerfil = this._fotoPerfil;
            _userProfile.StandId = standId;
            UsuarioService.ActualizarUsuario(_userProfile);

            try
            {
                var Token = _token;
                await DisplayAlert("Éxito", "aqui llego", "OK");

                if (string.IsNullOrEmpty(Token))
                {
                    await DisplayAlert("Error", "No se pudo autenticar el usuario. Inicia sesión de nuevo.", "OK");
                    return;
                }

                _httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token.Trim());

                var json = JsonSerializer.Serialize(_userProfile);
                Debug.WriteLine(json);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"/api/usuarios/{_userProfile.Id}";
                var response = await _httpclient.PutAsync(url, content);
                await DisplayAlert("Error", "aqui tb", "OK");
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "aqui estoy", "OK");
                    var userProfileJson = JsonSerializer.Serialize(_userProfile);
                    await SecureStorage.SetAsync("user_data", userProfileJson);
                    await DisplayAlert("Éxito", "Tus datos han sido actualizados correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {

                    await DisplayAlert("Error", " po tb aqui estoy", "OK");
                    var error = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error en la API", $"Código: {(int)response.StatusCode}\nMensaje: {error}", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema al guardar los cambios: {ex.Message}", "OK");
                await Navigation.PopAsync();
            }

        }

        // obtiene el tipo de usuario basado en el rol
        private int GetUserTypeFromRole()
        {
            if (IsOrganizer) return (int)TipoUsuario.Organizador;
            if (IsExpositorOrOrganizer) return (int)TipoUsuario.Expositor;

            return (int)TipoUsuario.Visitante;
        }
        private async void OnAdminPanelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//controlPage");

        }
       

        // notifica cambios a la UI
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Carga una foto desde el móvil
        private async void OnProfilePhotoTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Foto", "Abrir selector de fotos del dispositivo.\n(under construction)", "OK");
            return;
        }
        public async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//mainPage");
        }
    }
}
