using AutoMapper;
using ControlPanel.DAL.Models;
using ControlPanel.PLL.Helper;
using ControlPanel.PLL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Security.Policy;


namespace ControlPanel.PLL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
		private readonly IEmailSetting emailSetting;

		public AccountController(IMapper mapper,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser>signInManager ,RoleManager<IdentityRole> roleManager,IEmailSetting emailSetting)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
			this.emailSetting = emailSetting;
		}



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var user=await userManager.FindByEmailAsync(model.Email);

                if(user is not null)
                {
                    bool flag = await userManager.CheckPasswordAsync(user, model.Password);

                    if (flag)
                    {
                      var result=  await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                        if (result.Succeeded)
                        {
                            HttpContext.Session.SetString("UserName", user.UserName);
                            return RedirectToAction("Index", "User");
                        }
                    }
                    ModelState.AddModelError(string.Empty, "The Email Or Password is invalid ");
                  
                }
                ModelState.AddModelError(string.Empty, "The Email Or Password is invalid ");

            }
            return View(model);
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var check = await userManager.FindByEmailAsync(model.Email);
                if (check is null)
                {
                    var user = mapper.Map<RegisterViewModel, ApplicationUser>(model);


                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                       await userManager.AddToRoleAsync(user, "User");
                        return RedirectToAction(nameof(Login));
                    }
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                ModelState.AddModelError(String.Empty, "The Email is already taken");
            }
            return View(model);
        }


        public async Task<IActionResult> Signout()
        {
            HttpContext.Session.Remove("UserName");
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }



        public IActionResult ForgetPassword() {
        return View();
        }


        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user =await  userManager.FindByEmailAsync(model.Email);
                
                if(user is not null)
                {
					var token = await userManager.GeneratePasswordResetTokenAsync(user);
					var ResetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, Token = token }, Request.Scheme);

					var email = new Email()
                    {
                        To = model.Email,
                        Subject = "Reset Password",
                        Body = ResetLink
                    };
                   await emailSetting.SendEmail(email);
                    return RedirectToAction(nameof(CheckInbox));
                }
                ModelState.AddModelError(string.Empty,"The Email Is not Exist");
               
            }
           
            return View("ForgetPassword",model);

        }
        [HttpGet]
        public IActionResult CheckInbox()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email,string token)
        {
            TempData["Email"] = email;
            TempData["Token"] = token;
            return View();
        }

        [HttpPost]
		public async  Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email= TempData["Email"]as string;
                var token = TempData["Token"] as string;
                var user=await userManager.FindByEmailAsync(email);
                var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                foreach(var error in result.Errors)
                    ModelState.AddModelError(string.Empty,error.Description);
			}
            return View(model);

        }
        [HttpGet]
       public async Task GoogleLogin()
		{
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse))
            });
		}




        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            // Ensure the authentication was successful
            if (result.Succeeded)
            {
                var emailClaim = result.Principal.FindFirst(ClaimTypes.Email);

                // Check if the user has a valid email claim
                if (emailClaim != null)
                {
                    // Get the email value
                    var email = emailClaim.Value;

                    // Extract the username from the email (example: assuming email is in the form "username@example.com")
                    var userName = email.Split('@')[0];

                    // Store the username in the session
                    HttpContext.Session.SetString("UserName", userName);
                }
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
