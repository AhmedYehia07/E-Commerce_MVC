using Bulky.DataAccess.Data;
using Bulky.Models;
using DotNetMastery.DataAccess.Repository;
using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Utilty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DotNetMastery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var categoryList = unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order can't match the Name"); //custom validation error
            }
            if (ModelState.IsValid)  //validation error
            {
                unitOfWork.Category.Add(obj);
                unitOfWork.Save();
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
            var CategoryItem = unitOfWork.Category.Get(c => c.Id == id);
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
                unitOfWork.Category.Update(obj);
                unitOfWork.Save();
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
            var CategoryItem = unitOfWork.Category.Get(c => c.Id == id);
            if (CategoryItem == null)
            {
                return NotFound();
            }
            return View(CategoryItem);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteObj(int? id)
        {
            var CategoryItem = unitOfWork.Category.Get(c => c.Id == id);
            if (CategoryItem == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)  //validation error
            {
                unitOfWork.Category.Remove(CategoryItem);
                unitOfWork.Save();
                TempData["success"] = "Category Deleted Successfully";
                return RedirectToAction("Index", "Category");

            }
            return View();
        }
    }
}
