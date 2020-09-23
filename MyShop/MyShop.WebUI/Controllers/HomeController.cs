using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> repository;
        IRepository<ProductCategory> productCategories;

        public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            repository = productContext;
            productCategories = productCategoryContext;
        }
        // GET: ProductManagement
        public ActionResult Index(string Category = null)
        {
            List<Product> products; 
            List<ProductCategory> categories = productCategories.Collection().ToList();
            if (Category == null)
            {
                products = repository.Collection().ToList();
            } else
            {
                products = repository.Collection().Where(product => product.Category == Category).ToList();
            }
            ProductListViewModel model = new ProductListViewModel(products, categories);
            return View(model);
           
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Details(string Id)
        {
            Product product = repository.Find(Id);
            if(product != null)
            {
                return View(product);
            } else
            {
                return HttpNotFound();
            }
        }
    }
}