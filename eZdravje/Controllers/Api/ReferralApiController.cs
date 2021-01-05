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
    [Route("api/v1/Napotnice")]
    [ApiController]
    [ApiKeyAuth]
    public class ReferralApiController : ControllerBase
    {
        private readonly PatientContext _context;

        public ReferralApiController(PatientContext context)
        {
            _context = context;
        }

        // GET: api/ReferralApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referral>>> GetReferrals()
        {
            return await _context.Referrals.ToListAsync();
        }

        // GET: api/ReferralApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Referral>> GetReferral(int id)
        {
            var referral = await _context.Referrals.FindAsync(id);

            if (referral == null)
            {
                return NotFound();
            }

            return referral;
        }

        // PUT: api/ReferralApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReferral(int id, Referral referral)
        {
            if (id != referral.Id)
            {
                return BadRequest();
            }

            _context.Entry(referral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReferralExists(id))
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

        // POST: api/ReferralApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Referral>> PostReferral(Referral referral)
        {
            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReferral", new { id = referral.Id }, referral);
        }

        // DELETE: api/ReferralApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Referral>> DeleteReferral(int id)
        {
            var referral = await _context.Referrals.FindAsync(id);
            if (referral == null)
            {
                return NotFound();
            }

            _context.Referrals.Remove(referral);
            await _context.SaveChangesAsync();

            return referral;
        }

        private bool ReferralExists(int id)
        {
            return _context.Referrals.Any(e => e.Id == id);
        }
    }
}
