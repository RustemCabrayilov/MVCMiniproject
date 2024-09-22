using Microsoft.AspNetCore.Mvc;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using OMMS.UI.Models;

namespace OMMS.UI.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly IGenericRepository<Category> _categoryRepository;

        public CategoryViewComponent(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await GetItemsAsync();
            List<CategoryVM> models = new();
            foreach (var item in items)
            {

                var parentCategory =  items.Where(c=>c.Id==item.ParentId).ToList();
                models.Add(new()
                {
                    Name = item.Name,
                    Level = item.Level,
                    ParentId = item.ParentId,
                });
            }
            return View(models);
        }

        private async Task<List<Category>> GetItemsAsync()
        {
            var categories = await _categoryRepository.GetAll();
            return categories.ToList();
        }
    }
}
