using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Services.Authentication
{
    public class AuthenticationSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthenticationSeeder(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void AddRoles()
        {
            var tAdmin = CreateAdminRole(_roleManager);
            tAdmin.Wait();

            var tUser = CreateUserRole(_roleManager);
            tUser.Wait();
        }

        public void AddAdmin()
        {
            var tAdmin = CreateAdminIfNotExists();
            tAdmin.Wait();
        }
        private static async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        private static async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        private async Task CreateAdminIfNotExists()
        {
            var adminInDb = await _userManager.FindByEmailAsync("admin@admin.com");
            if (adminInDb == null)
            {
                var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
                var adminCreated = await _userManager.CreateAsync(admin, "admin123");

                if (adminCreated.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
