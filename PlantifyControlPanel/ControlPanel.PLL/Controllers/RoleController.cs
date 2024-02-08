using AutoMapper;
using ControlPanel.DAL.Models;
using ControlPanel.PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.PLL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public RoleController(RoleManager<IdentityRole> roleManager,IMapper mapper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                var mappedRole = roleManager.Roles.Select(r => new RoleViewModel()
                {
                  Id=r.Id,
                  RoleName = r.Name,
                }).ToList();
                return View(mappedRole);
            }
            else
            {
                var role = await roleManager.FindByNameAsync(name);
                if (role is not null)
                {

                    var mappedRole = new RoleViewModel()
                    {
                       Id= role.Id,
                       RoleName = role.Name
                    };
                    return View(new List<RoleViewModel>() { mappedRole });
                }

                return View(new List<RoleViewModel>() { });

            }


        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(RoleViewModel RoleVM)
        {

            if (ModelState.IsValid)
            {
                var MappedRole = mapper.Map<RoleViewModel, IdentityRole>(RoleVM);
                await roleManager.CreateAsync(MappedRole);


                return RedirectToAction(nameof(Index));
            }

            else
                return View(RoleVM);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id, string name = "Update")
        {
            if (id is null)
                return BadRequest();

            var role = await roleManager.FindByIdAsync(id);
            var mappedrole = mapper.Map<IdentityRole, RoleViewModel>(role);

            return View(name, mappedrole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] string id, RoleViewModel roleVM)
        {
            if (id != roleVM.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await roleManager.FindByIdAsync(id);
                    role.Name = roleVM.RoleName;
                    role.Id = roleVM.Id;


                    var result = await roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {

                        ModelState.AddModelError(string.Empty, "Failed Updated ! ");
                        return View(roleVM);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(roleVM);


        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            return await Update(id, "Delete");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, RoleViewModel roleVM)
        {

            if (id != roleVM.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(id);
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Faid To Update");

                }
            }

            return View(roleVM);
        }





    }
}
