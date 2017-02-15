using Carrinho.Core;
using Carrinho.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carrinho.WebApi
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        readonly ICheckoutManager _checkoutManager;

        public CartController(ICheckoutManager checkoutManager)
        {
            this._checkoutManager = checkoutManager;
        }

        [HttpPost]
        [ResponseCache(NoStore = true)]
        //[ValidateAntiForgeryToken]
        public CartDTO Post([FromBody]CartItemDTO value)
        {
            var cart = _checkoutManager.GetCart();
            var cartItem = cart.CartItems.Where(i => i.SKU == value.SKU).SingleOrDefault();
            if (cartItem != null)
            {
                cartItem.Quantity = value.Quantity;
                var recalculatedCart = _checkoutManager.GetCart(cart.CartItems);

                _checkoutManager.SaveCart(cartItem);
                return recalculatedCart;
            }
            else
            {
                return cart;
            }
        }
    }
}
