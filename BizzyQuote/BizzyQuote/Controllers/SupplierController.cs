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
            return RedirectToAction("Index", "Home");
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
            List<ProductToProductLineModel> phModel = new List<ProductToProductLineModel>();
            using (var mm = new MaterialsManager())
            {
                List<Product> products = mm.ActiveProducts().ToList();
                List<ProductLine> partsOfHouse = mm.ActivePartsOfHouse().ToList();
                List<ProductToLine> prodHouses = mm.AllProductToProductLine().ToList();

                model.Products = products.AsEnumerable();
                model.PartsOfHouse = partsOfHouse.AsEnumerable();

                // create the list based on our product and part list
                foreach (var partOfHouse in partsOfHouse)
                {
                    foreach (var product in products)
                    {
                        phModel.Add(new ProductToProductLineModel
                        {
                            IsActive = prodHouses.Any(ph => ph.ProductID == product.ID && ph.ProductLineID == partOfHouse.ID) && prodHouses.First(ph => ph.ProductID == product.ID && ph.ProductLineID == partOfHouse.ID).IsActive,
                            ProductID = product.ID,
                            ProductLineID = partOfHouse.ID,
                            ProductName = product.Name,
                            ProductLineName = partOfHouse.Name
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
            List<ProductToLine> parts = new List<ProductToLine>();
            // loop through to create a list and then save
            foreach (var part in model.ProductPartHouse.Where(m => m.IsActive))
            {
                parts.Add(new ProductToLine { IsActive = true, ProductLineID = part.ProductLineID, ProductID = part.ProductID});
            }

            // first delete all of the existing ones 
            using (var mm = new MaterialsManager())
            {
                mm.DeleteProductToProductLine();
                mm.CreateProductToProductLine(parts);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
