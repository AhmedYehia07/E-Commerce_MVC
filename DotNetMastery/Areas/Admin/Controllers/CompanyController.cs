using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models.ViewModels;
using DotNetMastery.Models;
using DotNetMastery.Utilty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DotNetMastery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController(IUnitOfWork unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var compenylist = unitOfWork.Company.GetAll().ToList();
            return View(compenylist);
        }
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //update
                Company company = unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0)
                {
                    unitOfWork.Company.Add(CompanyObj);
                    TempData["success"] = $"Company Created Successfully";
                }
                else
                {
                    unitOfWork.Company.update(CompanyObj);
                    TempData["success"] = $"Company Updated Successfully";
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
            var company = unitOfWork.Company.Get(x => x.Id == id);
            return View(company);
        }

        [HttpPost]
        public IActionResult Delete(Company company)
        {
            unitOfWork.Company.Remove(company);
            unitOfWork.Save();
            TempData["success"] = "Company Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
