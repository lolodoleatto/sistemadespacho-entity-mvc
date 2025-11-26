using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TP_MVC_CRUD.Models
{
    public class DespachoDetalle
    {
        public int DespachoDetalleId { get; set; }
        public int DespachoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        public decimal CostoUnitario { get; set; }
        public decimal Subtotal => Cantidad * CostoUnitario;

        [ValidateNever]
        public Despacho Despacho { get; set; }
        [ValidateNever]
        public Producto Producto { get; set; }
    }
}
