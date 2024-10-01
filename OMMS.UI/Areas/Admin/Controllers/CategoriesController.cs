using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles ="Admin,Employee")]
	public class CategoriesController : Controller
	{
		private readonly IGenericRepository<Category> _categoryRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly IGenericRepository<Employee> _employeeRepository;

		public CategoriesController(IGenericRepository<Branch> branchRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<Employee> employeeRepository,
			IGenericRepository<Category> categoryRepository)
		{
			_branchRepository = branchRepository;
			_employeeRepository = employeeRepository;
			_categoryRepository = categoryRepository;
		}

		public async Task<IActionResult> Index()
		{
			var categories = (await _categoryRepository.GetAll()).ToList();
			List<CategoryVM> models = new();
			foreach (var model in categories)
			{
				var branch = await _branchRepository.Get(model.BranchId);
				var employee = await _employeeRepository.Get(model.EmployeeId);
				var parentCategory = await _categoryRepository.Get(model.ParentId);
				models.Add(new()
				{
					Id = model.Id,
					Name = model.Name,
					BranchName = branch.Name,
					EmployeeName = employee.Name,
					Level = model.Level,
					ParentCategory = model.ParentId!=0?parentCategory.Name:"No parent category",

				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create()
		{
			var branchs = await _branchRepository.GetAll();
			var categories = await _categoryRepository.GetAll();
			var employees = await _employeeRepository.GetAll();
			CategoryVM model = new()
			{
				Branchs = branchs.ToList(),
				Employees = employees.ToList(),
				Categories = categories.ToList(),
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(CategoryVM model)
		{
			Category category = new()
			{
				Name = model.Name,
				Level = model.Level,
				ParentId = model.ParentId,
				BranchId = model.BranchId,
				EmployeeId = model.EmployeeId,
			};
			await _categoryRepository.Create(category);
			await _categoryRepository.SaveAsync();
			return RedirectToAction("Index");
		}
	}
}
