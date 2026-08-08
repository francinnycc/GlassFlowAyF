using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlassFlowAyF.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existe =
                await _userManager.FindByEmailAsync(model.Email);

            if (existe != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Este correo electrónico ya está registrado.");

                return View(model);
            }

            var usuario = new ApplicationUser
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
                    "Index",
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

            if (usuario == null || !usuario.Activo)
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
            ModelState.AddModelError(
                string.Empty,
                "Correo o contraseña incorrectos.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}