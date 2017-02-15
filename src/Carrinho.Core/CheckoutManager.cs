using Carrinho.Core.DTOs;
using Carrinho.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace Carrinho.Core
{
    public class CheckoutManager : ICheckoutManager
    {
        private string serverFilePath;
        private readonly DbContextOptions<Context> _dbOptions;

        public CheckoutManager(DbContextOptions<Context> dbOptions)
        {
            this._dbOptions = dbOptions;
        }

        public CheckoutManager(string serverFilePath)
        {
            this.serverFilePath = serverFilePath;
        }

        public CheckoutSummaryDTO GetCheckoutSummary()
        {
            var cartItems = GetCartItems();
            var subtotal = cartItems.Sum(i => i.Subtotal);
            var discountRule = DiscountManager.Instance.GetDiscount(subtotal);
            var discountValue = discountRule.CalculatedDiscount;
            var total = subtotal - discountValue;

            return new CheckoutSummaryDTO
            {
                OrderNumber = "123456789",
                DeliveryUpToNWorkingDays = 4,
                Total = total,
                CustomerInfo = GetDummyCustomerInfo(),
                CartItems = cartItems
            };
        }

        public CartDTO GetCart()
        {
            return GetCart(GetCartItems());
        }

        public CartDTO GetCart(List<CartItemDTO> cartItems)
        {
            var subtotal = cartItems.Sum(i => i.Subtotal);
            var discountRule = DiscountManager.Instance.GetDiscount(subtotal);
            var discountValue = discountRule.CalculatedDiscount;
            var total = subtotal - discountValue;

            return new CartDTO
            {
                Subtotal = subtotal,
                DiscountRate = discountRule.Rate * 100M,
                DiscountValue = discountValue,
                Total = total,
                CartItems = cartItems
            };
        }

        public void SaveCart(CartItemDTO newOrEditItem)
        {
            if (newOrEditItem.Quantity < 0)
                newOrEditItem.Quantity = 0;

            using (var db = new Context(this._dbOptions))
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var product = db.Product.Where(p => p.SKU == newOrEditItem.SKU).Single();

                    var cartItem =
                        (from ci in db.CartItem
                         join p in db.Product on ci.ProductId equals p.Id
                         where p.SKU == newOrEditItem.SKU
                         select ci)
                        .SingleOrDefault();

                    if (cartItem != null)
                    {
                        if (newOrEditItem.Quantity == 0)
                            db.CartItem.Remove(cartItem);
                        else
                        {
                            cartItem.Quantity = newOrEditItem.Quantity;
                            cartItem.Product = product;
                        }
                    }
                    else
                    {
                        db.CartItem.Add(new CartItem
                        {
                            Product = product,
                            Quantity = newOrEditItem.Quantity
                        });
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }

        public List<ProductDTO> GetProducts()
        {
            using (var db = new Context(this._dbOptions))
            {
                return db.Product
                    .Select(i =>
                    new ProductDTO
                    {
                        Id = i.Id,
                        SKU = i.SKU,
                        SmallImagePath = i.SmallImagePath,
                        Description = i.Description,
                        Price = i.Price
                    }).ToList();
            }
        }

        private List<CartItemDTO> GetCartItems()
        {
            using (var db = new Context(this._dbOptions))
            {
                return
                    (from ci in db.CartItem
                     from p in db.Product.Where(p => p.Id == ci.ProductId)
                     select new { ci, p })
                    .Select(i =>
                    new CartItemDTO
                    {
                        Id = i.ci.Id,
                        SKU = i.p.SKU,
                        SmallImagePath = i.p.SmallImagePath,
                        Description = i.p.Description,
                        Price = i.p.Price,
                        Quantity = i.ci.Quantity
                    }).ToList();
            }
        }

        private CustomerInfoDTO GetDummyCustomerInfo()
        {
            return new CustomerInfoDTO
            {
                CustomerName = "John Doe",
                PhoneNumber = "(11) 555-12345",
                Email = "johndoe@email.com",
                DeliveryAddress = "503-250 Ferrand Drive - Toronto Ontario, M3C 3G8 Canada"
            };
        }

        public Context InitializeDB()
        {
            var db = new Context(this._dbOptions);

            bool bancoNovo = db.Database.EnsureCreated();

            if (bancoNovo)
            {
                var products = new string[]
                {
                    "10 Million Member CodeProject T-Shirt|3399",
                    "Women's T-Shirt|3399",
                    "CodeProject.com Body Suit|1399",
                    "CodeProject Mug Mugs|1099",
                    "RootAdmin Mug|1099",
                    "Drinking Glass|1099",
                    "Stein|1399",
                    "Mousepad|1099",
                    "Square Sticker|299",
                };

                var index = 1;
                foreach (var p in products)
                {
                    var description = p.Split('|')[0];
                    var price = decimal.Parse(p.Split('|')[1]) / 100M;

                    var product =
                    db.Product.Add(new Product
                    {
                        SKU = Guid.NewGuid().ToString(),
                        SmallImagePath = string.Format("images/products/small_{0}.jpg", index),
                        LargeImagePath = string.Format("images/products/large_{0}.jpg", index),
                        Description = description,
                        Price = price
                    }).Entity;

                    var cartItem =
                    db.CartItem.Add(new CartItem
                    {
                        Product = product,
                        Quantity = 1
                    }).Entity;

                    index++;
                }
            }
            db.SaveChanges();

            return db;
        }
    }
}
