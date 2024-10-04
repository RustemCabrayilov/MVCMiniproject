using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Enums;
using OMMS.UI.Models;

namespace OMMS.UI.Controllers
{
	public class PaymentsController : Controller
	{
		private readonly IGenericRepository<Payment> _paymentRepository;
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly UserManager<AppUser> _userManager;

		public PaymentsController(IGenericRepository<Payment> paymentRepository,
			IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			UserManager<AppUser> userManager
		)
		{
			_paymentRepository = paymentRepository;
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_userManager = userManager;

		}

		public async Task<IActionResult> Index()
		{
			List<PaymentVM> models = new();
			var payments = await _paymentRepository.GetAll();
			string userId = _userManager.GetUserId(User);
			var customers = await _customerRepository.GetAll();
			var customer = customers.FirstOrDefault(c => c.AppUserId == userId);
			var customerPayments = payments.Where(c => c.CustomerId == customer.Id);
			foreach (var item in customerPayments.ToList())
			{
				var loan = await _loanRepository.Get(item.LoanId);
				models.Add(new PaymentVM
				{
					Id = item.Id,
					Amount = item.Amount,
					Date = item.PaymentDate,
					Loan = loan,
				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create(string userId, int loanId)
		{
			var customers = await _customerRepository.GetAll();
			var customer = customers.FirstOrDefault(c => c.AppUserId == userId);
			var loan = await _loanRepository.Get(loanId);
			List<string> paymentTypes = new();
			PaymentVM model = new()
			{
				Amount = loan.MonthlyPrice,
				LoanId = loan.Id,
				CustomerId = customer.Id,
				TotalPrice = loan.TotalPrice,
				PaymentTypes = paymentTypes
			};
			foreach (var item in Enum.GetValues(typeof(PaymentType)))
			{
				model.PaymentTypes.Add(item.ToString());
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(PaymentVM model)
		{

			Payment payment = new()
			{
				Amount = model.Amount,
				PaymentDate = DateTime.Now,
				PaymentType = model.PaymentType,
				LoanId = model.LoanId,
				CustomerId = model.CustomerId,
			};
			var loan = await _loanRepository.Get(model.LoanId);
			loan.TotalPrice -= model.Amount;
			await _paymentRepository.Create(payment);
			await _paymentRepository.SaveAsync();
			_loanRepository.Update(loan);
			_paymentRepository.Save();
			return RedirectToAction("Index", "Merchants");
		}
	}
}
