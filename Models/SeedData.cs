using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using proiectasp.Data;

namespace proiectasp.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService
                <DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Roles.Any())
                    return;


                context.Roles.AddRange(
                    new IdentityRole { Id = "c0aa1c14-5c8a-4a71-a90c-782218561580", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "c0aa1c14-5c8a-4a71-a90c-782218561581", Name = "User", NormalizedName = "User".ToUpper() },
                    new IdentityRole { Id = "c0aa1c14-5c8a-4a71-a90c-782218561582", Name = "Guest", NormalizedName = "Guest".ToUpper() }
                );

                var hasher = new PasswordHasher<ApplicationUser>();

                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "babe7d48-fa91-4ea1-bc30-295ae105926c",
                        UserName = "Admin",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN",
                        Signature = null,
                        LockStatus = true,
                        PasswordHash = hasher.HashPassword(null, "Adminpass!0")
                    },
                    new ApplicationUser
                    {
                        Id = "babe7d48-fa91-4ea1-bc30-295ae105926d",
                        UserName = "User01",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER",
                        Signature = "Just a user.",
                        LockStatus = false,
                        PasswordHash = hasher.HashPassword(null, "Userpass!0")
                    }
                );

                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "c0aa1c14-5c8a-4a71-a90c-782218561580",
                        UserId = "babe7d48-fa91-4ea1-bc30-295ae105926c"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "c0aa1c14-5c8a-4a71-a90c-782218561581",
                        UserId = "babe7d48-fa91-4ea1-bc30-295ae105926d"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
