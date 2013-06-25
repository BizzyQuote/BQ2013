using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Managers;
using BizzyQuote.Models;

namespace BizzyQuote.Controllers
{
    public class SupplierController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            List<Supplier> suppliers;

            using (var cm = new SupplierManager())
            {
                suppliers = cm.All().ToList();
            }

            ViewBag.Suppliers = suppliers;
            return View("SupplierList");
        }

        public ActionResult Create()
        {
            ViewBag.Supplier = new Supplier();
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Create(Supplier supplier)
        {
            using (var cm = new SupplierManager())
            {
                supplier = cm.Create(supplier);
            }
            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            using (var cm = new SupplierManager())
            {
                var supplier = cm.Single(id);
                ViewBag.Supplier = supplier;
                return View("Edit");
            }
        }

        [HttpPost]
        public ActionResult Edit(Supplier supplier)
        {
            using (var cm = new SupplierManager())
            {
                supplier = cm.Edit(supplier);
            }
            return RedirectToAction("List");
        }

        public ActionResult Products()
        {
            ProductHouseModel model = new ProductHouseModel();
            List<ProductToPartOfHouseModel> phModel = new List<ProductToPartOfHouseModel>();
            using (var mm = new MaterialsManager())
            {
                List<Product> products = mm.ActiveProducts().ToList();
                List<PartOfHouse> partsOfHouse = mm.ActivePartsOfHouse().ToList();
                List<ProductToPartOfHouse> prodHouses = mm.AllProductToPartOfHouse().ToList();

                model.Products = products.AsEnumerable();
                model.PartsOfHouse = partsOfHouse.AsEnumerable();

                // create the list based on our product and part list
                foreach (var partOfHouse in partsOfHouse)
                {
                    foreach (var product in products)
                    {
                        phModel.Add(new ProductToPartOfHouseModel
                        {
                            IsActive = prodHouses.Any(ph => ph.ProductID == product.ID && ph.PartOfHouseID == partOfHouse.ID) && prodHouses.First(ph => ph.ProductID == product.ID && ph.PartOfHouseID == partOfHouse.ID).IsActive,
                            ProductID = product.ID,
                            PartOfHouseID = partOfHouse.ID,
                            ProductName = product.Name,
                            PartOfHouseName = partOfHouse.Name
                        });
                    }
                }
                model.ProductPartHouse = phModel.AsEnumerable();
            }
            return View("Products", model);
        }

        [HttpPost]
        public ActionResult Products(ProductHouseModel model)
        {
            List<ProductToPartOfHouse> parts = new List<ProductToPartOfHouse>();
            // loop through to create a list and then save
            foreach (var part in model.ProductPartHouse.Where(m => m.IsActive))
            {
                parts.Add(new ProductToPartOfHouse { IsActive = true, PartOfHouseID = part.PartOfHouseID, ProductID = part.ProductID});
            }

            // first delete all of the existing ones 
            using (var mm = new MaterialsManager())
            {
                mm.DeleteProductToPartOfHouse();
                mm.CreateProductToPartOfHouse(parts);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
