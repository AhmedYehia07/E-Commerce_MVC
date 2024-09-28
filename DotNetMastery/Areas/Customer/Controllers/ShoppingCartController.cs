using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using DotNetMastery.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNetMastery.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController(IUnitOfWork unitOfWork) : Controller
    {
        public ShoppingCartVm ShoppingCartVm { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVm = new()
            {
                ShoppingCartList = unitOfWork.ShoppingCart.GetAll(u=> u.ApplicationUserId == userId,includeProperties:"Product"),
            };
            foreach(var cart in ShoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVm.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVm);
        }
        public IActionResult Plus(int cartId)
        {
            var cart = unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cart.Count++;
            unitOfWork.ShoppingCart.Update(cart);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
		public IActionResult Minus(int cartId)
		{
			var cart = unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if(cart.Count <= 1)
            {
				unitOfWork.ShoppingCart.Remove(cart);
			}
            else
            {
				cart.Count--;
				unitOfWork.ShoppingCart.Update(cart);
			}
			unitOfWork.Save();
			return RedirectToAction("Index");
		}
        public IActionResult Summary(int cartId)
        {
            return View();
        }

		public IActionResult Remove(int cartId)
		{
			var cart = unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			unitOfWork.ShoppingCart.Remove(cart);
			unitOfWork.Save();
			return RedirectToAction("Index");
		}
		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
                return shoppingCart.Product.Price;
            else if (shoppingCart.Count <= 100)
                return shoppingCart.Product.Price50;
            else
                return shoppingCart.Product.Price100;

        }
    }
}
