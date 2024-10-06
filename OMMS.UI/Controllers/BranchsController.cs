using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Controllers
{
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

		public async Task<IActionResult> Index(int? merchantId)
		{
			var branchs= await _branchRepository.GetAll();
			var branchList =branchs.Where(b=>b.MerchantId==merchantId);
			List<BranchVM> models = new();
            foreach (var item in branchList.ToList())
            {
				models.Add(new()
				{
					Id = item.Id,
					Name= item.Name,
				});
            }
            return View(models);
		}

        [Authorize(Roles = "Merchant,Admin")]

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
