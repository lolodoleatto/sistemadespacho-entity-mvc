# 📦 Sistema de Gestión de Despachos  
### ASP.NET Core MVC + Entity Framework Core (Code First)

Proyecto desarrollado para la materia **Programación III** de la **Tecnicatura Universitaria en Programación (TUP – UTN)**.  
El sistema permite gestionar **clientes, direcciones, productos y despachos de mercadería**, implementado con **ASP.NET Core MVC** y **Entity Framework Core** siguiendo el enfoque **Code First**.

---

## 🚀 Funcionalidades principales

- **ABM completo** de clientes, direcciones y productos  
- **Gestión de usuarios sin Identity**  
  - Registro manual  
  - Login con validación vía DbContext  
- **Creación, edición y confirmación de despachos**  
- **Validaciones en servidor y cliente**  
- Base de datos generada mediante **migraciones EF Core**  
- Arquitectura MVC con controllers, models y vistas Razor  
- Manejo de relaciones entre entidades (1:N, N:N según el caso)  
- Interfaz limpia con Bootstrap

---

## 🛠️ Tecnologías utilizadas

- **ASP.NET Core MVC 8**
- **C# 12**
- **Entity Framework Core (Code First)**
- **SQL Server**
- **LINQ**
- **Razor Pages**
- **Bootstrap 5**

---

## 📂 Estructura del proyecto

/Controllers
/Models
/Views
/Migrations
/wwwroot
appsettings.json
Program.cs

---

## 🗄️ Base de datos

Para crear la base:

```bash
dotnet ef database update
```

Para crear una migración:

```bash
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

---
## 🔧 Configuración

El archivo appsettings.json incluído está limpio, sin credenciales sensibles.
Agregá tus cadenas de conexión en:

appsettings.Development.json (ignorado en Git)

```Ejemplo:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=TU_DB;Trusted_Connection=True;"
  }
}
```
---

## 👥 Autores

Proyecto desarrollado por:

- Lorenzo Doleatto
- Gallo Agustín
- Fontanarrosa Luciano

---

