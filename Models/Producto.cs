using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TP_MVC_CRUD.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Descripcion { get; set; }
        public decimal CostoUnitario { get; set; }

        public int Stock { get; set; }

        [ValidateNever]
        public ICollection<DespachoDetalle> Detalles { get; set; }
    }
}
