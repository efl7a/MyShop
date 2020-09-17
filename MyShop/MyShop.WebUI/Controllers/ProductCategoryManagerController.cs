using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.DataAcess.InMemory;

namespace MyShop.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> repository;

        public ProductCategoryManagerController(IRepository<ProductCategory> productCategoryContext)
        {
            repository = productCategoryContext;
        }
        // GET: ProductManagement
        public ActionResult Index()
        {
            List<ProductCategory> productCategories = repository.Collection().ToList();
            return View(productCategories);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }
            else
            {
                repository.Insert(productCategory);
                repository.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            ProductCategory productCategoryToEdit = repository.Find(Id);
            if (productCategoryToEdit != null)
            {
                return View(productCategoryToEdit);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory product, string Id)
        {
            ProductCategory productCategoryToEdit = repository.Find(Id);
            if (productCategoryToEdit != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }
                else
                {
                    productCategoryToEdit.Category = product.Category;

                    repository.Commit();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult Delete(string Id)
        {
            ProductCategory productCategoryToDelete = repository.Find(Id);
            if (productCategoryToDelete != null)
            {
                return View(productCategoryToDelete);
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
            ProductCategory productCategoryToDelete = repository.Find(Id);
            if (productCategoryToDelete != null)
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