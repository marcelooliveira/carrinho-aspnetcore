using Carrinho.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carrinho.Core
{
    public interface ICheckoutManager
    {
        CartDTO GetCart();
        void SaveCart(CartItemDTO modifiedItem);
        CartDTO GetCart(List<CartItemDTO> cartItems);
        CheckoutSummaryDTO GetCheckoutSummary();
        List<ProductDTO> GetProducts();
        Context InitializeDB();
    }
}
