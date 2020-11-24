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

            var roles = new IdentityRole[] 
            {
                new IdentityRole{Id="1", Name="Administrator"},
                new IdentityRole{Id="2", Name="Direktor"},
                new IdentityRole{Id="3", Name="Specialist"},
                new IdentityRole{Id="4", Name="Pacient"}
            };

            foreach (IdentityRole r in roles)
            {
                context.Roles.Add(r);
            }

            var user = new User
            {
                Email = "menci@fri.si",
                UserName = "menci",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };


            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(user,"Testni123!");
                user.PasswordHash = hashed;
                context.Users.Add(user);
                
            }

            var UserRoles = new IdentityUserRole<string>[]
            {
                new IdentityUserRole<string>{RoleId = roles[0].Id, UserId=user.Id}
            };

            foreach (IdentityUserRole<string> r in UserRoles)
            {
                context.UserRoles.Add(r);
            }

            user = new User
            {
                Email = "horvat@fri.si",
                UserName = "horvat",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };


            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(user,"Testni123!");
                user.PasswordHash = hashed;
                context.Users.Add(user);
                
            }

            UserRoles = new IdentityUserRole<string>[]
            {
                new IdentityUserRole<string>{RoleId = roles[0].Id, UserId=user.Id}
            };

            foreach (IdentityUserRole<string> r in UserRoles)
            {
                context.UserRoles.Add(r);
            }
            context.SaveChanges();
        }
    }
}