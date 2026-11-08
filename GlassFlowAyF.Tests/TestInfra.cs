using System.Net;
using System.Security.Claims;
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace GlassFlowAyF.Tests;

/// <summary>Base SQLite relacional en memoria con datos mínimos para el Módulo 15.</summary>
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _conexion;

    public const string CorreoCliente = "cliente@test.com";
    public const string CorreoOtro = "otro@test.com";
    public const string CorreoAdmin = "admin@test.com";
    public const string CorreoCotizador = "cotizador@test.com";
    public const string CorreoOperaciones = "operaciones@test.com";

    public string ClienteId { get; private set; } = "";
    public string OtroClienteId { get; private set; } = "";
    public string AdminId { get; private set; } = "";
    public int SolicitudId { get; private set; }
    public int SolicitudSinPropuestaId { get; private set; }
    public int CotizacionId { get; private set; }
    public int DisenoId { get; private set; }

    public FakeEmailService Email { get; } = new();

    public TestDb()
    {
        _conexion = new SqliteConnection("DataSource=:memory:");
        _conexion.Open();
        using var ctx = NuevoContexto();
        ctx.Database.EnsureCreated();
        Sembrar(ctx);
    }

    public ApplicationDbContext NuevoContexto() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_conexion).Options);

    public RevisionClienteService Servicio(ApplicationDbContext ctx)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Notificaciones:CotizadorEmail"] = CorreoCotizador,
            ["Notificaciones:OperacionesEmail"] = CorreoOperaciones
        }).Build();

        var notificaciones = new NotificacionService(ctx, Email, config, NullLogger<NotificacionService>.Instance);
        return new RevisionClienteService(ctx, notificaciones, NullLogger<RevisionClienteService>.Instance);
    }

    public ComandoRevision Comando(ApplicationDbContext ctx, string? comentario = null, bool terminos = true,
        string? usuarioId = null, int? cotizacionId = null)
    {
        var id = cotizacionId ?? CotizacionId;
        var version = ctx.Cotizaciones.AsNoTracking().Where(c => c.Id == id).Select(c => c.Version).First();
        return new ComandoRevision(id, version, comentario, terminos, usuarioId ?? ClienteId,
            "203.0.113.10", "xunit-agent");
    }

    public static UserManager<ApplicationUser> UserManager(ApplicationDbContext ctx) =>
        new(new UserStore<ApplicationUser>(ctx), Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(), Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(), new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<ApplicationUser>>.Instance);

    /// <summary>Prepara ControllerContext, usuario autenticado, IP y TempData.</summary>
    public static T ConUsuario<T>(T controller, string usuarioId, string rol) where T : Controller
    {
        var identidad = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId),
            new Claim(ClaimTypes.Role, rol)
        }, "Test");

        var http = new DefaultHttpContext { User = new ClaimsPrincipal(identidad) };
        http.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.7");
        http.Request.Headers.UserAgent = "Navegador-Pruebas/1.0";

        controller.ControllerContext = new ControllerContext { HttpContext = http };
        controller.TempData = new TempDataDictionary(http, new FakeTempDataProvider());
        return controller;
    }

    private void Sembrar(ApplicationDbContext ctx)
    {
        var rolAdmin = new IdentityRole("Administrador") { NormalizedName = "ADMINISTRADOR" };
        var rolCliente = new IdentityRole("Cliente") { NormalizedName = "CLIENTE" };
        ctx.Roles.AddRange(rolAdmin, rolCliente);

        ApplicationUser Usuario(string correo, string nombre) => new()
        {
            UserName = correo, NormalizedUserName = correo.ToUpperInvariant(),
            Email = correo, NormalizedEmail = correo.ToUpperInvariant(),
            NombreCompleto = nombre, Cedula = "100000000", Activo = true
        };

        var cliente = Usuario(CorreoCliente, "Cliente Prueba");
        var otro = Usuario(CorreoOtro, "Otro Cliente");
        var admin = Usuario(CorreoAdmin, "Admin Prueba");
        ctx.Users.AddRange(cliente, otro, admin);
        ctx.SaveChanges();

        ctx.UserRoles.AddRange(
            new IdentityUserRole<string> { UserId = admin.Id, RoleId = rolAdmin.Id },
            new IdentityUserRole<string> { UserId = cliente.Id, RoleId = rolCliente.Id },
            new IdentityUserRole<string> { UserId = otro.Id, RoleId = rolCliente.Id });

        var material = new Material
        {
            Nombre = "Vidrio templado claro", Tipo = "Vidrio templado", Grosor = 8,
            Perfil = "Aluminio serie 80", Color = "Negro mate", Acabado = "Anodizado", PrecioAdicional = 0
        };
        ctx.Materiales.Add(material);

        SolicitudCotizacion NuevaSolicitud(string estado) => new()
        {
            NombreCliente = "Cliente Prueba", Correo = CorreoCliente, Telefono = "88888888",
            ProductoId = 1, TipoProducto = "Puerta de baño corrediza", Material = material,
            Descripcion = "Puerta de baño", Ancho = 1.2m, Alto = 2m, Cantidad = 2,
            UnidadMedida = "m", UbicacionInstalacion = "Baño", Estado = estado
        };

        var solicitud = NuevaSolicitud(EstadosRevision.Cotizado);
        var sinPropuesta = NuevaSolicitud("En revisión");
        ctx.SolicitudesCotizacion.AddRange(solicitud, sinPropuesta);
        ctx.SaveChanges();

        var diseno = new DisenoIA
        {
            SolicitudCotizacionId = solicitud.Id, Estilo = "Minimalista", ColorPerfil = "Negro mate",
            RutaImagenGenerada = "/uploads/disenos/render1.png", EsFavorito = true, Recomendacion = "Usar herrajes ocultos."
        };
        ctx.DisenosIA.Add(diseno);

        var cotizacion = new Cotizacion
        {
            SolicitudCotizacionId = solicitud.Id, CostoMateriales = 500, ManoObra = 200, CostoInstalacion = 100,
            OtrosCostos = 0, Subtotal = 800, PorcentajeImpuesto = 13, Impuesto = 104, Descuento = 4, Total = 900,
            Estado = EstadosRevision.CotPendiente, Condiciones = "50% de adelanto.",
            FechaVencimiento = DateTime.Today.AddDays(10)
        };
        ctx.Cotizaciones.Add(cotizacion);
        ctx.SaveChanges();

        ClienteId = cliente.Id;
        OtroClienteId = otro.Id;
        AdminId = admin.Id;
        SolicitudId = solicitud.Id;
        SolicitudSinPropuestaId = sinPropuesta.Id;
        CotizacionId = cotizacion.Id;
        DisenoId = diseno.Id;
    }

    public void Dispose() => _conexion.Dispose();
}

public sealed class FakeEmailService : IEmailService
{
    public List<(string Destino, string Asunto, string Cuerpo)> Enviados { get; } = new();
    public bool Fallar { get; set; }

    public Task EnviarCorreoAsync(string destinatario, string asunto, string contenidoHtml)
    {
        if (Fallar) throw new InvalidOperationException("SMTP no disponible");
        Enviados.Add((destinatario, asunto, contenidoHtml));
        return Task.CompletedTask;
    }
}

public sealed class FakeTempDataProvider : ITempDataProvider
{
    private IDictionary<string, object> _datos = new Dictionary<string, object>();
    public IDictionary<string, object> LoadTempData(HttpContext context) => _datos;
    public void SaveTempData(HttpContext context, IDictionary<string, object> values) => _datos = values;
}
