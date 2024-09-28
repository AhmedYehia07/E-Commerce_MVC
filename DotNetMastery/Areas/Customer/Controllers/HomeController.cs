
using Bulky.DataAccess.Data;
using Bulky.Models;
using DotNetMastery.DataAccess.Repository;
using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace DotNetMastery.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork) : Controller
    {

        public IActionResult Index()
        {
            IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(productList);
        }

		public IActionResult Details(int productId)
		{
            ShoppingCart cart = new ShoppingCart
            {
                Product = unitOfWork.Product.Get(u => u.Id == productId, "Category"),
                Count = 1,
                ProductId = productId
            };
			return View(cart);
		}

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;
            var product = unitOfWork.ShoppingCart.Get(u => u.ProductId == cart.ProductId && u.ApplicationUserId == cart.ApplicationUserId);
            if(product == null)
            {
                unitOfWork.ShoppingCart.Add(cart);
                unitOfWork.Save();
                TempData["success"] = "Item Added Successfully";
            }
            else
            {
				product.Count += cart.Count;
				unitOfWork.ShoppingCart.Update(product);
                unitOfWork.Save();
                TempData["success"] = "Item Added Successfully";
            }
            return RedirectToAction(nameof(Index));
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
