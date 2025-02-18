using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Services.Authentication
{
    public class AuthenticationSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public void AddRoles()
        {
            var tAdmin = CreateAdminRole(_roleManager);
            tAdmin.Wait();

            var tUser = CreateUserRole(_roleManager);
            tUser.Wait();
        }

        private static async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        private static async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
}
