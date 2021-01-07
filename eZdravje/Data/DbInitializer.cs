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

            var roles = new IdentityRole[]
            {
                new IdentityRole{Id="2", Name="Staff"},
                new IdentityRole{Id="3", Name="Patient"}
            };
            

            foreach (IdentityRole r in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                roleStore.CreateAsync(r);
              
            }

            context.SaveChanges();
        }
    }
}