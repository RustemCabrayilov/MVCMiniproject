using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.UI.Models;

namespace OMMS.UI.Areas.Admin.Controllers
{
	[Area("admin")]
	[Authorize(Roles = "Admin,Employee,Merchant,Branch")]
	public class AccountController : Controller
	{
		private readonly SignInManager<AppUser> _signInmanagerManager;
		private readonly UserManager<AppUser> _userManager;

		public AccountController(SignInManager<AppUser> signInmanagerManager, UserManager<AppUser> userManager)
		{
			_signInmanagerManager = signInmanagerManager;
			_userManager = userManager;
		}

		public IActionResult LogIn()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> LogIn(LogInVM model)
		{
	
			if (ModelState.IsValid)
			{
				var hasUser = await _userManager.FindByEmailAsync(model.Email);
				if (hasUser != null)
				{
					var signInResult = await _signInmanagerManager.PasswordSignInAsync(hasUser, model.Password, false, false);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Email or Password wrong");
				}
			}
			return View(model);
		}
	}
}
