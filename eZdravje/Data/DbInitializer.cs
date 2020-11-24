using eZdravje.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace eZdravje.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PatientContext context)
        {
            context.Database.EnsureCreated();

            if (context.Roles.Any())
            {
                return;
            }

            string[] roles = new string[] { "Administrator", "Direktor", "Specialist", "Pacient"};

           
            foreach(string r in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var nr = new IdentityRole(r);
                nr.NormalizedName = r.ToUpper();
                roleStore.CreateAsync(nr);
            }
            

            /*foreach (IdentityRole r in roles)
            {
                context.Roles.Add(r);
              
            }*/

            context.SaveChanges();
        }
    }
}