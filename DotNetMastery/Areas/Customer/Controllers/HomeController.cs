
using Bulky.DataAccess.Data;
using Bulky.Models;
using DotNetMastery.DataAccess.Repository;
using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DotNetMastery.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork) : Controller
    {

        public IActionResult Index()
        {
            IEnumerable<Product> productList = unitOfWork.Product.GetAll("Category");
            return View(productList);
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
