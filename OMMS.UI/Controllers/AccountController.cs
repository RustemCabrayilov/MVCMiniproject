using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.UI.Models;

namespace OMMS.UI.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
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
				if (hasUser == null)
				{
					ModelState.AddModelError("", "Email or password wrong");
				}
				else
				{
				
					var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.Persistent, false);
					if (signInResult.Succeeded)
					{

						return RedirectToAction("Index", "Merchants");
					}
				}
			}

			return View(model);
		}
		public IActionResult SignUp()
		{

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignUp(SignUpVM model)
		{
			if (ModelState.IsValid)
			{
				AppUser user = new()
				{
					UserName = model.UserName,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction("LogIn", "Account");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
				}
			}
			return View(model);
		}
	}
}
