using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class EmployeesController : Controller
	{

		private readonly IGenericRepository<Employee> _employeeRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly UserManager<AppUser> _userManager;
		public EmployeesController(IGenericRepository<Employee> employeeRepository, UserManager<AppUser> userManager, IGenericRepository<Branch> branchRepository)
		{
			_employeeRepository = employeeRepository;
			_userManager = userManager;
			_branchRepository = branchRepository;
		}

		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Create()
		{
			string userId = _userManager.GetUserId(User);
			var branchs = await _branchRepository.GetAll();
			EmployeeVM model = new()
			{
				AppUserId = userId,
				Branchs = branchs.ToList()
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(EmployeeVM model)
		{
			Employee employee = new()
			{
				Name = model.Name,
				Surname = model.Surname,
				Position = model.Position,
				BranchId = model.BranchId,
				AppUserId = model.AppUserId,
			};
			await _employeeRepository.Create(employee);
			await _employeeRepository.SaveAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
