using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TP_MVC_CRUD.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string Email { get; set; }
        
        [ValidateNever]
        public ICollection<Direccion> Direcciones { get; set; }
        [ValidateNever]
        public ICollection<Despacho> Despachos { get; set; }
    }
}
