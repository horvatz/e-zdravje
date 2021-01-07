using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using eZdravje.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using eZdravje.Data;
using Microsoft.EntityFrameworkCore;

namespace eZdravje.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PatientContext _context;

        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            PatientContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Gesli se ne ujemata.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Activation Code")]
            public string ActivationCode { get; set; }



            //[Required]
            //[EmailAddress]
            //[Display(Name = "Selected Role")]
            //public int SelectedRole { get; set; }
        }
        public List<SelectListItem> Options { get; set; }
        public async Task OnGetAsync(string returnUrl = null)
        {
            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                var codeCheck = await _context.ActivationCodes.FirstOrDefaultAsync(x => x.Code == Input.ActivationCode);
                

                if (codeCheck != null && !codeCheck.IsUsed)
                {
                    var user = new User { UserName = Input.Email, Email = Input.Email };
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");



                        var currentUser = await _userManager.FindByNameAsync(user.UserName);


                        if (!await _roleManager.RoleExistsAsync(codeCheck.Role))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(codeCheck.Role));
                        }

                        var role = await _userManager.AddToRoleAsync(currentUser, codeCheck.Role);

                        if(codeCheck.Role == "Pacient")
                        {
                            var res = _context.Patients.SingleOrDefault(x => x.Id == Int32.Parse(codeCheck.UserId));
                            if(res != null)
                            {
                                res.UserId = currentUser.Id;
                            }

                        } else if(codeCheck.Role == "Zdravnik")
                        {
                            var res = _context.Specialists.SingleOrDefault(x => x.Id == Int32.Parse(codeCheck.UserId));
                            if (res != null)
                            {
                                res.UserId = currentUser.Id;
                            }
                        }

                        codeCheck.IsUsed = true;
                        await _context.SaveChangesAsync();


                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                } 
                else
                {
                    ModelState.AddModelError("", "Aktivacijska koda ni pravilna");
                }
                
               
               
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
