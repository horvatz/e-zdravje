using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
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
    public class PatientsController : Controller
    {
        private readonly PatientContext _context;
        private readonly UserManager<User> _usermanager;

        public PatientsController(PatientContext context, UserManager<User> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        // GET: Patients
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Index()
        {
            var user = await _usermanager.GetUserAsync(User);
            var roles = await _usermanager.GetRolesAsync(user);

            if (roles.Contains("Zdravnik"))
            {
                var currentSpecialist = await _context.Specialists.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                var patientContext = _context.Patients.Include(s => s.Specialist).Where(s => s.SpecialistId == currentSpecialist.Id);
                return View(await patientContext.ToListAsync());
            }
            else
            {
                var patientContext = _context.Patients.Include(s => s.Specialist);
                return View(await patientContext.ToListAsync());
            }


        }

        // GET: Patients/GetCode
        [Authorize(Roles = "Administrator, Zdravnik")]
        public IActionResult GetCode(string code)
        {
            ViewData["Code"] = code;
            return View();
        }

        // GET: Patients/Details/5
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(s => s.Specialist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize(Roles = "Administrator, Zdravnik")]
        // GET: Patients/Create
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
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
                .ToList();
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
                return View();

            } 
            else
            {
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

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Street,PostalCode,City,Birthday,SpecialistId")] Patient patient)
        {
            var currentUser = await _usermanager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();

                var code = DateTime.Now.ToString() + "klgdjfslhghbvcx456";

                var tmpSource = ASCIIEncoding.ASCII.GetBytes(code);
                var tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

                StringBuilder codeOut = new StringBuilder(tmpHash.Length);
                for (int i = 0; i < tmpHash.Length; i++)
                {
                    codeOut.Append(tmpHash[i].ToString("X2"));
                }

                var codeObj = new ActivationCode
                {
                    Code = codeOut.ToString(),
                    Role = "Pacient",
                    UserId = patient.Id.ToString(),
                    IsUsed = false
                };

                _context.Add(codeObj);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetCode), new { code = codeOut.ToString()});
            }
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", patient.SpecialistId);
            return View(patient);
        }

        // GET: Patients/Edit/5

        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
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
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id})",
                    UserId = s.UserId
                }).Where(s => s.UserId == user.Id)
                .ToList();
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
                return View(patient);

            }
            else
            {
                var doctors = _context.Specialists
                .Select(s => new
                {
                    Id = s.Id,
                    Doc = $"{s.Name} {s.LastName} (ID: {s.Id})"
                })
                .ToList();
                ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
                return View(patient);
            }
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Street,PostalCode,City,Birthday,SpecialistId")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            ViewData["SpecialistId"] = new SelectList(_context.Specialists, "Id", "Id", patient.SpecialistId);
            return View(patient);
        }

        // GET: Patients/Delete/5
        [Authorize(Roles = "Administrator, Zdravnik")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(s => s.Specialist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [Authorize(Roles = "Administrator, Zdravnik")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
