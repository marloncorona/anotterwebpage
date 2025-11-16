using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MvcDemo.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                false,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
                return RedirectToPage("/Home/Index");

            ModelState.AddModelError("", "Credenciales inválidas");
            return Page();
        }
    }
}