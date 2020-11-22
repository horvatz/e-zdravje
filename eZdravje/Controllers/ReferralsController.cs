using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eZdravje.Data;
using eZdravje.Models;

namespace eZdravje.Controllers
{
    public class ReferralsController : Controller
    {
        private readonly PatientContext _context;

        public ReferralsController(PatientContext context)
        {
            _context = context;
        }

        // GET: Referrals
        public async Task<IActionResult> Index()
        {
            var patientContext = _context.Referrals.Include(r => r.Patient).Include(r => r.Specialist).Include(r => r.SpecialistCategory);
            return View(await patientContext.ToListAsync());
        }

        // GET: Referrals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referral = await _context.Referrals
                .Include(r => r.Patient)
                .Include(r => r.Specialist)
                .Include(r => r.SpecialistCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (referral == null)
            {
                return NotFound();
            }

            return View(referral);
        }

        // GET: Referrals/Create
        public IActionResult Create()
        {
            var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})"
                })

                .ToList();

            var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})"
                })
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "Id", "Item");
            ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Item");
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Name");
            return View();
        }

        // POST: Referrals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Category,IsUsed,SpecialistId,PatientId,SpecialistCategoryId")] Referral referral)
        {
            if (ModelState.IsValid)
            {
                _context.Add(referral);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", referral.PatientId);
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", referral.SpecialistId);
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Id", referral.SpecialistCategoryId);
            return View(referral);
        }

        // GET: Referrals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referral = await _context.Referrals.FindAsync(id);
            if (referral == null)
            {
                return NotFound();
            }

            var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})"
                })

                .ToList();

            var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})"
                })
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "Id", "Item", referral.PatientId);
            ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Item", referral.SpecialistId);
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Name", referral.SpecialistCategoryId);
            return View(referral);
        }

        // POST: Referrals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Category,IsUsed,SpecialistId,PatientId,SpecialistCategoryId")] Referral referral)
        {
            if (id != referral.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(referral);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReferralExists(referral.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", referral.PatientId);
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", referral.SpecialistId);
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Id", referral.SpecialistCategoryId);
            return View(referral);
        }

        // GET: Referrals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referral = await _context.Referrals
                .Include(r => r.Patient)
                .Include(r => r.Specialist)
                .Include(r => r.SpecialistCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (referral == null)
            {
                return NotFound();
            }

            return View(referral);
        }

        // POST: Referrals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var referral = await _context.Referrals.FindAsync(id);
            _context.Referrals.Remove(referral);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReferralExists(int id)
        {
            return _context.Referrals.Any(e => e.Id == id);
        }
    }
}
