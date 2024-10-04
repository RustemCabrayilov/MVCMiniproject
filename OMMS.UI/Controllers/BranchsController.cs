using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Controllers
{
	[Authorize(Roles ="Merchant,Admin")]
	public class BranchsController : Controller
	{
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly IGenericRepository<Merchant> _merchantRepository;
		private readonly UserManager<AppUser> _userManager;

		public BranchsController(IGenericRepository<Branch> branchRepository,
			IGenericRepository<Merchant> merchantRepository,
			UserManager<AppUser> userManager)
		{
			_branchRepository = branchRepository;
			_merchantRepository = merchantRepository;
			_userManager = userManager;
		}

		public IActionResult Index()
		{

			return View();
		}
		public async Task<IActionResult> Create()
		{
			string userId = _userManager.GetUserId(User);
			var merchants = await _merchantRepository.GetAll();
			BranchVM model = new()
			{
				AppUserId = userId,
				Merchants = merchants.ToList()
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(BranchVM model)
		{

			string userId = _userManager.GetUserId(User);
			Branch branch = new()
			{
				Name = model.Name,
				Description = model.Description,
				Address = model.Address,
				MerchantId = model.MerchantId,
				AppUserId = model.AppUserId,

			};
			await _branchRepository.Create(branch);
			await _branchRepository.SaveAsync();
			return RedirectToAction("Index", "Home");

		}
	}
}
