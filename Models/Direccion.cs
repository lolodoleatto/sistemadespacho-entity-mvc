using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TP_MVC_CRUD.Models
{
    public class Direccion
    {
        public int DireccionId { get; set; }
        public string Calle { get; set; }
        public string Localidad { get; set; }

        public int ClienteId { get; set; }

        [ValidateNever]
        public Cliente Cliente { get; set; }
    }
}
