using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Entities;
using OMMS.UI.Models;

namespace OMMS.UI.Area.Admin
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            List<RoleVM> model = new();
            var roles = _roleManager.Roles.ToList();
            foreach (var role in roles)
            {
                model.Add(new RoleVM
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleVM model)
        {
            AppRole appRole = new()
            {
                Name = model.Name,
            };
           var result = await _roleManager.CreateAsync(appRole);
            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
