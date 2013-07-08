using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Enums;
using BizzyQuote.Data.Managers;
using BizzyQuote.Models;

namespace BizzyQuote.Controllers
{
    [Authorize(Roles = "Administrator,Manager,Supplier")]
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
            MaterialModel model = new MaterialModel();
            return View("EditMaterial", model);
        }

        [HttpPost]
        public ActionResult CreateMaterial(MaterialModel model)
        {
            using (var mm = new MaterialsManager())
            {
                // TODO: Refactor into an extention method that uses generics 
                var material = new Material
                    {
                        Name = model.Name,
                        SubName = model.SubName,
                        Measurement = model.Measurement,
                        CreatedOn = model.CreatedOn,
                        ModifiedOn = model.ModifiedOn,
                        UnitCost = model.UnitCost,
                        IsActive = model.IsActive,
                        ManufacturerID = model.ManufacturerID,
                        Height = model.Height,
                        Width = model.Width,
                        Thickness = model.Thickness,
                        Finish = model.Finish,
                        Overlap = model.Overlap,
                        SKU = model.SKU
                    };
                material = mm.CreateMaterial(material);
            }
            return RedirectToAction("Materials");
        }

        public ActionResult EditMaterial(int id)
        {
            using (var mm = new MaterialsManager())
            {
                var material = mm.SingleMaterial(id);
                var model = new MaterialModel(material);
                return View("EditMaterial", model);
            }
        }

        [HttpPost]
        public ActionResult EditMaterial(MaterialModel model)
        {
            using (var mm = new MaterialsManager())
            {
                // TODO: Refactor into an extention method that uses generics 
                var material = new Material
                {
                    Name = model.Name,
                    SubName = model.SubName,
                    Measurement = model.Measurement,
                    CreatedOn = model.CreatedOn,
                    ModifiedOn = model.ModifiedOn,
                    UnitCost = model.UnitCost,
                    IsActive = model.IsActive,
                    ManufacturerID = model.ManufacturerID,
                    Height = model.Height,
                    Width = model.Width,
                    Thickness = model.Thickness,
                    Texture = model.Texture,
                    Finish = model.Finish,
                    Overlap = model.Overlap,
                    SKU = model.SKU,
                    ID = model.ID
                };

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

        [Authorize(Roles = "Administrator,Supplier")]
        public ActionResult MaterialProductsGrid(int? supplierID)
        {
            using (var um = new UserManager())
            {
                var currentUser = um.ByUsername(User.Identity.Name);
                if (currentUser.SupplierID != null)
                {
                    supplierID = currentUser.SupplierID.GetValueOrDefault();
                }
            }

            var model = new MaterialProductGridModel();
            var materialToProductModels = new List<MaterialToProductModel>();

            using (var mm = new MaterialsManager())
            {
                var products = mm.ActiveProducts().ToList();
                var materials = mm.BySupplier(supplierID.GetValueOrDefault()).ToList();
                var materialToProducts = mm.ByManufacturerID(supplierID.GetValueOrDefault()).ToList();

                model.Products = products.AsEnumerable();
                model.Materials = materials.AsEnumerable();

                // create the list based on our product and material list
                foreach (var material in materials)
                {
                    foreach (var product in products)
                    {
                        materialToProductModels.Add(new MaterialToProductModel
                        {
                            IsActive = materialToProducts.Any(mp => mp.ProductID == product.ID && mp.MaterialID == material.ID),
                            ProductID = product.ID,
                            MaterialID = material.ID,
                            ProductName = product.Name,
                            MaterialName = material.Name,
                            MaterialToProductID = materialToProducts.Any(mp => mp.ProductID == product.ID && mp.MaterialID == material.ID) ? materialToProducts.First(mp => mp.ProductID == product.ID && mp.MaterialID == material.ID).ID : 0
                        });
                    }
                }
                model.MaterialProducts = materialToProductModels.AsEnumerable();
            }

            return View("MaterialProductsGrid", model);
        }

        [Authorize(Roles = "Administrator,Supplier")]
        [HttpPost]
        public ActionResult MaterialProductsGrid(MaterialProductGridModel model)
        {
            List<MaterialToProduct> mpAdd = new List<MaterialToProduct>();
            List<int> mpDelete = new List<int>();
            foreach (var mp in model.MaterialProducts)
            {
                if (mp.IsActive)
                {
                    // we only need to add new ones since nothing will change with existing ones
                    if (mp.MaterialToProductID == 0)
                    {
                        mpAdd.Add(new MaterialToProduct
                            {
                                ID = mp.MaterialToProductID,
                                MaterialID = mp.MaterialID,
                                ProductID = mp.ProductID
                            });
                    }
                }
                else
                {
                    // only need to delete ones that already exist in the database
                    if (mp.MaterialToProductID > 0)
                    {
                        mpDelete.Add(mp.MaterialToProductID);
                    }
                }
            }

            // now update the database by deleting ones and updating others
            using (var mm = new MaterialsManager())
            {
                mm.DeleteMaterialToProducts(mpDelete);
                mm.InsertMaterialToProducts(mpAdd);
            }

            return RedirectToAction("Index");
        }

        //[HttpPost]
        public ActionResult Clone(int id)
        {
            Material newMaterial = new Material();
            using (var mm = new MaterialsManager())
            {
                Material mat = mm.SingleMaterial(id);

                // use reflection to populate the new Material

                // reflect to get instances of the entity preoprties
                var editProperties =
                    from p in mat.GetType().GetProperties()
                    select p;

                // copy the values
                PropertyInfo[] editProps = editProperties.ToArray();
                foreach (PropertyInfo propertyInfo in editProps)
                {
                    propertyInfo.SetValue(newMaterial, propertyInfo.GetValue(mat, null), null);
                }

                // set specifics
                newMaterial.Name = mat.Name + " - Clone";
                if (newMaterial.Name.Length > 100)
                {
                    newMaterial.Name = newMaterial.Name.Substring(0, 100);
                }
                newMaterial.CreatedOn = DateTime.Now;
                newMaterial.ModifiedOn = DateTime.Now;
                newMaterial.ID = 0;


                newMaterial = mm.CreateMaterial(newMaterial);
            }
            return RedirectToAction("EditMaterial", new {id = newMaterial.ID});
        }

    }
}
