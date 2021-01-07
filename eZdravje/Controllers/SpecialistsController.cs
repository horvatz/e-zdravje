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
