using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TP_MVC_CRUD.Data;
using TP_MVC_CRUD.Models;

namespace TP_MVC_CRUD.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Cuenta/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            // valida el user y contraseña si existe en la bd
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contraseña == contraseña);

            // si no encuentra, retorna error
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            // guardamos info mínima en sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("EsAdmin", usuario.EsAdministrador ? "true" : "false");

            // redirige a la página principal
            return RedirectToAction("Index", "Home");
        }

        // GET: /Cuenta/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult SinAcceso()
        {
            return View();
        }
    }
}
