using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IGenericRepository<Payment> _paymentRepository;

		public PaymentController(IGenericRepository<Payment> paymentRepository)
		{
			_paymentRepository = paymentRepository;
		}

		public async Task<IActionResult> Create(PaymentVM model)
		{
			if (ModelState.IsValid)
			{
				Payment payment = new()
				{
					
				};
				await _paymentRepository.Create(payment);
				await _paymentRepository.SaveAsync();
			}
			return Ok();
		}
	}
}
