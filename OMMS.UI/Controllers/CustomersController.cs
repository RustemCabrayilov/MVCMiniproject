using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Controllers
{
	public class CustomersController : Controller
	{
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly UserManager<AppUser> _userManager;

		public CustomersController(IGenericRepository<Customer> customerRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<Loan> loanRepository,
			IGenericRepository<LoanItem> loanItemRepository)
		{
			_customerRepository = customerRepository;
			_userManager = userManager;
			_loanRepository = loanRepository;
			_loanItemRepository = loanItemRepository;
		}
		public async Task<IActionResult> Details()
		{
			string userId = _userManager.GetUserId(User); 
			var user =await _userManager.FindByIdAsync(userId);
			var customer = (await _customerRepository.GetAll()).FirstOrDefault(c=>c.AppUserId==userId);
			var loans= (await _loanRepository.GetAll()).Where(l=>l.CustomerId==customer.Id).ToList();
			List<LoanItemVM> LoanItemModels = new();
            foreach (var loan in loans)
			{
				var loanItems=(await _loanItemRepository.GetAll()).Where(l=>l.LoanId==loan.Id).ToList();
                foreach (var loanItem in loanItems)
                {
                LoanItemModels.Add(new() {
				LoanId=loan.Id,
				Loan=loan,
				Count=loanItem.Count,
				Price=loanItem.Price,
				ProductId=loanItem.ProductId,
				});
                    
                }
            }
            CustomerVM model = new()
			{
				Id = customer.Id,
				Name = customer.Name,
				Surname = customer.Surname,
				Address = customer.Address,
				Occupation = customer.Occupation,
				UserName = user.UserName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				LoanItems=LoanItemModels
			};
			return View(model);
		}
		public async Task<IActionResult> Edit(int Id)
		{
			string userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId);
			var customer = await _customerRepository.Get(Id);
			CustomerVM model = new()
			{
				Id = customer.Id,
				Name = customer.Name,
				Surname = customer.Surname,
				Address = customer.Address,
				Occupation = customer.Occupation,
				UserName = user.UserName,
				UserId = userId
				
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(CustomerVM model,int id)
		{
			string userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId);
			user.UserName=model.UserName;
			Customer customer = new()
			{
				Id = model.Id,
				Name = model.Name,
				Surname = model.Surname,
				Address = model.Address,
				Occupation = model.Occupation,
				AppUserId = model.UserId
			};
			await _userManager.UpdateAsync(user);
		   _customerRepository.Update(customer);
			_customerRepository.Save();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Create()
		{
			var userId =  _userManager.GetUserId(User);
			CustomerVM customer = new()
			{
				UserId=userId,
			};
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CustomerVM model)
		{
			Customer customer = new()
			{
				Name = model.Name,
				Surname = model.Surname,
				Occupation = model.Occupation,
				Address = model.Address,
				AppUserId = model.UserId,
			};
			await _customerRepository.Create(customer);
			await _customerRepository.SaveAsync();
			var referer = Request.Headers["Referer"].ToString();
			return Redirect(referer);
		}
	}
}
