using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Enums;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.ViewComponents
{
	public class LoanModalViewComponent:ViewComponent
	{
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<LoanDetail> _loanDetailRepository;


		public LoanModalViewComponent(IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			IGenericRepository<Employee> employeeRepository,
			IGenericRepository<LoanDetail> loanDetailRepository)
		{
			_loanRepository = loanRepository;
			_loanDetailRepository = loanDetailRepository;
		}

		public async Task<IViewComponentResult> InvokeAsync(int Id)
		{
			var loanDetail = await GetItemAsync(Id);
			var loan =await _loanRepository.Get(Id);
			LoanDetailVM model = new()
			{
				Loan = loanDetail.Loan,
				CurrentAmount = loanDetail.CurrentAmount
			};

			return View(model);
		}

		private async Task<LoanDetail> GetItemAsync(int Id)
		{
			var loan = await _loanRepository.Get(Id);
			var loanDetails = await _loanDetailRepository.GetAll();
			var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanId == loan.Id);
			
			return loanDetail;
		}
	}
}
