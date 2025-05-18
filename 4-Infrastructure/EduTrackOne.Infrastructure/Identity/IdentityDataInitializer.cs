using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Infrastructure.Identity
{
    public static class IdentityDataInitializer
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Crée les rôles si non existants
            string[] roles = new[] { "Admin", "Enseignant" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Création d'un utilisateur de test
            var utilisateur = await userManager.FindByNameAsync("karine");
            if (utilisateur == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = "Karine",
                    Email = "karine.bille@arcenciel.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newUser, "Passw0rd!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Enseignant");
                }
                else
                {
                    // Log ou affiche les erreurs
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Erreur : {error.Description}");
                    }
                }
            }
            // Création de l'utilisateur Admin
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@arcenciel.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"Erreur création admin : {error.Description}");
                }
            }
        }
    }
}