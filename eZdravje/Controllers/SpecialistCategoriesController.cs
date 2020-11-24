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
    public class SpecialistCategoriesController : Controller
    {
        private readonly PatientContext _context;

        public SpecialistCategoriesController(PatientContext context)
        {
            _context = context;
        }

        // GET: SpecialistCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.SpecialistCategories.ToListAsync());
        }

        // GET: SpecialistCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialistCategory = await _context.SpecialistCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialistCategory == null)
            {
                return NotFound();
            }

            return View(specialistCategory);
        }

        // GET: SpecialistCategories/Create
        [Authorize(Roles = "Administrator, Direktor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SpecialistCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Direktor")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] SpecialistCategory specialistCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialistCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialistCategory);
        }

        // GET: SpecialistCategories/Edit/5
        [Authorize(Roles = "Administrator, Direktor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialistCategory = await _context.SpecialistCategories.FindAsync(id);
            if (specialistCategory == null)
            {
                return NotFound();
            }
            return View(specialistCategory);
        }

        // POST: SpecialistCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Direktor")]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Description")] SpecialistCategory specialistCategory)
        {
            if (id != specialistCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialistCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialistCategoryExists(specialistCategory.Id))
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
            return View(specialistCategory);
        }

        // GET: SpecialistCategories/Delete/5
        [Authorize(Roles = "Administrator, Direktor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialistCategory = await _context.SpecialistCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialistCategory == null)
            {
                return NotFound();
            }

            return View(specialistCategory);
        }

        // POST: SpecialistCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Direktor")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var specialistCategory = await _context.SpecialistCategories.FindAsync(id);
            _context.SpecialistCategories.Remove(specialistCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialistCategoryExists(int id)
        {
            return _context.SpecialistCategories.Any(e => e.Id == id);
        }
    }
}
