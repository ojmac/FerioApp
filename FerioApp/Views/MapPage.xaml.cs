using Android.Gms.Maps;

namespace FerioApp
{
    public partial class MapPage : ContentPage
    {
        public partial class MapPage : ContentPage
        {
            public MapDrawable MapDrawable { get; set; }

            public MapPage()
            {
                InitializeComponent();

                MapDrawable = new MapDrawable();

                // Supongamos que ya tienes los Stands cargados del backend
                MapDrawable.Stands = ListaDeStandsCargados;
                MapView.Drawable = MapDrawable;
            }
        }

    }
}