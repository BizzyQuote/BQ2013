using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Enums;
using BizzyQuote.Data.Managers;

namespace BizzyQuote.Controllers
{
    public class QuotesController : Controller
    {
        //
        // GET: /Quotes/

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult List()
        {
            List<Quote> quotes;

            using (var qm = new QuoteManager())
            {
                quotes = qm.All().ToList();
            }

            ViewBag.Quotes = quotes;
            return View("QuoteList");
        }

        public ActionResult Create()
        {
            // We create a shell of a quote and redirect to the edit
            var quote = new Quote();
            quote.CreatedOn = DateTime.Now;
            quote.ModifiedOn = DateTime.Now;
            quote.IsActive = false;
            using (var um = new UserManager())
            {
                var user = um.ByUsername(User.Identity.Name);
                if (user.CompanyID != null)
                    quote.CompanyID = user.CompanyID;
                quote.EmployeeID = user.ID;
            }
            using (var qm = new QuoteManager())
            {
                quote = qm.Create(quote);
            }
            return RedirectToAction("Edit", new { id = quote.ID});
        }

        public ActionResult Edit(int id)
        {
            Quote quote;
            using (var qm = new QuoteManager())
            {
                quote = qm.Single(id);
                ViewBag.Quote = quote;
            }

            // determine the supplier and materials available
            List<CompanyToSupplier> companySuppliers;
            using (var sm = new SupplierManager())
            {
                companySuppliers = sm.ByCompanyID(quote.CompanyID.GetValueOrDefault()).ToList();
            }
            List<Product> products;
            using (var mm = new MaterialsManager())
            {
                List<Material> allMaterials = new List<Material>();
                foreach (var companyToSupplier in companySuppliers)
                {
                    allMaterials.AddRange(mm.BySupplier(companyToSupplier.SupplierID));
                }
                ViewBag.AvailableMaterials = allMaterials;
            
                products = mm.ActiveProducts(quote.CompanyID.GetValueOrDefault()).ToList();

                var partsOfHouse = mm.ActivePartsOfHouse().ToList();
                ViewBag.PartsOfHouse = partsOfHouse.AsEnumerable();
            }

            
            ViewBag.Products = products;

            
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Edit(Quote quote)
        {
            quote.IsActive = true;
            using (var qm = new QuoteManager())
            {
                quote = qm.Edit(quote);
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult AddQuoteItem(QuoteItem item)
        {
            Material material;
            using (var mm = new MaterialsManager())
            {
                material = mm.SingleMaterial(item.MaterialID.GetValueOrDefault());
            }
            // calculate the amount and save
            switch (item.Measurement)
            {
                case(Measurement.SquareFeet):
                    item.Amount = item.Height*item.Width*material.UnitCost / material.UnitSize; 
                    break;
                case (Measurement.LinearFeet):
                    item.Amount = item.LinearFt*material.UnitCost / material.UnitSize;
                    break;
                case (Measurement.Constant):
                    break;
            }
            using (var qm = new QuoteManager())
            {
                item = qm.CreateItem(item);
            }
            return Edit(item.QuoteID);
        }

    }
}
