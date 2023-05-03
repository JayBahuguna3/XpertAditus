using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using XpertAditusUI.Service;

namespace XpertAditusUI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotUserNameModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        readonly UtilityService _utilityService;
        public ForgotUserNameModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, UtilityService utilityService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _utilityService = utilityService;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ModelState.AddModelError(string.Empty, "InValid Email");
                    return Page();
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetUserName",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                string body = $"Please reset your UserName by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                _utilityService.SendMail_ForgotUserName(Input.Email, "Reset UserName", body);

                return RedirectToPage("./ForgotUserNameConfirmation");
            }

            return Page();
        }
    }
}
