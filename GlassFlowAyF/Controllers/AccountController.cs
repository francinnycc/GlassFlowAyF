using System.Text;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GlassFlowAyF.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        // ===================================================
        // REGISTRO
        // ===================================================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var existe =
                await _userManager.FindByEmailAsync(
                    model.Email);


            if (existe != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Este correo electrónico ya está registrado.");

                return View(model);
            }


            var usuario =
                new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    PhoneNumber = model.Telefono,
                    Direccion = model.Direccion,
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };


            var resultado =
                await _userManager.CreateAsync(
                    usuario,
                    model.Password);


            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(
                    usuario,
                    "Cliente");


                await _signInManager.SignInAsync(
                    usuario,
                    isPersistent: false);


                return RedirectToAction(
                    "Dashboard",
                    "Home");
            }


            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }


            return View(model);
        }


        // ===================================================
        // LOGIN
        // ===================================================

        [HttpGet]
        public IActionResult Login(
            string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var usuario =
                await _userManager.FindByEmailAsync(
                    model.Email);


            if (usuario == null ||
                !usuario.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Correo o contraseña incorrectos.");

                return View(model);
            }


            var resultado =
                await _signInManager.PasswordSignInAsync(
                    usuario,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);


            if (resultado.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }


                return RedirectToAction(
                    "Dashboard",
                    "Home");
            }


            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La cuenta está temporalmente bloqueada por varios intentos fallidos.");

                return View(model);
            }


            ModelState.AddModelError(
                string.Empty,
                "Correo o contraseña incorrectos.");


            return View(model);
        }


        // ===================================================
        // RECUPERAR CONTRASEÑA
        // ===================================================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var usuario =
                await _userManager.FindByEmailAsync(
                    model.Email);


            /*
             * Por seguridad no indicamos si el
             * correo existe o no.
             */
            if (usuario == null ||
                !usuario.Activo)
            {
                return RedirectToAction(
                    nameof(ForgotPasswordConfirmation));
            }


            var token =
                await _userManager
                    .GeneratePasswordResetTokenAsync(
                        usuario);


            var tokenCodificado =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));


            var enlace =
                Url.Action(
                    nameof(ResetPassword),
                    "Account",
                    new
                    {
                        email = usuario.Email,
                        token = tokenCodificado
                    },
                    Request.Scheme);


            var contenido =
                $"""
                <div style="font-family: Arial, sans-serif;
                            max-width: 600px;
                            margin: auto;
                            color: #0b2235;">

                    <h2 style="color:#00aeb3;">
                        GlassFlow A&amp;F
                    </h2>

                    <p>
                        Hemos recibido una solicitud
                        para restablecer la contraseña
                        de tu cuenta.
                    </p>

                    <p>
                        Presiona el siguiente botón
                        para crear una nueva contraseña:
                    </p>

                    <p style="margin:30px 0;">
                        <a href="{enlace}"
                           style="
                           background:#00aeb3;
                           color:white;
                           padding:12px 22px;
                           text-decoration:none;
                           border-radius:6px;">
                            Restablecer contraseña
                        </a>
                    </p>

                    <p>
                        Si no solicitaste este cambio,
                        puedes ignorar este correo.
                    </p>

                    <hr />

                    <small>
                        GlassFlow A&amp;F —
                        Diseño • Calidad • Instalación
                    </small>

                </div>
                """;


            await _emailService.EnviarCorreoAsync(
                usuario.Email!,
                "Recuperación de contraseña - GlassFlow A&F",
                contenido);


            return RedirectToAction(
                nameof(ForgotPasswordConfirmation));
        }


        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        // ===================================================
        // RESTABLECER CONTRASEÑA
        // ===================================================

        [HttpGet]
        public IActionResult ResetPassword(
            string? email,
            string? token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    nameof(Login));
            }


            var model =
                new ResetPasswordViewModel
                {
                    Email = email,
                    Token = token
                };


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var usuario =
                await _userManager.FindByEmailAsync(
                    model.Email);


            if (usuario == null)
            {
                return RedirectToAction(
                    nameof(ResetPasswordConfirmation));
            }


            string token;


            try
            {
                token =
                    Encoding.UTF8.GetString(
                        WebEncoders.Base64UrlDecode(
                            model.Token));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El enlace de recuperación no es válido.");

                return View(model);
            }


            var resultado =
                await _userManager.ResetPasswordAsync(
                    usuario,
                    token,
                    model.Password);


            if (resultado.Succeeded)
            {
                return RedirectToAction(
                    nameof(ResetPasswordConfirmation));
            }


            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }


            return View(model);
        }


        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        // ===================================================
        // GESTIÓN DE PERFIL
        // ===================================================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(nameof(Login));
            }


            var roles = await _userManager.GetRolesAsync(usuario);


            var model = new ProfileViewModel
            {
                NombreCompleto = usuario.NombreCompleto ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Telefono = usuario.PhoneNumber ?? string.Empty,
                Direccion = usuario.Direccion ?? string.Empty,
                Cedula = usuario.Cedula ?? string.Empty,
                NumeroContactoEmergencia = usuario.NumeroContactoEmergencia ?? string.Empty,
                AlergiasMedicamentos = usuario.AlergiasMedicamentos ?? string.Empty,
                PadecimientosEnfermedades = usuario.PadecimientosEnfermedades ?? string.Empty,
                Medicamentos = usuario.Medicamentos ?? string.Empty,
                FechaRegistro = usuario.FechaRegistro,
                Rol = roles.FirstOrDefault() ?? "Sin rol"
            };


            return View(model);
        }


        // ===================================================
        // EDITAR PERFIL
        // ===================================================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(nameof(Login));
            }


            var model = new EditProfileViewModel
            {
                NombreCompleto = usuario.NombreCompleto ?? string.Empty,
                Telefono = usuario.PhoneNumber ?? string.Empty,
                Direccion = usuario.Direccion ?? string.Empty,
                Cedula = usuario.Cedula ?? string.Empty,
                NumeroContactoEmergencia = usuario.NumeroContactoEmergencia ?? string.Empty,
                AlergiasMedicamentos = usuario.AlergiasMedicamentos ?? string.Empty,
                PadecimientosEnfermedades = usuario.PadecimientosEnfermedades ?? string.Empty,
                Medicamentos = usuario.Medicamentos ?? string.Empty
            };


            return View(model);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(
            EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(nameof(Login));
            }


            /*
             * Solo modificamos información personal.
             * El correo, UserName, contraseña y rol
             * permanecen sin cambios.
             */
            usuario.NombreCompleto = model.NombreCompleto.Trim();
            usuario.PhoneNumber = model.Telefono.Trim();
            usuario.Direccion = model.Direccion.Trim();
            usuario.Cedula = NormalizarCedula(model.Cedula);
            usuario.NumeroContactoEmergencia = NormalizarOpcional(model.NumeroContactoEmergencia);
            usuario.AlergiasMedicamentos = NormalizarOpcional(model.AlergiasMedicamentos);
            usuario.PadecimientosEnfermedades = NormalizarOpcional(model.PadecimientosEnfermedades);
            usuario.Medicamentos = NormalizarOpcional(model.Medicamentos);


            var resultado =
                await _userManager.UpdateAsync(usuario);


            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }


            /*
             * Renovamos la cookie de autenticación
             * después de actualizar el usuario.
             */
            await _signInManager.RefreshSignInAsync(usuario);


            TempData["Mensaje"] =
                "Tu información personal fue actualizada correctamente.";


            return RedirectToAction(nameof(Profile));
        }

        // ===================================================
        // CAMBIO DE CONTRASEÑA
        // ===================================================

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(
                new ChangePasswordViewModel());
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var usuario =
                await _userManager.GetUserAsync(User);


            if (usuario == null)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction(
                    nameof(Login));
            }


            var resultado =
                await _userManager.ChangePasswordAsync(
                    usuario,
                    model.CurrentPassword,
                    model.NewPassword);


            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    if (error.Code == "PasswordMismatch")
                    {
                        ModelState.AddModelError(
                            nameof(model.CurrentPassword),
                            "La contraseña actual es incorrecta.");
                    }
                    else
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }
                }


                return View(model);
            }


            /*
             * Renueva la cookie de autenticación
             * después del cambio.
             */
            await _signInManager.RefreshSignInAsync(
                usuario);


            return RedirectToAction(
                nameof(ChangePasswordConfirmation));
        }


        [Authorize]
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }


        // ===================================================
        // LOGOUT
        // ===================================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();


            return RedirectToAction(
                "Index",
                "Home");
        }


        // ===================================================
        // ACCESO DENEGADO
        // ===================================================

        public IActionResult AccessDenied()
        {
            return View();
        }


        // ===================================================
        // NORMALIZACIÓN DE DATOS DEL PERFIL
        // ===================================================

        /*
         * La cédula se almacena como texto de 9 dígitos.
         * Se eliminan los guiones y espacios para que
         * el formato sea siempre consistente en la
         * base de datos.
         */
        private static string NormalizarCedula(
            string? cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                return string.Empty;
            }


            var valor = cedula
                .Trim()
                .Replace(
                    "-",
                    string.Empty)
                .Replace(
                    " ",
                    string.Empty);


            return valor;
        }


        /*
         * Los campos de salud son opcionales.
         * Si el usuario los deja vacíos se guardan
         * como null y no como texto en blanco.
         */
        private static string? NormalizarOpcional(
            string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }


            return valor.Trim();
        }
    }
}