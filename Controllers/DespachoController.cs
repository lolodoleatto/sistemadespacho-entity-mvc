using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using TP_MVC_CRUD.Data;
using TP_MVC_CRUD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using TP_MVC_CRUD.Filters;

namespace TP_MVC_CRUD.Controllers
{
    [FiltroAutorizacion(soloAdmin: true)]
    public class DespachoController : Controller
    {
        // clase interna para representar un item del carrito de despacho
        public class ItemCarrito
        {
            public int ProductoId { get; set; }
            public string Descripcion { get; set; }
            public int Cantidad { get; set; }
            public decimal CostoUnitario { get; set; }
            public decimal Subtotal => Cantidad * CostoUnitario;
        }

        // contexto de la base de datos
        private readonly AppDbContext _context;

        public DespachoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int? clienteId)
        {
            // carga los clientes y productos al viewbag para usar en los combos.
            ViewBag.Clientes = new SelectList(_context.Clientes
                .Select(c => new { c.ClienteId, NombreCompleto = c.Nombre + " " + c.Apellido }),
                "ClienteId", "NombreCompleto");

            ViewBag.Productos = new SelectList(_context.Productos, "ProductoId", "Descripcion");

            // recuperamos el carrito de despacho de la sesión
            var carritoJson = HttpContext.Session.GetString("CarritoDespacho");
            List<ItemCarrito> carrito;
            // si no hay carrito, inicializamos una lista vacía
            if (string.IsNullOrEmpty(carritoJson))
                carrito = new List<ItemCarrito>();
            else
                // si hay carrito, lo deserializamos
                carrito = JsonSerializer.Deserialize<List<ItemCarrito>>(carritoJson);

            // calculamos el total de cantidad y monto del carrito
            ViewBag.TotalCantidad = carrito.Sum(i => i.Cantidad);
            ViewBag.TotalMonto = carrito.Sum(i => i.Subtotal);

            // si hay un clienteId, lo guarda en tempdata para mantener el valor en el select
            if (clienteId.HasValue)
                TempData["ClienteIdSeleccionado"] = clienteId.Value;

            return View(carrito); // devolvemos la vista con el modelo = lista de ItemCarrito.
        }

        [HttpPost]
        public IActionResult AgregarProducto(int clienteId, int productoId, int cantidad)
        {
            // si la cantidad es menor o igual a 0, redirige al formulario de creación
            if (cantidad <= 0)
                return RedirectToAction(nameof(Create), new { clienteId });

            // busca el producto en la BD
            var producto = _context.Productos.FirstOrDefault(p => p.ProductoId == productoId);

            // si no existe el producto, redirige al formulario de creación
            if (producto == null)
                return RedirectToAction(nameof(Create), new { clienteId });

            // recupera el carrito de la sesion. Si no existe, inicializa una lista vacía.
            var carritoJson = HttpContext.Session.GetString("CarritoDespacho");
            List<ItemCarrito> carrito;
            if (string.IsNullOrEmpty(carritoJson))
                carrito = new List<ItemCarrito>();
            else
                carrito = JsonSerializer.Deserialize<List<ItemCarrito>>(carritoJson);

            // busca si el producto ya está en el carrito. Si ya esta lo actualiza la cantidad, si no lo agrega como nuevo item.
            var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);
            if (item == null)
            {
                carrito.Add(new ItemCarrito
                {
                    ProductoId = producto.ProductoId,
                    Descripcion = producto.Descripcion,
                    Cantidad = cantidad,
                    CostoUnitario = producto.CostoUnitario
                });
            }
            else
            {
                item.Cantidad += cantidad;
            }

            // serializa el carrito y lo guarda en la sesión
            var serializado = JsonSerializer.Serialize(carrito);
            HttpContext.Session.SetString("CarritoDespacho", serializado);

