using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Carrinho.Core;

namespace Carrinho.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICheckoutManager _checkoutManager;

        public HomeController(ICheckoutManager checkoutManager)
        {
            this._checkoutManager = checkoutManager;
        }

        public ActionResult Index()
        {
            return View(_checkoutManager.GetProducts());
        }

        public ActionResult Cart()
        {
            return View(_checkoutManager.GetCart());
        }

        public ActionResult CheckoutSuccess()
        {
            return View(_checkoutManager.GetCheckoutSummary());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(string SKU)
        {
            _checkoutManager.SaveCart(new Core.DTOs.CartItemDTO
            {
                SKU = SKU,
                Quantity = 1
            });
            return RedirectToAction("Cart", "Home");
        }
    }
}
