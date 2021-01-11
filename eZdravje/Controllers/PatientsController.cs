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
                var patientContext =  _context.Patients.Include(s => s.Specialist).Where(s => s.SpecialistId == currentSpecialist.Id);
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

        // GET: Patients/CodesList
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CodesList()
        {
            var codes = _context.ActivationCodes.Where(s => s.Role == "Pacient").OrderBy(s => s.UserId);
            return View(await codes.ToListAsync());
        }

        // GET: Patients/CodesCreate
        [Authorize(Roles = "Administrator")]
        public IActionResult CodeCreate()
        {
            var patients = _context.Patients
                .Select(s => new
                {
                    Id = s.Id,
                    Item = $"{s.Name} {s.LastName} (ID: {s.Id}) (Naslov: {s.Street}, {s.PostalCode} {s.City})"
                })

                .ToList();
            ViewData["PatientId"] = new SelectList(patients, "Id", "Item");
            return View();
        }

        // POST: Patients/CodesCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CodeCreate([Bind("Id")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.ActivationCodes.RemoveRange(_context.ActivationCodes.Where(a => a.UserId == patient.Id.ToString()));
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
                return RedirectToAction(nameof(GetCode), new { code = codeOut.ToString() });
            }
            return View();
        }

        // POST Patients/CodesDeactivate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeactivateCode(int id)
        {
            var code = await _context.ActivationCodes.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (code != null)
            {
                code.IsUsed = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CodesList));
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
                var current = await _context.Patients.AsNoTracking().SingleOrDefaultAsync(s => s.Id == patient.Id);
                if (current.UserId != null)
                {
                    patient.UserId = current.UserId;
                }

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
            var activationCode = await _context.ActivationCodes.Where(x => x.UserId == id.ToString()).FirstOrDefaultAsync();
            
            if (activationCode != null)
            {
                _context.ActivationCodes.Remove(activationCode);
            }

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
