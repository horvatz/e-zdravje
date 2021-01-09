using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eZdravje.Data;
using eZdravje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace eZdravje.Controllers
{
    public class SpecialistsController : Controller
    {
        private readonly PatientContext _context;
        private readonly UserManager<User> _userManager;

        public SpecialistsController(PatientContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Specialists
        
        public async Task<IActionResult> Index()
        {
            var patientContext = _context.Specialists.Include(s => s.SpecialistCategory);
            return View(await patientContext.ToListAsync());
        }

        // GET: Specialists/GetCode
        [Authorize(Roles = "Administrator")]
        public IActionResult GetCode(string code)
        {
            ViewData["Code"] = code;
            return View();
        }

        // GET: Specialists/CodesList
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CodesList()
        {
            var codes = _context.ActivationCodes.Where(s => s.Role == "Zdravnik").OrderBy(s => s.UserId);
            return View(await codes.ToListAsync());
        }

        // GET: Specialists/CodesCreate
        [Authorize(Roles = "Administrator")]
        public IActionResult CodeCreate()
        {
            var doctors = _context.Specialists
               .Select(s => new
               {
                   Id = s.Id,
                   Doc = $"{s.Name} {s.LastName} (ID: {s.Id})",
                   UserId = s.UserId
               })
               .ToList();
            ViewData["SpecialistId"] = new SelectList(doctors, "Id", "Doc");
            return View();
        }

        // POST: Specialists/CodesCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CodeCreate([Bind("Id")] Specialist specialist)
        {
            if (ModelState.IsValid)
            {
                _context.ActivationCodes.RemoveRange(_context.ActivationCodes.Where(a => a.UserId == specialist.Id.ToString()));
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
                    Role = "Zdravnik",
                    UserId = specialist.Id.ToString(),
                    IsUsed = false
                };

                _context.Add(codeObj);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetCode), new { code = codeOut.ToString() });
            }
            return View();
        }

        // POST Specialists/CodesDeactivate

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeactivateCode(int id)
        {
            var code = await _context.ActivationCodes.Where(c => c.Id == id).FirstOrDefaultAsync();
            if(code != null)
            {
                code.IsUsed = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CodesList));
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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Street,PostalCode,City,SpecialistCategoryId")] Specialist specialist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialist);
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
                    Role = "Zdravnik",
                    UserId = specialist.Id.ToString(),
                    IsUsed = false
                };

                _context.Add(codeObj);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetCode), new { code = codeOut.ToString() });
            }
            ViewData["SpecialistCategoryId"] = new SelectList(_context.SpecialistCategories, "Id", "Id", specialist.SpecialistCategoryId);
            return View(specialist);
        }

        // GET: Specialists/Edit/5
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Street,PostalCode,City,SpecialistCategoryId")] Specialist specialist)
        {
            if (id != specialist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var current = await _context.Specialists.AsNoTracking().SingleOrDefaultAsync(s => s.Id == specialist.Id);
                if(current.UserId != null)
                {
                    specialist.UserId = current.UserId;
                }
                
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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialist = await _context.Specialists.FindAsync(id);
            var activationCode = await _context.ActivationCodes.Where(x => x.UserId == id.ToString()).FirstOrDefaultAsync();
            _context.ActivationCodes.Remove(activationCode);
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
