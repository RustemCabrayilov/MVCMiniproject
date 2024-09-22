using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductsController : Controller
	{
		private readonly IGenericRepository<Product> _productRepository;
		private readonly IGenericRepository<Category> _categoryRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductsController(IGenericRepository<Product> productRepository,
			IGenericRepository<Category> categoryRepository,
			IWebHostEnvironment webHostEnvironment)
		{
			_productRepository = productRepository;
			_categoryRepository = categoryRepository;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{
			List<ProductVM> models = new();
			var products = await _productRepository.GetAll();
			foreach (var product in products)
			{
				var category = await _categoryRepository.Get(product.Id);
				models.Add(new()
				{
					Id = product.Id,
					Name = product.Name,
					CategoryId = product.CategoryId,
					CategoryName = category.Name,
					Description = product.Description,
					Count = product.Count,
					Price = product.Price,
					Brand = product.Brand,
					Model = product.Model,
					Thumbnail= product.Thumbnail,
				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create()
		{
			var categories = _categoryRepository.GetAll().Result.ToList();
			ProductVM model = new()
			{
				Categories = categories,
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(ProductVM model)
		{
			List<ProductImage> productImages = new();
            foreach (var imageFile in model.ImageFiles)
            {
				productImages = UploadImage(imageFile);
            }
            Product product = new()
			{
				Id = model.Id,
				Name = model.Name,
				CategoryId = model.CategoryId,
				Description = model.Description,
				Count = model.Count,
				Price = model.Price,
				Brand = model.Brand,
				Model = model.Model,
				
			};
			product.ProductImages= productImages;
			product.Thumbnail = Path.Combine(productImages[0].Directory, productImages[0].Name);

			await _productRepository.Create(product);
			await _productRepository.SaveAsync();
			return RedirectToAction("Index");

		}
		public List<ProductImage> UploadImage(IFormFile file)
		{
			List<ProductImage> productImages = new();
			string localPath = _webHostEnvironment.WebRootPath;
			string folderName = "productImages";
			string fileName =Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
			string filePath=Path.Combine(localPath,folderName,fileName);
			using (var localFile = System.IO.File.OpenWrite(filePath))
			using (var uploadedFile = file.OpenReadStream())
			{
				uploadedFile.CopyTo(localFile);
			}
			productImages.Add(new()
			{
				Name=fileName,
				Directory=folderName
			});
			return productImages;
		}
	}
}
