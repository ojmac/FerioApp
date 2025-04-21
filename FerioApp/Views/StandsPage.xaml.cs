using FerioApp.Models;
using FerioApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace FerioApp

{
    public partial class StandsPage : ContentPage, INotifyPropertyChanged
    {
        private StandService _standService;
        private ObservableCollection<Stand> _stands;
        private AddStandPage _addStandPage; 

        public ObservableCollection<Stand> Stands
        {
            get => _stands;
            set
            {
                _stands = value;
                OnPropertyChanged(nameof(Stands)); 
            }
        }

        public bool IsAdmin { get; set; } = UsuarioService.UsuarioActual.Role == "Organizador";  

        public StandsPage(StandService standService, AddStandPage addStandPage  )
        {
            InitializeComponent();
            _standService = standService;
            IsAdmin = UsuarioService.UsuarioActual?.IsOrganizer ?? false;
            BindingContext = this;

            Shell.SetBackgroundColor(this, Color.FromArgb("#3A86FF"));
            Shell.SetTitleColor(this, Color.FromArgb("#000000"));
            _addStandPage = addStandPage;
        }

        // Carga los stands cuando la página se muestra
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStands();  
        }

        //Carga en pantalla los stands existentes en db
        private async Task LoadStands()
        {
            var stands = await _standService.GetStands();
            Debug.WriteLine($"Se cargaron {stands.Count} stands.");

            foreach (var stand in stands)
            {
                Debug.WriteLine($"Stand: {stand.Nombre}, {stand.Descripcion}, {stand.Ubicacion}, {stand.HorarioAtencion}");
            }


            Stands = new ObservableCollection<Stand>(stands);
        }
        // Navega a la página de agregar un stand
        private async void OnAddStandClicked(object sender, EventArgs e)
        {
            try
            {
                
                await Navigation.PushAsync(_addStandPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔥 ERROR al navegar a AddStandPage:");
                Debug.WriteLine(ex.Message);
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }



        // Borra Stands por su Id
        private async void OnRemoveStandClicked(object sender, EventArgs e)
        {
            if (!int.TryParse(standIdToDeleteEntry.Text, out int standId))
            {
                await DisplayAlert("Error", "Por favor ingresa un ID válido de stand.", "OK");
                return;
            }
            
            bool confirm = await DisplayAlert("¿Estás seguro?",
                $"Se eliminará el stand con ID {standId} de forma permanente.",
                "Eliminar", "Cancelar");

            if (!confirm)
                return;

            
            await _standService.DeleteStand(standId);

            standIdToDeleteEntry.Text = string.Empty;

            await DisplayAlert("Hecho", $"Stand con ID {standId} eliminado.", "OK");
            
            await LoadStands();

        }

    }
}