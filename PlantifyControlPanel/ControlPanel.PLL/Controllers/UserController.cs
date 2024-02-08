using AutoMapper;
using ControlPanel.DAL.Models;
using ControlPanel.PLL.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ControlPanel.PLL.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<ApplicationUser> userManager ,SignInManager<ApplicationUser> signInManager,IMapper mapper,RoleManager<IdentityRole>roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string username)
       {

            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            
            if (string.IsNullOrEmpty(username))
            {
                var mappedUsers =  userManager.Users.Select( u => new UserViewModel()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Roles = userManager.GetRolesAsync(u).Result
                }).ToList();
                return View(mappedUsers);
            }
            else
            {
                var user=await userManager.FindByNameAsync(username);
                if (user is not null)
                {

                    var mappeduser = new UserViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = userManager.GetRolesAsync(user).Result
                    };
                    return View(new List<UserViewModel>() { mappeduser });
                }

                return View(new List<UserViewModel>() {  });

            }


            
            

        }

        [HttpGet]
        public async  Task<IActionResult> Update(string id,string name="Update")
        {
            if (id is null)
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);
           
            var mappeduser = mapper.Map<ApplicationUser, UserViewModel>(user);
            mappeduser.AllRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            mappeduser.Roles = await userManager.GetRolesAsync(user);
            
            return View(name,mappeduser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] string id ,UserViewModel userVM)
        {
            if (id != userVM.Id)
                return BadRequest();
            userVM.AllRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userManager.FindByIdAsync(id);
                    user.UserName = userVM.UserName;
                    user.Email = userVM.Email;
                    user.SecurityStamp = Guid.NewGuid().ToString();


                    // Clear existing roles
                    var userRoles = await userManager.GetRolesAsync(user);
                    await userManager.RemoveFromRolesAsync(user, userRoles);

                    // Add the selected role
                    await userManager.AddToRoleAsync(user, string.Join(",",userVM.Roles));



                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                       
                        ModelState.AddModelError(string.Empty,"Failed Updated ! please reverse your data if its match already exists if falid!");
                        return View(userVM);
                    }
                }catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(userVM);


        }
        [HttpGet]
        public async Task< IActionResult> Delete(string id)
        {
            return await Update(id,"Delete");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Delete(string id, UserViewModel userVM)
        {

            if(id !=userVM.Id)
                return BadRequest();
            if(ModelState.IsValid)
            {
                var user=await userManager.FindByIdAsync(id);
               var  result= await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Faid To Update");

                }
            }

            return View(userVM);
        }

        private List<SelectListItem> GetRolesList()
        {
            var roles = roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name })
                .ToList();

            return roles;
        }

    }
}
