using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using XpertAditusUI.Data;

namespace XpertAditusUI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetUserNameModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetUserNameModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string UserName { get; set; }

            //[Display(Name = "Confirm password")]
            [Compare("UserName", ErrorMessage = "The username and confirmation username do not match.")]
            public string ConfirmUserName { get; set; }

            public string Code { get; set; }
        }
        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for username reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                return RedirectToPage("./ResetUserNameConfirmation");
            }

            if (Input.UserName != user.UserName)
            {
                var result = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetUserNameConfirmation");
                }
              
            }
            else
            {
                var result = await _userManager.SetUserNameAsync(user, Input.UserName);
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
           
            
            return Page();
        }
    }
}
