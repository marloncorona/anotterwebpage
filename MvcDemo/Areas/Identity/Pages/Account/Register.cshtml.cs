using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MvcDemo.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El email es obligatorio.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page(); // <-- Aquí regresa si hay error

            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToPage("/Account/LoginSuccess");
            }

            // Mostrar errores reales en pantalla
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return Page();
        }
    }
}
