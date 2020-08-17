using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    public class ProductManagementController : Controller
    {
        ProductRepository repository;

        public ProductManagementController()
        {
            repository = new ProductRepository();
        }
        // GET: ProductManagement
        public ActionResult Index()
        {
            List<Product> products = repository.Collection().ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            } else
            {
                repository.Insert(product);
                repository.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            Product productToEdit = repository.Find(Id);
            if(productToEdit != null)
            {
                return View(productToEdit);
            } else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(Product product, string Id)
        {
            Product productToEdit = repository.Find(Id);
            if(productToEdit != null)
            {
                if(!ModelState.IsValid)
                {
                    return View(product);
                } else
                {
                    productToEdit.Category = product.Category;
                    productToEdit.Description = product.Description;
                    productToEdit.Name = product.Name;
                    productToEdit.Image = product.Image;
                    productToEdit.Price = product.Price;

                    repository.Commit();
                    return RedirectToAction("Index");
                }
            } else
            {
                return HttpNotFound();
            }
        }

        public ActionResult Delete(string Id)
        {
            Product productToDelete = repository.Find(Id);
            if (productToDelete != null)
            {
                return View(productToDelete);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productToDelete = repository.Find(Id);
            if (productToDelete != null)
            {
                repository.Delete(Id);    
                repository.Commit();
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}