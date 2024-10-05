using DotNetMastery.DataAccess.Repository.IRepository;
using DotNetMastery.Models;
using DotNetMastery.Models.ViewModels;
using DotNetMastery.Utilty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace DotNetMastery.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController(IUnitOfWork unitOfWork) : Controller
    {
        [BindProperty]
        public ShoppingCartVm ShoppingCartVm { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVm = new()
            {
                ShoppingCartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach(var cart in ShoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVm.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVm);
        }
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVm = new()
			{
				ShoppingCartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};

            ShoppingCartVm.OrderHeader.User = unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.User.Name;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.User.PhoneNumber;
            ShoppingCartVm.OrderHeader.StreetAddress = ShoppingCartVm.OrderHeader.User.StreetAddress;
            ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.User.City;
            ShoppingCartVm.OrderHeader.State = ShoppingCartVm.OrderHeader.User.State;
            ShoppingCartVm.OrderHeader.PostalCode = ShoppingCartVm.OrderHeader.User.PostalCode;

			foreach (var cart in ShoppingCartVm.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVm.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}
			return View(ShoppingCartVm);
		}
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVm.ShoppingCartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

            ShoppingCartVm.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVm.OrderHeader.ApplicationUserId = userId;

            var ApplicationUser = unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			foreach (var cart in ShoppingCartVm.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVm.OrderHeader.OrderTotal += cart.Price * cart.Count;
			}

			if (ApplicationUser.CompanyId.GetValueOrDefault()==0)
            {
                //Customer
                ShoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVm.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //Company
                ShoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVm.OrderHeader.OrderStatus = SD.StatusApproved;
			}
            unitOfWork.OrderHeader.Add(ShoppingCartVm.OrderHeader);
            unitOfWork.Save();
            foreach(var cart in ShoppingCartVm.ShoppingCartList)
            {
                var OrderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVm.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                unitOfWork.OrderDetail.Add(OrderDetail);
                unitOfWork.Save();
            }

			if (ApplicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                var domain = "https://localhost:7140/";
				var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain+ $"Customer/ShoppingCart/OrderConfirmation?id={ShoppingCartVm.OrderHeader.Id}",
                    CancelUrl = domain+ "Customer/ShoppingCart/Index",
                    Mode = "payment",
					LineItems = new List<SessionLineItemOptions>()
                };
				foreach (var item in ShoppingCartVm.ShoppingCartList)
				{
                    var SessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
						{
                            UnitAmount = (long)(item.Price *100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
							{
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(SessionLineItem);
				}
				var service = new SessionService();
				Session session = service.Create(options);
                unitOfWork.OrderHeader.updateStripePaymentID(ShoppingCartVm.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}
           

			return RedirectToAction(nameof(OrderConfirmation), new {id = ShoppingCartVm.OrderHeader.Id});
		}
		public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = unitOfWork.OrderHeader.Get(u=>u.Id == id,includeProperties:"User");
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment) //customer
            {
                var Service = new SessionService();
                var session = Service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower()=="paid")
                {
                    unitOfWork.OrderHeader.updateStripePaymentID(id,session.Id,session.PaymentIntentId);
                    unitOfWork.OrderHeader.updateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);

                    var CartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId);
                    unitOfWork.ShoppingCart.RemoveRange(CartList);
                    unitOfWork.Save();
                    
                }
			}
            return View(id);
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
