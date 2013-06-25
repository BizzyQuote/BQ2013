using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Managers;

namespace BizzyQuote.Controllers
{
    [Authorize(Roles = "Administrator,Manager")]
    public class MaterialsController : Controller
    {
        //
        // GET: /Materials/

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult Products()
        {
            List<Product> products;

            using (var mm = new MaterialsManager())
            {
                products = mm.ActiveProducts().ToList();
            }

            ViewBag.Products = products;
            return View("ProductList");
        }

        public ActionResult Materials()
        {
            List<Material> materials;

            using (var mm = new MaterialsManager())
            {
                materials = mm.ActiveMaterials().ToList();
            }

            ViewBag.Materials = materials;
            return View("MaterialList");
        }

        public ActionResult SupplierMaterials(int supplierID)
        {
            List<Material> materials;

            using (var mm = new MaterialsManager())
            {
                materials = mm.BySupplier(supplierID).ToList();
            }

            ViewBag.Materials = materials;
            return View("MaterialList");
        }

        public ActionResult AllProducts()
        {
            List<Product> products;

            using (var mm = new MaterialsManager())
            {
                products = mm.AllProducts().ToList();
            }

            ViewBag.Products = products;
            return View("ProductList");
        }

        public ActionResult AllMaterials()
        {
            List<Material> materials;

            using (var mm = new MaterialsManager())
            {
                materials = mm.AllMaterials().ToList();
            }

            ViewBag.Materials = materials;
            return View("MaterialList");
        }

        public ActionResult CreateProduct()
        {
            ViewBag.Product = new Product();
            return View("EditProduct");
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            using (var mm = new MaterialsManager())
            {
                product = mm.CreateProduct(product);
            }
            return RedirectToAction("Products");
        }

        public ActionResult EditProduct(int id)
        {
            using (var mm = new MaterialsManager())
            {
                var product = mm.SingleProduct(id);
                ViewBag.Product = product;
                return View("EditProduct");
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            using (var mm = new MaterialsManager())
            {
                product = mm.EditProduct(product);
            }

            return RedirectToAction("Products");
        }

        public ActionResult CreateMaterial()
        {
            List<Product> products;
            using (var mm = new MaterialsManager())
            {
                products = mm.AllProducts().ToList();
            }
            var selectList = new SelectList(products, "ID", "Name");
            ViewBag.Products = selectList.AsEnumerable();
            ViewBag.Material = new Material();
            return View("EditMaterial");
        }

        [HttpPost]
        public ActionResult CreateMaterial(Material material)
        {
            using (var mm = new MaterialsManager())
            {
                material = mm.CreateMaterial(material);
            }
            return RedirectToAction("Materials");
        }

        public ActionResult EditMaterial(int id)
        {
            using (var mm = new MaterialsManager())
            {
                
                var material = mm.SingleMaterial(id);
                ViewBag.Material = material;

                List<Product> products;
                products = mm.AllProducts().ToList();
                //var selectList = new SelectList(products, "ID", "Name", material.ProductID);
                //ViewBag.Products = selectList.AsEnumerable();

                return View("EditMaterial");
            }
        }

        [HttpPost]
        public ActionResult EditMaterial(Material material)
        {
            using (var mm = new MaterialsManager())
            {
                material = mm.EditMaterial(material);
            }

            return RedirectToAction("Materials");
        }

        public ActionResult AvailableProducts(int materialID)
        {
            List<Product> products;
            List<MaterialToProduct> materialProducts;
            using (var mm = new MaterialsManager())
            {
                products = mm.ActiveProducts().OrderBy(p => p.Name).ToList();
                materialProducts = mm.ByMaterialID(materialID).ToList();
            }
            ViewBag.Products = products;
            ViewBag.MaterialToProducts = materialProducts;
            return View("AvailableProducts");
        }
    }
}
