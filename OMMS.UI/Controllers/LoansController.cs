using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Enums;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Enums;
using OMMS.UI.Models;
using System.Linq;

namespace OMMS.UI.Controllers
{
	public class LoansController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly IGenericRepository<Product> _productRepository;

		public LoansController(IGenericRepository<Loan> loanRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<Customer> customerRepository,
			IGenericRepository<Product> productRepository,
			IGenericRepository<LoanItem> loanItemRepository)
		{
			_loanRepository = loanRepository;
			_userManager = userManager;
			_customerRepository = customerRepository;
			_productRepository = productRepository;
			_loanItemRepository = loanItemRepository;
		}

		public async Task<IActionResult> Index()
		{
			List<LoanVM> models = new();
			string userId = _userManager.GetUserId(User);
			var customers = await _customerRepository.GetAll();
			var customer = customers.ToList().FirstOrDefault(c => c.AppUserId == userId);
			var loans = await _loanRepository.GetAll();
			var pendingLoans = loans.Where(l => l.CustomerId == customer.Id&&l.Status==Status.Pending);
			var loanItems = await _loanItemRepository.GetAll();
			foreach (var loan in pendingLoans.ToList())
			{
				var loanItem = loanItems.FirstOrDefault(l => l.LoanId == loan.Id);
				models.Add(new()
				{
					Title = loan.Title,
					MonthlyPrice = loan.MonthlyPrice,
					TotalPrice = loan.TotalPrice,
					Terms = loan.Terms,
					Customer = customer,
					Status = loan.Status,
					Count=loanItem.Count,
					Price=loanItem.Price
				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create(int? productId)
		{
			var userId = _userManager.GetUserId(User);
			var customer = (await _customerRepository.GetAll()).FirstOrDefault(c => c.AppUserId == userId);
			var product = await _productRepository.Get(productId ?? 0);
			LoanVM model = new()
			{
				CustomerId = customer.Id,
				Product = product,
				ProductId = product.Id,
				TermList = new()
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
			var userId = _userManager.GetUserId(User);
			var product = await _productRepository.Get(model.ProductId);
			decimal monthlyPrice = (product.Price / model.Terms) * (decimal)1.05;
			decimal totalPrice = monthlyPrice * model.Terms;
			Loan loan = new()
			{
				Title = $"{product.Name}-{product.Model}-{product.Brand}-{model.Terms}months-{model.CustomerId}",
				Terms = model.Terms,
				CustomerId = model.CustomerId,
				TotalPrice = totalPrice,
				MonthlyPrice = monthlyPrice,
				Status = Status.Pending
			};
			await _loanRepository.Create(loan);
			await _loanRepository.SaveAsync();
			int count = model.Count;
			decimal price = count * totalPrice;
			LoanItem loanItem = new()
			{
				LoanId = loan.Id,
				ProductId = model.ProductId,
				Count = count,
				Price = price
			};
			await _loanItemRepository.Create(loanItem);
			await _loanItemRepository.SaveAsync();
			return RedirectToAction("Details", "Products", new { Id = product.Id });
		}
	}
}
