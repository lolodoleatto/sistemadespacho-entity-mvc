using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TP_MVC_CRUD.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public bool EsAdministrador { get; set; }

        [ValidateNever]
        public ICollection<Despacho> Despachos { get; set; }
    }
}