            //redirige y recarga el formulario
            return RedirectToAction(nameof(Create), new { clienteId });
        }

        [HttpPost]
        public IActionResult RemoverProducto(int productoId)
        {
            // recupera el carrito de la sesión. Si no existe, redirige al formulario de creación.
            var carritoJson = HttpContext.Session.GetString("CarritoDespacho");
            if (string.IsNullOrEmpty(carritoJson))
                return RedirectToAction(nameof(Create));

            // deserializa el carrito
            var carrito = JsonSerializer.Deserialize<List<ItemCarrito>>(carritoJson);

            // si encuentra el producto en el carrito, lo elimina
            var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);
            if (item != null)
                carrito.Remove(item);

            // serializa el carrito actualizado y lo guarda en la sesión
            HttpContext.Session.SetString("CarritoDespacho", JsonSerializer.Serialize(carrito));

            // redirige al formulario de creación
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar(int clienteId)
        {
            // verifica si el cliente existe
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.ClienteId == clienteId);
            if (!clienteExiste)
            {
                TempData["Error"] = "El cliente seleccionado no existe.";
                return RedirectToAction(nameof(Create));
            }

            // recupera el carrito de despacho de la sesión y lo deserializa
            var carritoJson = HttpContext.Session.GetString("CarritoDespacho");
            if (string.IsNullOrEmpty(carritoJson))
            {
                TempData["Error"] = "No hay productos para confirmar.";
                return RedirectToAction(nameof(Create));
            }
            var carrito = JsonSerializer.Deserialize<List<ItemCarrito>>(carritoJson);

            // verifica si el carrito está vacío
            if (!carrito.Any())
            {
                TempData["Error"] = "El carrito está vacío.";
                return RedirectToAction(nameof(Create));
            }

            // verifica si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            // recorre el carrito y verifica el stock de cada producto
            foreach (var item in carrito)
            {
                var producto = _context.Productos.FirstOrDefault(p => p.ProductoId == item.ProductoId);
                if (producto == null || producto.Stock < item.Cantidad)
                {
                    TempData["Error"] = $"Stock insuficiente para el producto: {item.Descripcion}. Stock disponible: {producto?.Stock ?? 0}";
                    return RedirectToAction(nameof(Create));
                }
            }

            // guarda el despacho. Crea el encabezado del despacho
            var despacho = new Despacho
            {
                ClienteId = clienteId,
                UsuarioId = usuarioId.Value,
                Fecha = DateTime.Now,
                Confirmado = true
            };
            _context.Despachos.Add(despacho);
            await _context.SaveChangesAsync();

            // guarda los detalles del despacho y descuenta el stock de cada producto
            foreach (var item in carrito)
            {
                var producto = _context.Productos.First(p => p.ProductoId == item.ProductoId);

                var detalle = new DespachoDetalle
                {
                    DespachoId = despacho.DespachoId,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    CostoUnitario = item.CostoUnitario
                };

                _context.DespachoDetalles.Add(detalle);

                // descontar stock
                producto.Stock -= item.Cantidad;
            }

            // guarda los cambios en la base de datos y limpia el carrito de la sesión
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("CarritoDespacho");

            // muestra un mensaje de éxito y redirige al listado de despachos
            TempData["Success"] = "Despacho confirmado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Despacho
        public async Task<IActionResult> Index(int? usuarioId)
        {
            // carga los usuarios al viewbag para usar en el select de filtro
            var usuarios = await _context.Usuarios.ToListAsync();
            ViewBag.Usuarios = new SelectList(usuarios, "UsuarioId", "NombreUsuario", usuarioId);

            // consulta los despachos, incluyendo cliente y usuario, y aplica el filtro por usuario si corresponde
            var despachos = _context.Despachos
                .Include(d => d.Cliente)
                .Include(d => d.Usuario)
                .AsQueryable();

            // si se especifica un usuarioId, filtra los despachos por ese usuario
            if (usuarioId.HasValue && usuarioId.Value != 0)
            {
                despachos = despachos.Where(d => d.UsuarioId == usuarioId.Value);
            }

            // ordena los despachos por fecha descendente y convierte a lista asíncrona
            var lista = await despachos.OrderByDescending(d => d.Fecha).ToListAsync();

            return View(lista);
        }


        // GET: Despacho/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // verifica si el id es nulo
            if (id == null) return NotFound();

            // busca el despacho por id, incluyendo cliente, usuario y detalles con productos
            var despacho = await _context.Despachos
                .Include(d => d.Cliente)
                .Include(d => d.Usuario)
                .Include(d => d.Detalles)
                    .ThenInclude(det => det.Producto)
                .FirstOrDefaultAsync(m => m.DespachoId == id);

            // verifica si se encontró el despacho
            if (despacho == null) return NotFound();

            // retorna la vista con el despacho encontrado
            return View(despacho);
        }
    }
}