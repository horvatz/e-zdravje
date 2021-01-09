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
    public class ReferralsController : Controller
    {
        private readonly PatientContext _context;
        private readonly UserManager<User> _usermanager;

        public ReferralsController(PatientContext context, UserManager<User> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        // GET: Referrals
        [Authorize(Roles = "Administrator, Zdravnik, Pacient")]
        public async Task<IActionResult> Index()
        {
            var user = await _usermanager.GetUserAsync(User);
            var roles = await _usermanager.GetRolesAsync(user);

            if (roles.Contains("Zdravnik"))
            {
                var currentSpecialist = await _context.Specialists.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

                var referrals = _context.Referrals.Include(r => r.Patient).Include(r => r.Specialist).Include(r => r.SpecialistCategory).Where(r => r.SpecialistId == currentSpecialist.Id);

                return View(await referrals.ToListAsync());
            }
            else if (roles.Contains("Pacient"))
            {
                var currentPatient = await _context.Patients.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

                var referrals = _context.Referrals.Include(r => r.Patient).Include(r => r.Specialist).Include(r => r.SpecialistCategory).Where(r => r.PatientId == currentPatient.Id);

                return View(await referrals.ToListAsync());
            }
            else
            {
                var referrals = _context.Referrals.Include(r => r.Patient).Include(r => r.Specialist).Include(r => r.SpecialistCategory);
                return View(await referrals.ToListAsync());
            }
        }

        // GET: Referrals/Details/5
        [Authorize(Roles = "Administrator, Zdravnik, Pacient")]
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
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Create()
        {
            var user = await _usermanager.GetUserAsync(User);
            var roles = await _usermanager.GetRolesAsync(user);

            

            if(roles.Contains("Zdravnik"))
            {
                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
                .ToList();

                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})",
                    SpecialistId = s.SpecialistId
                }).Where(s => s.SpecialistId == doctors[0].Id)
                .ToList();

                ViewData["PatientId"] = new SelectList(patients, "Id", "Item");
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Item");
            } else
            {
                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
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
            }

            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Name");
            return View();
        }

        // POST: Referrals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Category,IsUsed,SpecialistId,PatientId,SpecialistCategoryId")] Referral referral)
        {
            var currentUser = await _usermanager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                //referral.SpecialistId = currentUser;// cuurent logged in doc
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
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _usermanager.GetUserAsync(User);
            var roles = await _usermanager.GetRolesAsync(user);

            if (id == null)
            {
                return NotFound();
            }

            var referral = await _context.Referrals.FindAsync(id);
            if (referral == null)
            {
                return NotFound();
            }


            if (roles.Contains("Zdravnik"))
            {
                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
                .ToList();

                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Item");

                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Patient = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})",
                    SpecialistId = s.SpecialistId

                }).Where(s => s.SpecialistId == doctors[0].Id)
               .ToList();

                ViewBag.PatientId = new SelectList(patients, "Id", "Patient", null);
            }
            else
            {
                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Ustanova: {s.Street})"
                })
                .ToList();

                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Item");

                var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Patient = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})"
                })
                .ToList();

                ViewBag.PatientId = new SelectList(patients, "Id", "Patient", null);
            }

            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Name", referral.SpecialistCategoryId);
            return View(referral);
        }

        // POST: Referrals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zdravnik")]
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
        [Authorize(Roles = "Administrator, Zdravnik")]
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
        [Authorize(Roles = "Administrator, Zdravnik")]
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
