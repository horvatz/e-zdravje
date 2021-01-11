using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eZdravje.Data;
using eZdravje.Models;
using eZdravje.Filters;

namespace eZdravje.Controllers_Api
{
    [Route("api/v1/Kategorije")]
    [ApiController]
    [ApiKeyAuth]
    public class SpecialistCategoriesApiController : ControllerBase
    {
        private readonly PatientContext _context;

        public SpecialistCategoriesApiController(PatientContext context)
        {
            _context = context;
        }

        // GET: api/SpecialistCategoriesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialistCategory>>> GetSpecialistCategories()
        {
            return await _context.SpecialistCategories.ToListAsync();
        }

        // GET: api/SpecialistCategoriesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialistCategory>> GetSpecialistCategory(int id)
        {
            var specialistCategory = await _context.SpecialistCategories.FindAsync(id);

            if (specialistCategory == null)
            {
                return NotFound();
            }

            return specialistCategory;
        }

        // PUT: api/SpecialistCategoriesApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialistCategory(int id, SpecialistCategory specialistCategory)
        {
            if (id != specialistCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(specialistCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialistCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // POST: api/SpecialistCategoriesApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /*[HttpPost]
        public async Task<ActionResult<SpecialistCategory>> PostSpecialistCategory(SpecialistCategory specialistCategory)
        {
            _context.SpecialistCategories.Add(specialistCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecialistCategory", new { id = specialistCategory.Id }, specialistCategory);
        }*/

        // DELETE: api/SpecialistCategoriesApi/5
        /*[HttpDelete("{id}")]
        public async Task<ActionResult<SpecialistCategory>> DeleteSpecialistCategory(int id)
        {
            var specialistCategory = await _context.SpecialistCategories.FindAsync(id);
            if (specialistCategory == null)
            {
                return NotFound();
            }

            _context.SpecialistCategories.Remove(specialistCategory);
            await _context.SaveChangesAsync();

            return specialistCategory;
        }*/

        private bool SpecialistCategoryExists(int id)
        {
            return _context.SpecialistCategories.Any(e => e.Id == id);
        }
    }
}
