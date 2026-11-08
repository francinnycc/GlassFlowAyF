using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder =
    WebApplication.CreateBuilder(args);


// =======================================================
// MVC
// =======================================================

builder.Services
    .AddControllersWithViews();

builder.Services
    .AddScoped<
        IEmailService,
        EmailService>();

// Módulo 15 – Revisión y Aprobación del Cliente
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IRevisionClienteService, RevisionClienteService>();

// IP real del cliente para la auditoría cuando hay un proxy local (solo se confía en loopback por defecto).
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(o =>
    o.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                       | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto);


// =======================================================
// SERVICIO OPENAI
// =======================================================

builder.Services
    .AddHttpClient<
        IOpenAIService,
        OpenAIService>(
        client =>
        {
            client.BaseAddress =
                new Uri(
                    "https://api.openai.com/");

            client.Timeout =
                TimeSpan.FromMinutes(3);
        });


// =======================================================
// MYSQL
// =======================================================

var connectionString =
    builder.Configuration
        .GetConnectionString(
            "DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena DefaultConnection.");


builder.Services
    .AddDbContext<ApplicationDbContext>(
        options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(
                    connectionString)));


// =======================================================
// IDENTITY
// =======================================================

builder.Services
    .AddIdentity<
        ApplicationUser,
        IdentityRole>(
        options =>
        {
            options.Password
                .RequiredLength = 6;

            options.Password
                .RequireDigit = true;

            options.Password
                .RequireUppercase = true;

            options.Password
                .RequireLowercase = true;

            options.Password
                .RequireNonAlphanumeric = false;

            options.User
                .RequireUniqueEmail = true;

            options.Lockout
                .MaxFailedAccessAttempts = 5;

            options.Lockout
                .DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(10);
        })
    .AddEntityFrameworkStores<
        ApplicationDbContext>()
    .AddDefaultTokenProviders();


// =======================================================
// COOKIE
// =======================================================

builder.Services
    .ConfigureApplicationCookie(
        options =>
        {
            options.LoginPath =
                "/Account/Login";

            options.AccessDeniedPath =
                "/Account/AccessDenied";

            options.ExpireTimeSpan =
                TimeSpan.FromHours(2);

            options.SlidingExpiration =
                true;
        });


// =======================================================
// CREAR APP
// =======================================================

var app =
    builder.Build();


// =======================================================
// PIPELINE
// =======================================================

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}");


// =======================================================
// ROLES Y USUARIOS
// =======================================================

await CrearRolesYUsuariosAsync(app);


app.Run();


// =======================================================
// CREAR ROLES Y USUARIOS
// =======================================================

static async Task CrearRolesYUsuariosAsync(
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


    var configuration =
        scope.ServiceProvider
            .GetRequiredService<
                IConfiguration>();


    // ===================================================
    // ROLES
    // ===================================================

    string[] roles =
    {
        "Administrador",
        "Cliente",
        "Instalador",
        "Tecnico"
    };


    foreach (var rol in roles)
    {
        if (!await roleManager
            .RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(
                new IdentityRole(rol));
        }
    }


    // ===================================================
    // ADMINISTRADOR
    // ===================================================

    var adminEmail =
        configuration[
            "SeedAdmin:Email"];


    var adminPassword =
        configuration[
            "SeedAdmin:Password"];


    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPassword))
    {
        var admin =
            await userManager
                .FindByEmailAsync(adminEmail);


        if (admin == null)
        {
            admin =
                new ApplicationUser
                {
                    UserName =
                        adminEmail,

                    Email =
                        adminEmail,

                    NombreCompleto =
                        "Administrador GlassFlow",

                    EmailConfirmed =
                        true,

                    Activo =
                        true,

                    FechaRegistro =
                        DateTime.Now
                };


            var resultado =
                await userManager
                    .CreateAsync(
                        admin,
                        adminPassword);


            if (resultado.Succeeded)
            {
                await userManager
                    .AddToRoleAsync(
                        admin,
                        "Administrador");
            }
        }
    }


    // ===================================================
    // INSTALADORES DEMO
    // ===================================================

    var installerPassword =
        configuration[
            "SeedInstaller:Password"];


    if (!string.IsNullOrWhiteSpace(
        installerPassword))
    {
        await CrearInstalador(
            userManager,
            "instalador1@glassflowaf.com",
            "Carlos Ramírez",
            installerPassword);


        await CrearInstalador(
            userManager,
            "instalador2@glassflowaf.com",
            "Andrés Rodríguez",
            installerPassword);


        await CrearInstalador(
            userManager,
            "instalador3@glassflowaf.com",
            "Luis Fernández",
            installerPassword);
    }


    // ===================================================
    // TÉCNICO DEMO
    // ===================================================

    var tecnicoPassword =
        configuration[
            "SeedTecnico:Password"];


    if (!string.IsNullOrWhiteSpace(
        tecnicoPassword))
    {
        await CrearTecnico(
            userManager,
            "tecnico1@glassflowaf.com",
            "Bruno Fernández",
            tecnicoPassword);
    }


    // ===================================================
    // ADMINISTRADOR ADICIONAL (contraseña en appsettings.Development.json)
    // ===================================================

    var admin10Password =
        configuration[
            "SeedAdmin10:Password"];


    if (!string.IsNullOrWhiteSpace(
        admin10Password))
    {
        await CrearUsuarioConRol(
            userManager,
            "admin10@glassflowaf.com",
            "Administrador GlassFlow 10",
            admin10Password,
            "Administrador");
    }
}


