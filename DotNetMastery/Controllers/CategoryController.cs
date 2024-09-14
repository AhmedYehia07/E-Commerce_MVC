using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetMastery.Controllers
{
    public class CategoryController(ApplicationDbContext dbContext) : Controller
    {
        public IActionResult Index()
        {
            var categoryList = dbContext.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order can't match the Name"); //custom validation error
            }
            if (ModelState.IsValid)  //validation error
            {
                dbContext.Categories.Add(obj);
                dbContext.SaveChanges();
				TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");

            }     
            return View();
        }

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			//var Category = dbContext.Categories.Find(id);
			//var Category = dbContext.Categories.Where(c=>c.Id == id).FirstOrDefault();
			var CategoryItem = dbContext.Categories.FirstOrDefault(c => c.Id == id);
			if (CategoryItem == null)
			{
				return NotFound();
			}
			return View(CategoryItem);
		}
        [HttpPost]
		public IActionResult Edit(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("Name", "Display Order can't match the Name"); //custom validation error
			}
			if (ModelState.IsValid)  //validation error
			{
				dbContext.Categories.Update(obj);
				dbContext.SaveChanges();
				TempData["success"] = "Category Updated Successfully";
				return RedirectToAction("Index", "Category");

			}
			return View();
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			//var Category = dbContext.Categories.Find(id);
			//var Category = dbContext.Categories.Where(c=>c.Id == id).FirstOrDefault();
			var CategoryItem = dbContext.Categories.FirstOrDefault(c => c.Id == id);
			if (CategoryItem == null)
			{
				return NotFound();
			}
			return View(CategoryItem);
		}
		[HttpPost,ActionName("Delete")]
		public IActionResult DeleteObj(int? id)
		{
			var CategoryItem = dbContext.Categories.FirstOrDefault(c => c.Id == id);
			if (CategoryItem == null)
			{
				return NotFound();
			}
			if (ModelState.IsValid)  //validation error
			{
				dbContext.Categories.Remove(CategoryItem);
				dbContext.SaveChanges();
				TempData["success"] = "Category Deleted Successfully";
				return RedirectToAction("Index", "Category");

			}
			return View();
		}
	}
}
