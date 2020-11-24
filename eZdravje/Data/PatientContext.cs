using eZdravje.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace eZdravje.Data
{
    public class PatientContext : IdentityDbContext<User>
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<SpecialistCategory> SpecialistCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
