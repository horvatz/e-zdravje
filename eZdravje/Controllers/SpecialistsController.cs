using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eZdravje.Data;
using eZdravje.Models;
using Microsoft.AspNetCore.Authorization;

namespace eZdravje.Controllers
{
    public class SpecialistsController : Controller
    {
        private readonly PatientContext _context;

        public SpecialistsController(PatientContext context)
        {
            _context = context;
        }

        // GET: Specialists
        public async Task<IActionResult> Index()
        {
            var patientContext = _context.Specialists.Include(s => s.SpecialistCategory);
            return View(await patientContext.ToListAsync());
        }

        // GET: Specialists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialists
                .Include(s => s.SpecialistCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialist == null)
            {
                return NotFound();
            }

            return View(specialist);
        }

        // GET: Specialists/Create
        [Authorize(Roles = "Administrator, Direktor")]
        public IActionResult Create()
        {
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Name");
            return View();
        }

        // POST: Specialists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Street,PostalCode,City,SpecialistCategoryId")] Specialist specialist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Id", specialist.SpecialistCategoryId);
            return View(specialist);
        }

        // GET: Specialists/Edit/5
        [Authorize(Roles = "Administrator, Direktor, Specialist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialists.FindAsync(id);
            if (specialist == null)
            {
                return NotFound();
            }
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Name", specialist.SpecialistCategoryId);
            return View(specialist);
        }

        // POST: Specialists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Street,PostalCode,City,SpecialistCategoryId")] Specialist specialist)
        {
            if (id != specialist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialistExists(specialist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Id", specialist.SpecialistCategoryId);
            return View(specialist);
        }

        // GET: Specialists/Delete/5
        [Authorize(Roles = "Administrator, Direktor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Specialists
                .Include(s => s.SpecialistCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialist == null)
            {
                return NotFound();
            }

            return View(specialist);
        }

        // POST: Specialists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialist = await _context.Specialists.FindAsync(id);
            _context.Specialists.Remove(specialist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialistExists(int id)
        {
            return _context.Specialists.Any(e => e.Id == id);
        }
    }
}
