using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using DotNetMastery.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotNetMastery.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost) : Controller
    {
        public IActionResult Index()
        {
            var productList = unitOfWork.Product.GetAll("Category").ToList();
            return View(productList);
        }

        //[HttpPost,ActionName("Index")]
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var product = unitOfWork.Product.Get(x => x.Id == id);
        //    if (!string.IsNullOrEmpty(product.ImgURL)) //check if there is an old image to delete it
        //    {
        //        string wwwRootPath = webHost.WebRootPath;
        //        var oldImagePath = Path.Combine(wwwRootPath, (product.ImgURL).TrimStart('\\'));
        //        if (System.IO.File.Exists(oldImagePath))
        //        {
        //            System.IO.File.Delete(oldImagePath);
        //        }
        //    }
        //    unitOfWork.Product.Remove(product);
        //    unitOfWork.Save();
        //    TempData["success"] = "Product Deleted Successfully";
        //    return RedirectToAction("Index");
        //}

        public IActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            var productVM = new ProductVM
            {
                Product = new Product(),
                CategoryList = unitOfWork.Category.GetAll().Select(u=> new SelectListItem
                {
                    Text = u.Name,
					Value=u.Id.ToString()
				})
            };
            if(id == null || id== 0)
            {
				//Create
				return View(productVM);
			}
			else
            {
                //update
                productVM.Product = unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
		public IActionResult Upsert(ProductVM productVm, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = webHost.WebRootPath; //give the path of the root folder (wwwroot)
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //Give a random name to the file + file extention
                    string productPath = Path.Combine(wwwRootPath, @"images\Product"); //Give the path of product images
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    if(!string.IsNullOrEmpty(productVm.Product.ImgURL)) //check if there is an old image to delete it
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, (productVm.Product.ImgURL).TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    productVm.Product.ImgURL = @"\images\Product\" + fileName;
                }
                if(productVm.Product.Id == 0)
                {
					unitOfWork.Product.Add(productVm.Product);
					TempData["success"] = $"Product Created Successfully";
				}
                else
                {
                    unitOfWork.Product.Update(productVm.Product);
					TempData["success"] = $"Product Updated Successfully";
				}
                unitOfWork.Save();
				return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = unitOfWork.Product.Get(x => x.Id == id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {
            if (!string.IsNullOrEmpty(product.ImgURL)) //check if there is an old image to delete it
            {
                string wwwRootPath = webHost.WebRootPath;
                var oldImagePath = Path.Combine(wwwRootPath, (product.ImgURL).TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            unitOfWork.Product.Remove(product);
            unitOfWork.Save();
            TempData["success"] = "Product Deleted Successfully";
            return RedirectToAction("Index");
        }

    }
}
