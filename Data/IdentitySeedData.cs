using Microsoft.AspNetCore.Identity;

namespace CineTix.Data;

public static class IdentitySeedData
{
    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var roleName in new[] { AdminRole, CustomerRole })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Không thể tạo role {roleName}.");
                }
            }
        }

        var adminEmail = configuration["Identity:SeedAdminEmail"];
        var adminPassword = configuration["Identity:SeedAdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Không thể tạo tài khoản Admin: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(admin, AdminRole);
            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException("Không thể gán quyền Admin cho tài khoản mẫu.");
            }
        }
    }
}
