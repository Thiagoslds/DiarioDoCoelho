using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiarioDoCoelho.Data;

/// <summary>
/// Responsável por aplicar migrations pendentes e garantir a existência
/// do usuário administrador único que gerencia o CMS do site.
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@diariodocoelho.com.br";
        var adminPassword = configuration["AdminUser:Password"] ?? "Coelho@2024!";

        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Falha ao criar o usuário administrador: {errors}");
            }
        }
    }
}
