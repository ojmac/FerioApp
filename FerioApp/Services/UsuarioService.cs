using FerioApp.Models;  


namespace FerioApp.Services;

public class UsuarioService
{
  
    private static Usuario _usuarioActual;

    public static Usuario UsuarioActual
    {
        get
        {
            if (_usuarioActual == null)
            {
                _usuarioActual = new Usuario();
            }
            return _usuarioActual;
        }
    }

    // actualiza la información del usuario
    public static void ActualizarUsuario(Usuario usuario)
    {
        _usuarioActual = usuario;
    }
    public static void LimpiarUsuario()
    {
        _usuarioActual = null;
        _usuarioActual = new Usuario();
    }   
}
