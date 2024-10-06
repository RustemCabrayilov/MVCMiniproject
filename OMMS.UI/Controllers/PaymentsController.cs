using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
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
		private readonly IGenericRepository<LoanDetail> _loanDetailRepository;
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IToastNotification _toastr;

		public PaymentsController(IGenericRepository<Payment> paymentRepository,
			IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<LoanDetail> loanDetailRepository,
			IToastNotification toastr)
		{
			_paymentRepository = paymentRepository;
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_userManager = userManager;
			_loanDetailRepository = loanDetailRepository;
			_toastr = toastr;
		}

		public async Task<IActionResult> Index()
		{
			List<PaymentVM> models = new();
			var payments = await _paymentRepository.GetAll();
			string userId = _userManager.GetUserId(User);
			var customers = await _customerRepository.GetAll();
			var customer = customers.FirstOrDefault(c => c.AppUserId == userId);
			if (customer==null)
			{
				_toastr.AddWarningToastMessage("You havent registered  as a customer");
				return RedirectToAction("Create","Customers");
			}
			var customerPayments = payments.Where(c => c.CustomerId == customer.Id);
			foreach (var item in customerPayments.ToList())
			{
				var loan = await _loanRepository.Get(item.LoanId);
				var loanDetails = await _loanDetailRepository.GetAll();
				var loanDetail = loanDetails.FirstOrDefault(c => c.LoanId ==loan.Id );
				models.Add(new PaymentVM
				{
					Id = item.Id,
					Amount = item.Amount,
					Date = item.PaymentDate,
					Loan = loan,
					Customer=customer,
					LoanDetail=loanDetail
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
			var loanDetails = await _loanDetailRepository.GetAll();
			var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanId == loan.Id);
			PaymentVM model = new()
			{
				Amount = loan.MonthlyPrice,
				LoanId = loan.Id,
				Loan = loan,
				CustomerId = customer.Id,
				CurrentAmount=loanDetail.CurrentAmount,
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
			var loanDetails = await _loanDetailRepository.GetAll();
			var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanId == loan.Id);
			loanDetail.LoanId = loan.Id;
			loanDetail.CurrentAmount -= model.Amount;
			await _paymentRepository.Create(payment);
			await _paymentRepository.SaveAsync();
			_loanDetailRepository.Update(loanDetail);
			_loanDetailRepository.Save();
			return RedirectToAction("Index", "Merchants");
		}
	}
}
