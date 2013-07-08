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
    public class PricingController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult BySupplier(int id)
        {
            List<Pricing> prices;
            List<Material> materials;
            int supplierID = id;
            using (var um = new UserManager())
            {
                // any user tied to a supplier can only see their supplier rates
                var currentUser = um.ByUsername(User.Identity.Name);
                if (currentUser.SupplierID != null)
                {
                    supplierID = currentUser.SupplierID.GetValueOrDefault();
                }
            }

            using (var mm = new MaterialsManager())
            {
                materials = mm.BySupplier(supplierID).ToList();
            }

            using (var pm = new PricingManager())
            {
                prices = pm.BySupplier(supplierID).ToList();
            }

            // populate the model
            List<PricingModel> model = new List<PricingModel>();
            foreach (var mat in materials)
            {
                var m = new PricingModel();
                if (prices.Any(p => p.MaterialID == mat.ID))
                {
                    var price = prices.FirstOrDefault(p => p.MaterialID == mat.ID);
                    m.ID = price.ID;
                    m.CompanyID = price.CompanyID;
                    m.MaterialID = price.MaterialID;
                    m.SupplierID = price.SupplierID;
                    m.CreatedOn = price.CreatedOn;
                    m.CreatedBy = price.CreatedBy;
                    m.ModifiedBy = price.ModifiedBy;
                    m.ModifiedOn = price.ModifiedOn;
                    m.Price = price.Price;
                    m.MaterialName = mat.Name;
                }
                else
                {
                    m.ID = 0;
                    m.MaterialID = mat.ID;
                    m.SupplierID = supplierID;
                    m.CreatedOn = DateTime.Now;
                    m.CreatedBy = User.Identity.Name;
                    m.ModifiedBy = User.Identity.Name;
                    m.ModifiedOn = DateTime.Now;
                    m.MaterialName = mat.Name;
                }

                model.Add(m);
            }

            return View("PricingList", model);
        }

        [HttpPost]
        public ActionResult BySupplier(List<PricingModel> models)
        {
            var prices = models.Select(model => new Pricing()
            {
                ID = model.ID,
                CompanyID = model.CompanyID,
                MaterialID = model.MaterialID,
                SupplierID = model.SupplierID,
                CreatedOn = model.CreatedOn,
                ModifiedOn = model.ModifiedOn,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy,
                Price = model.Price
            }).Where(m => m.Price.GetValueOrDefault() > 0 || m.ID > 0).ToList();

            using (var pm = new PricingManager())
            {
                var result = pm.UpdatePrices(prices);
            }

            return RedirectToAction("Index");
        }

    }
}
