using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Edwin",
                    Email = "Edwin@test.com",
                    UserName = "Edwin@test.com",
                    Address = new Address
                    {
                        FirstName = "Edwin",
                        LastName = "Jeba",
                        Street = "Salah-Al-Din Street",
                        City = "Dubai",
                        ZipCode = "00000"
                    }
                };

                await userManager.CreateAsync(user, "P@ssw0rd");
            }
        }
    }
}