using Microsoft.EntityFrameworkCore;
using TP_MVC_CRUD.Data;

// crea el builder para configurar servicios y middleware
var builder = WebApplication.CreateBuilder(args);

// configura el contexto de la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// agrega los servicios mvc necesarios para la aplicación
builder.Services.AddControllersWithViews();

// agrega un cache en memoria que se usara para funcionalidades como la sesion
builder.Services.AddDistributedMemoryCache();

// configura la sesion para almacenar datos del usuario
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // no accesible por JavaScript
    options.Cookie.IsEssential = true; // necesario para que la sesión funcione incluso si el usuario no ha consentido cookies
});

// agrega el acceso al contexto HTTP, necesario para acceder a la sesión y otros servicios sin controladores
builder.Services.AddHttpContextAccessor();

// construye la app
var app = builder.Build();

// configura manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // fuerza que todas las peticiones usen HTTPS
app.UseStaticFiles(); // permite servir archivos estáticos como CSS, JS, imágenes desde la carpeta wwwroot

app.UseRouting(); // habilita el sistema de rutas para enviar peticiones a controladores y acciones.

app.UseSession(); //habilita la sesión para guardar datos por usuario

app.UseAuthorization(); // habilita la autorización para controlar el acceso a recursos (permite utilizar nuestro filtro personalizado)

// define la ruta por defecto
app.MapControllerRoute(
    name: "default",
   pattern: "{controller=Cuenta}/{action=Login}/{id?}");

// ejecuta la aplicación
app.Run();
