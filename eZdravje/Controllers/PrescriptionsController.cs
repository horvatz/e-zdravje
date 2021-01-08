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
using Microsoft.AspNetCore.Identity;

namespace eZdravje.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly PatientContext _context;
        private readonly UserManager<User> _usermanager;

        public PrescriptionsController(PatientContext context, UserManager<User> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        // GET: Prescriptions
        [Authorize(Roles = "Administrator, Zdravnik, Pacient")]
        public async Task<IActionResult> Index()
        {
            
            var user =  await _usermanager.GetUserAsync(User);
            var roles =  await _usermanager.GetRolesAsync(user);


            if (roles.Contains("Zdravnik"))
            {
                var currentSpecialist = await _context.Specialists.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

                var prescriptions =  _context.Prescriptions
                .Include(c => c.Patient)
                .Include(s => s.Specialist)
                .Where(s => s.SpecialistId == currentSpecialist.Id)
                .AsNoTracking();

                return View(await prescriptions.ToListAsync());
            }
            else if(roles.Contains("Pacient"))
            {
                var currentPatient = await _context.Patients.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

                var prescriptions = _context.Prescriptions
                .Include(c => c.Patient)
                .Include(s => s.Specialist)
                .Where(c => c.PatientId == currentPatient.Id)
                .AsNoTracking();

                return View(await prescriptions.ToListAsync());
                
            } 
            else
            {
                var prescriptions = _context.Prescriptions
                .Include(c => c.Patient)
                .Include(s => s.Specialist)
                .AsNoTracking();

                return View(await prescriptions.ToListAsync());
            }
        }

        // GET: Prescriptions/Details/5
        [Authorize(Roles = "Administrator, Zdravnik, Pacient")]
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
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Create()
        {
            var user = await _usermanager.GetUserAsync(User);
            var roles = await  _usermanager.GetRolesAsync(user);

            if (roles.Contains("Zdravnik"))
            {
                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
                .ToList();

                var patients = _context.Patients
                .Select(s => new
                {
                   Id = s.Id,
                   Patient = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})",
                   SpecialistId = s.SpecialistId
                   
                })
               .ToList();
                ViewBag.PatientId = new SelectList(patients, "Id", "Patient", null);
                
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
                return View();
            }
            else
            {

                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Patient = $"{s.Name} {s.LastName} (ID: {s.Id}))"
                })
                .ToList();
                ViewBag.PatientId = new SelectList(patients, "Id", "Patient", null);

                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id})"
                })
                .ToList();
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");


                return View();
            }
           
        }

        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Free,IsUsed,PatientId,SpecialistId")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", prescription.SpecialistId);
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        [Authorize(Roles = "Administrator, Zdravnik")]
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

            var user = await _usermanager.GetUserAsync(User);
            var roles = await _usermanager.GetRolesAsync(user);

            if (roles.Contains("Zdravnik"))
            {
                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
                .ToList();

                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Patient = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})",
                    SpecialistId = s.SpecialistId
                })
               .ToList();
                ViewBag.PatientId = new SelectList(patients, "Id", "Patient", null);

                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
                return View(prescription);
            }
            else
            {
                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Patient = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})"
                })
                .ToList();
                ViewBag.PatientId = new SelectList(patients, "Id", "Patient", null);

                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id})"
                })
                .ToList();
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");

                return View(prescription);
            }
        }

        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zdravnik")]
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
        [Authorize(Roles = "Administrator, Zdravnik")]
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
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
