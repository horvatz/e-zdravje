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
    [Route("api/v1/Zdravniki")]
    [ApiController]
    [ApiKeyAuth]
    public class SpecialistApiController : ControllerBase
    {
        private readonly PatientContext _context;

        public SpecialistApiController(PatientContext context)
        {
            _context = context;
        }

        // GET: api/SpecialistApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialist>>> GetSpecialists()
        {
            return await _context.Specialists.ToListAsync();
        }

        // GET: api/SpecialistApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialist>> GetSpecialist(int id)
        {
            var specialist = await _context.Specialists.FindAsync(id);

            if (specialist == null)
            {
                return NotFound();
            }

            return specialist;
        }

        // PUT: api/SpecialistApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialist(int id, Specialist specialist)
        {
            if (id != specialist.Id)
            {
                return BadRequest();
            }

            _context.Entry(specialist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SpecialistApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Specialist>> PostSpecialist(Specialist specialist)
        {
            _context.Specialists.Add(specialist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecialist", new { id = specialist.Id }, specialist);
        }

        // DELETE: api/SpecialistApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Specialist>> DeleteSpecialist(int id)
        {
            var specialist = await _context.Specialists.FindAsync(id);
            if (specialist == null)
            {
                return NotFound();
            }

            _context.Specialists.Remove(specialist);
            await _context.SaveChangesAsync();

            return specialist;
        }

        private bool SpecialistExists(int id)
        {
            return _context.Specialists.Any(e => e.Id == id);
        }
    }
}
