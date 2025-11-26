using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP_MVC_CRUD.Data;
using TP_MVC_CRUD.Filters;
using TP_MVC_CRUD.Models;

namespace TP_MVC_CRUD.Controllers
{
    [FiltroAutorizacion(soloAdmin: true)]
    public class DireccionController : Controller
    {
        private readonly AppDbContext _context;

        public DireccionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Direccion
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Direcciones.Include(d => d.Cliente);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Direccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direcciones
                .Include(d => d.Cliente)
                .FirstOrDefaultAsync(m => m.DireccionId == id);
            if (direccion == null)
            {
                return NotFound();
            }

            return View(direccion);
        }

        // GET: Direccion/Create
        public IActionResult Create()
        {
            ViewBag.ClienteId = new SelectList( _context.Clientes.Select(c => new { c.ClienteId, NombreCompleto = c.Nombre + " " + c.Apellido }),
                                    "ClienteId", "NombreCompleto");
            return View();
        }

        // POST: Direccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DireccionId,Calle,Localidad,ClienteId")] Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(direccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "ClienteId", direccion.ClienteId);
            return View(direccion);
        }

        // GET: Direccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direcciones.FindAsync(id);
            if (direccion == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "ClienteId", direccion.ClienteId);
            return View(direccion);
        }

        // POST: Direccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DireccionId,Calle,Localidad,ClienteId")] Direccion direccion)
        {
            if (id != direccion.DireccionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(direccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DireccionExists(direccion.DireccionId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nombre", direccion.ClienteId);
            return View(direccion);
        }

        // GET: Direccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direcciones
                .Include(d => d.Cliente)
                .FirstOrDefaultAsync(m => m.DireccionId == id);
            if (direccion == null)
            {
                return NotFound();
            }

            return View(direccion);
        }

        // POST: Direccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var direccion = await _context.Direcciones.FindAsync(id);
            if (direccion != null)
            {
                _context.Direcciones.Remove(direccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DireccionExists(int id)
        {
            return _context.Direcciones.Any(e => e.DireccionId == id);
        }
    }
}
