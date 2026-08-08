using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString =
    builder.Configuration
        .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena DefaultConnection.");

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(
        options =>
        {
            options.Password.RequiredLength = 6;

            options.Password.RequireDigit = true;

            options.Password.RequireUppercase = true;

            options.Password.RequireLowercase = true;

            options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;

            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(10);
        })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(
    options =>
    {
        options.LoginPath =
            "/Account/Login";

        options.AccessDeniedPath =
            "/Account/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(2);

        options.SlidingExpiration = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

/*
 * MUY IMPORTANTE:
 * Authentication debe ir antes de Authorization.
 */
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}");

await CrearRolesYAdministradorAsync(app);

app.Run();


static async Task CrearRolesYAdministradorAsync(
    WebApplication app)
{
    using var scope =
        app.Services.CreateScope();

    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<
                UserManager<ApplicationUser>>();

    string[] roles =
    {
        "Administrador",
        "Cliente"
    };

    foreach (var rol in roles)
    {
        if (!await roleManager
            .RoleExistsAsync(rol))
        {
            await roleManager
                .CreateAsync(
                    new IdentityRole(rol));
        }
    }

    const string adminEmail =
        "admin@glassflowaf.com";

    var admin =
        await userManager
            .FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin =
            new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,

                NombreCompleto =
                    "Administrador GlassFlow",

                EmailConfirmed = true,

                Activo = true,

                FechaRegistro =
                    DateTime.Now
            };

        var resultado =
            await userManager.CreateAsync(
                admin,
                "Admin123");

        if (resultado.Succeeded)
        {
            await userManager
                .AddToRoleAsync(
                    admin,
                    "Administrador");
        }
    }
}