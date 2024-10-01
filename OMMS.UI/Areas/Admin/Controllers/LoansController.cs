using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Entities;
using OMMS.DAL.Enums;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Enums;
using OMMS.UI.Models;

namespace OMMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin,Employee")]
	public class LoansController : Controller
	{
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly IGenericRepository<Employee> _employeeRepository;
		private readonly IGenericRepository<Product> _productRepository;
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly UserManager<AppUser> _userManager;

		public LoansController(IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			IGenericRepository<Employee> employeeRepository,
			IGenericRepository<Product> productRepository,
			IGenericRepository<LoanItem> loanItemRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<Branch> branchRepository)
		{
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_employeeRepository = employeeRepository;
			_productRepository = productRepository;
			_loanItemRepository = loanItemRepository;
			_userManager = userManager;
			_branchRepository = branchRepository;
		}

		public async Task<IActionResult> Index()
		{
			List<LoanVM> models = new();
			var loans = (await _loanRepository.GetAll()).ToList();

			foreach (var loan in loans)
			{
				var customer = await _customerRepository.Get(loan.CustomerId);
				var employee = await _employeeRepository.Get(loan.EmployeeId ?? 0);
				models.Add(new()
				{
					Id = loan.Id,
					Title = loan.Title,
					MonthlyPrice = loan.MonthlyPrice,
					TotalPrice = loan.TotalPrice,
					Terms = loan.Terms,
					Customer = customer,
					Employee = employee,
					Status = loan.Status,
				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create()
		{
			var customers = (await _customerRepository.GetAll()).ToList();
			var employees = (await _employeeRepository.GetAll()).ToList();
			var products = (await _productRepository.GetAll()).ToList();
			LoanVM model = new()
			{
				Customers = customers,
				Employees = employees,
				Products = products,
				TermList = new(),

			};
			foreach (var item in Enum.GetValues(typeof(Terms)))
			{
				int term = (int)item;
				model.TermList.Add(term);
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(LoanVM model)
		{
			var product = await _productRepository.Get(model.ProductId);
			decimal monthlyPrice = (product.Price / model.Terms) * (decimal)1.05;
			decimal totalPrice = monthlyPrice * model.Terms;
			Loan loan = new()
			{
				Title = model.Title,
				MonthlyPrice = monthlyPrice,
				TotalPrice = totalPrice,
				Terms = model.Terms,
				EmployeeId = model.EmployeeId,
				CustomerId = model.CustomerId,
				Status = Status.Accept,
			};
			int count = model.Count;
			decimal price = count * totalPrice;
			LoanItem loanItem = new()
			{
				LoanId = loan.Id,
				ProductId = model.ProductId,
				Count = count,
				Price = price
			};
			await _loanRepository.Create(loan);
			await _loanRepository.SaveAsync();
			await _loanItemRepository.Create(loanItem);
			await _loanItemRepository.SaveAsync();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Edit(int Id)
		{
			var loan = await _loanRepository.Get(Id);
			var loanItems = await _loanItemRepository.GetAll();
			var loanItem = loanItems.FirstOrDefault(l => l.LoanId == Id);
			var customers = (await _customerRepository.GetAll()).ToList();
			var employees = (await _employeeRepository.GetAll()).ToList();
			var products = (await _productRepository.GetAll()).ToList();
			LoanVM model = new()
			{
				Title = loan.Title,
				CustomerId = loan.CustomerId,
				Customers = customers,
				EmployeeId = loan.EmployeeId,
				Employees = employees,
				ProductId = loanItem.ProductId,
				Products = products,
				Terms = loan.Terms,
				TermList = new(),
				Statuses = new()
			};
			foreach (var item in Enum.GetValues(typeof(Terms)))
			{
				int term = (int)item;
				model.TermList.Add(term);
			}
			foreach (var status in Enum.GetValues(typeof(Status)))
			{
				model.Statuses.Add(status.ToString());
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(LoanVM model, int id, string assess)
		{
			var loan = await _loanRepository.Get(model.Id != 0 ? model.Id : id);
			var product = await _productRepository.Get(model.ProductId);
			decimal monthlyPrice = (product.Price / model.Terms) * (decimal)1.05;
			decimal totalPrice = monthlyPrice * model.Terms;
			loan.Title = model.Title;
			loan.CustomerId = model.CustomerId;
			loan.EmployeeId = model.EmployeeId;
			loan.Terms = model.Terms;
			loan.TotalPrice = totalPrice;
			loan.MonthlyPrice = monthlyPrice;
			loan.Status = model.Status;
			if (assess == "Accept")
			{
				loan.Status = Status.Accept;
			}
			else if (assess == "Reject")
			{
				loan.Status = Status.Reject;
			}
			_loanRepository.Update(loan);
			_loanRepository.Save();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Delete(int Id)
		{
			var loan = await _loanRepository.Get(Id);
			var customer = await _customerRepository.Get(loan.CustomerId);
			var employee = await _employeeRepository.Get(loan.EmployeeId ?? 0);
			/*		var product=(await _loanItemRepository.GetAll()).Where(l=>l.LoanId==Id);*/
			LoanVM model = new()
			{
				Id = Id,
				Title = loan.Title,
				Customer = customer,
				Employee = employee,
				Terms = loan.Terms,
				MonthlyPrice = loan.MonthlyPrice,
				TotalPrice = loan.TotalPrice,
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Delete(LoanVM model)
		{
			var loan = await _loanRepository.Get(model.Id);
			_loanRepository.Remove(loan.Id);
			_loanRepository.Save();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Assess(int? loanId)
		{
			var loan = await _loanRepository.Get(loanId ?? 0);
			var customer = await _customerRepository.Get(loan.CustomerId);
			string userId = _userManager.GetUserId(User);
			var employees = await _employeeRepository.GetAll();
			var employee = employees.ToList().FirstOrDefault(e => e.AppUserId == userId);
			var loanItem = await _loanItemRepository.GetAll();
			int productId = loanItem.FirstOrDefault(p => p.LoanId == loanId).ProductId;
			var product = await _productRepository.Get(productId, "Branch");
			LoanVM model = new()
			{
				Id = loan.Id,
				Title = loan.Title,
				Customer = customer,
				CustomerId = customer.Id,
				Product = product,
				ProductId = productId,
				EmployeeId = employee.Id,
				Terms = loan.Terms,
				MonthlyPrice = loan.MonthlyPrice,
				TotalPrice = loan.TotalPrice,
				Status = loan.Status,
			};
			return View(model);
		}
	}
}
