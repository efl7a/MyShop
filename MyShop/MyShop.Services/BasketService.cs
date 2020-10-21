using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService : IBasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContext, IRepository<Basket> BasketContext)
        {
            this.productContext = ProductContext;
            this.basketContext = BasketContext;
        }


        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if(cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId);
                }
                else
                {
                    if(createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            } else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }
            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(5);
            httpContext.Response.Cookies.Add(cookie);
            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);

            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);
            if(item != null)
            {
                item.Quantity++;
            }
            else
            {
                item = new BasketItem()
                {
                    ProductId = productId,
                    BasketId = basket.Id,
                    Quantity = 1
                };
                basket.BasketItems.Add(item);
            }
            //This is not necessary for updating the quantity due to entity, but...
            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            
            if(basket != null)
            {
                BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    basket.BasketItems.Remove(item);
                    basketContext.Commit();
                }
            }
        }

        public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            if(basket != null)
            {
                //var result = (from b in basket.BasketItems
                //                join p in productContext.Collection() on b.ProductId equals p.Id
                //                select new BasketItemViewModel() { 
                //                    Image = p.Image,
                //                    Name = p.Name,
                //                    Quantity = b.Quantity,
                //                    Price = p.Price
                //                });
                var result = basket.BasketItems.Join<BasketItem, Product, string, BasketItemViewModel>(
                                productContext.Collection(),
                                basketItem => basketItem.ProductId,
                                product => product.Id,
                                (basketItem, product) =>
                                     new BasketItemViewModel()
                                     {
                                         Id = basketItem.Id,
                                         Quantity = basketItem.Quantity,
                                         Image = product.Image,
                                         Name = product.Name,
                                         Price = product.Price
                                     }).ToList();
                return result;
            }

            return new List<BasketItemViewModel>();
        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);

            if (basket != null)
            {
                int? basketCount = basket.BasketItems.Select(item => item.Quantity).Sum();
                decimal? basketTotal = basket.BasketItems.Join<BasketItem, Product, string, decimal>(
                                productContext.Collection(),
                                basketItem => basketItem.ProductId,
                                product => product.Id,
                                (basketItem, product) =>
                                     basketItem.Quantity * product.Price).Sum();

                if(basketCount != null && basketTotal != null)
                {
                    model.BasketCount = (int) basketCount;
                    model.BasketTotal = (decimal) basketTotal;
                }
            }
            return model;
        }

    }
}
