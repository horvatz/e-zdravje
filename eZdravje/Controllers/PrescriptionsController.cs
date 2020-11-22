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
            return View();
        }

        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Free,IsUsed,PatientId")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulatePatientsDropDownList(prescription.PatientId);
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
                i => i.IsUsed))
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
            var patientsQuery = from p in _context.Patients
                                orderby p.Name
                                select p;
            ViewBag.PatientId = new SelectList(patientsQuery.AsNoTracking(), "Id", "Name", selectedPatient);
        }
    }
}
