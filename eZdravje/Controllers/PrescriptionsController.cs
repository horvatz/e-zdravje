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
    public class PrescriptionsController : Controller
    {
        private readonly PatientContext _context;

        public PrescriptionsController(PatientContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index()
        {
            var prescriptions = _context.Prescriptions
            .Include(c => c.Patient)
            .Include(s => s.Specialist)
            .AsNoTracking();
            return View(await prescriptions.ToListAsync());
            //return View(await _context.Prescriptions.ToListAsync());
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(c => c.Specialist)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // GET: Prescriptions/Create
        public IActionResult Create()
        {
            PopulatePatientsDropDownList();
            var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})"
                })
                .ToList();
            ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
            return View();
        }

        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Free,IsUsed,PatientId,SpecialistId")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulatePatientsDropDownList(prescription.PatientId);
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", prescription.SpecialistId);
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(i => i.Patient)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
                    

            if (prescription == null)
            {
                return NotFound();
            }
            var doctors = _context.Specialists
            .Select(s => new
            {
                Id = s.Id,
                Doc = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})"
            })
            .ToList();
            ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");

            PopulatePatientsDropDownList(prescription.Id);
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPrescription(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var prescriptionToUpdate = await _context.Prescriptions
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (await TryUpdateModelAsync<Prescription>(
                prescriptionToUpdate,
                "",
                i => i.Name, i => i.Description, i => i.Free,
                i => i.IsUsed, i => i.SpecialistId, i => i.PatientId))
                {
                if(String.IsNullOrWhiteSpace(prescriptionToUpdate.Patient?.Name))
                {
                    prescriptionToUpdate.Patient = null;
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Please restart!");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", prescriptionToUpdate.SpecialistId);
            return View(prescriptionToUpdate);
        }

        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(s => s.Specialist)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.Id == id);
        }

        private void PopulatePatientsDropDownList(object selectedPatient = null)
        {
           
            var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Patient = $"{s.Name} {s.LastName} (ID: {s.Id}))"
                })
                .ToList();

            ViewBag.PatientId = new SelectList(patients, "Id", "Patient", selectedPatient);
        }
    }
}
