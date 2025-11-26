using Microsoft.AspNetCore.Mvc; // para RedirectToActionResult
using Microsoft.AspNetCore.Mvc.Filters; // para ActionFilterAttribute y ActionExecutingContext
using Microsoft.AspNetCore.Http; // para trabajar con la sesion

namespace TP_MVC_CRUD.Filters
{
    public class FiltroAutorizacion : ActionFilterAttribute
    {
        // guarda si el filtro es solo para administradores
        private readonly bool _soloAdmin;

        // constructor inicializado en false
        public FiltroAutorizacion(bool soloAdmin = false)
        {
            _soloAdmin = soloAdmin;
        }

        // metodo que se ejecuta antes del accionar del controlador
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // obtenemos el contexto HTTP y la información del usuario de la sesión
            var httpContext = context.HttpContext;
            var usuarioId = httpContext.Session.GetInt32("UsuarioId");
            var esAdmin = httpContext.Session.GetString("EsAdmin") == "true";

            // verificamos si el usuario está autenticado
            if (usuarioId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Cuenta", null);
                return;
            }

            // si el filtro es solo para administradores, verificamos si el usuario es administrador, si no lo es redirige a SinAcceso
            if (_soloAdmin && !esAdmin)
            {
                context.Result = new RedirectToActionResult("SinAcceso", "Cuenta", null);
                return;
            }

            // deja al controlardor continuar con la acción con el contexto actualizado
            base.OnActionExecuting(context);
        }

      
    }
}