// =======================================================
// CREAR INSTALADOR
// =======================================================

static async Task CrearInstalador(
    UserManager<ApplicationUser> userManager,
    string correo,
    string nombre,
    string password)
{
    var usuario =
        await userManager
            .FindByEmailAsync(correo);


    if (usuario == null)
    {
        usuario =
            new ApplicationUser
            {
                UserName =
                    correo,

                Email =
                    correo,

                NombreCompleto =
                    nombre,

                EmailConfirmed =
                    true,

                Activo =
                    true,

                FechaRegistro =
                    DateTime.Now
            };


        var resultado =
            await userManager
                .CreateAsync(
                    usuario,
                    password);


        if (resultado.Succeeded)
        {
            await userManager
                .AddToRoleAsync(
                    usuario,
                    "Instalador");
        }
    }
}


// =======================================================
// CREAR TÉCNICO
// =======================================================

static async Task CrearTecnico(
    UserManager<ApplicationUser> userManager,
    string correo,
    string nombre,
    string password)
{
    var usuario =
        await userManager
            .FindByEmailAsync(correo);


    if (usuario == null)
    {
        usuario =
            new ApplicationUser
            {
                UserName =
                    correo,

                Email =
                    correo,

                NombreCompleto =
                    nombre,

                EmailConfirmed =
                    true,

                Activo =
                    true,

                FechaRegistro =
                    DateTime.Now
            };


        var resultado =
            await userManager
                .CreateAsync(
                    usuario,
                    password);


        if (resultado.Succeeded)
        {
            await userManager
                .AddToRoleAsync(
                    usuario,
                    "Tecnico");
        }
    }
}

// =======================================================
// CREAR USUARIO CON ROL (genérico)
// Si el usuario ya existe solo garantiza que tenga el rol.
// =======================================================

static async Task CrearUsuarioConRol(
    UserManager<ApplicationUser> userManager,
    string correo,
    string nombre,
    string password,
    string rol)
{
    var usuario =
        await userManager.FindByEmailAsync(correo);

    if (usuario == null)
    {
        usuario = new ApplicationUser
        {
            UserName = correo,
            Email = correo,
            NombreCompleto = nombre,
            EmailConfirmed = true,
            Activo = true,
            FechaRegistro = DateTime.Now
        };

        var resultado =
            await userManager.CreateAsync(usuario, password);

        if (!resultado.Succeeded)
        {
            throw new InvalidOperationException(
                $"No se pudo crear {correo}: " +
                string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }
    }

    if (!await userManager.IsInRoleAsync(usuario, rol))
    {
        await userManager.AddToRoleAsync(usuario, rol);
    }
}
