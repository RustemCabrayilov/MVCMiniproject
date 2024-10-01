using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Enums;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;
using System.Diagnostics;

namespace OMMS.UI.Area.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGenericRepository<Merchant> _merchantRepository;
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;

		public HomeController(ILogger<HomeController> logger,
			IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			IGenericRepository<Employee> employeeRepository,
			IGenericRepository<Merchant> merchantRepository)
		{
			_logger = logger;
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_employeeRepository = employeeRepository;
			_merchantRepository = merchantRepository;
		}

		public async Task<IActionResult> Index()
        {
            List<MerchantVM> models = new();
            var merchants=await _merchantRepository.GetAll();
            foreach (var merchant in merchants)
            {
                models.Add(new MerchantVM() {
                Id = merchant.Id,
                Name = merchant.Name,
                Description = merchant.Description,
                TerminalNo = merchant.TerminalNo,
                });
            }
            return View(models);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
