using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Enums;
using BizzyQuote.Data.Managers;
using BizzyQuote.Models;

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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult ListByCompany()
        {
            List<Quote> quotes;
            int companyID = 0;
            using (var um = new UserManager())
            {
                // any user tied to a company can only see their company quotes
                var currentUser = um.ByUsername(User.Identity.Name);
                if (currentUser.CompanyID != null)
                {
                    companyID = currentUser.CompanyID.GetValueOrDefault();
                }
            }
            using (var qm = new QuoteManager())
            {
                quotes = qm.ActiveByCompany(companyID).OrderByDescending(q => q.CreatedOn).ToList();
            }

            ViewBag.Quotes = quotes;
            return View("QuoteList");
        }

        [Authorize(Roles = "Administrator,Manager,Member")]
        public ActionResult ListByEmployee()
        {
            List<Quote> quotes;
            int userID = 0;
            using (var um = new UserManager())
            {
                // any normal user can only see their own quotes
                var currentUser = um.ByUsername(User.Identity.Name);
                userID = currentUser.ID;
            }
            using (var qm = new QuoteManager())
            {
                quotes = qm.ActiveByEmployee(userID).OrderByDescending(q => q.CreatedOn).ToList();
            }

            ViewBag.Quotes = quotes;
            return View("QuoteList");
        }

        [Authorize(Roles = "Administrator,Manager,Member")]
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
            return RedirectToAction("Options", new { id = quote.ID});
        }

        [Authorize(Roles = "Administrator,Manager,Member")]
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

                var productLines = mm.ActiveProductLines().ToList();
                ViewBag.ProductLines = productLines.AsEnumerable();
                ViewBag.ProductToProductLine = mm.AllProductToProductLine().ToList();
                ViewBag.MaterialToProducts = mm.AllMaterialToProducts().ToList();
            }

            
            ViewBag.Products = products;

            
            return View("Edit");
        }

        [Authorize(Roles = "Administrator,Manager,Member")]
        public ActionResult Edit2(int id)
        {
            Quote quote;
            List<QuoteOption> options;
            using (var qm = new QuoteManager())
            {
                quote = qm.Single(id);
                options = qm.QuoteOptions(id).ToList();
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

                var productLines = mm.ActiveProductLines().ToList();
                ViewBag.PartsOfHouse = productLines.AsEnumerable();
                ViewBag.ProductToProductLine = mm.AllProductToProductLine().ToList();
                ViewBag.MaterialToProducts = mm.AllMaterialToProducts().ToList();
            }


            ViewBag.Products = products;


            return View("Edit");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager,Member")]
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
        [Authorize(Roles = "Administrator,Manager,Member")]
        public ActionResult AddQuoteItem(QuoteItem item)
        {
            Material material;
            List<WasteFactor> wasteFactors = new List<WasteFactor>();
            using (var mm = new MaterialsManager())
            {
                material = mm.SingleMaterial(item.MaterialID.GetValueOrDefault());
            }
            using (var qm = new QuoteManager())
            {
                var quote = qm.Single(item.QuoteID);
                using (var wfm = new WasteFactorManager())
                {
                    wasteFactors = wfm.ByCompany(quote.CompanyID.GetValueOrDefault()).ToList();
                }
                decimal wasteFactor =
                            wasteFactors.Any(
                                wf => wf.ProductID == item.ProductID && wf.ProductLineID == item.ProductLineID)
                                ? wasteFactors.First(
                                    wf => wf.ProductID == item.ProductID && wf.ProductLineID == item.ProductLineID)
                                              .WasteFactor1.GetValueOrDefault() : 0M;
                // calculate the amount and save
                switch (item.Measurement)
                {
                    case(Measurement.SquareFeet):
                        if (item.SquareFt.GetValueOrDefault() == 0)
                        {
                            item.SquareFt = item.Height*item.Width;
                        }
                        var pieceSqFt = (material.Height - material.Overlap.GetValueOrDefault())*(1M/12M)*material.Width;
                        var pieces = Math.Ceiling((decimal)(item.SquareFt.GetValueOrDefault() * (1M + wasteFactor) / pieceSqFt));

                        item.Amount = pieces * material.UnitCost; 
                        break;
                    case (Measurement.LinearFeet):
                        item.Amount = item.LinearFt * (1M + wasteFactor) * material.UnitCost / material.Width;
                        break;
                    case (Measurement.Constant):
                        item.Amount = item.Dollars;
                        break;
                }
            
                item = qm.CreateItem(item);
            }
            return RedirectToAction("Edit", new { id = item.QuoteID });
        }

        [Authorize(Roles = "Administrator,Manager,Member")]
        public ActionResult Options(int id)
        {
            List<Manufacturer> manufacturers;
            List<ProductLine> productLines;

            var model = new QuoteOptionsModel {QuoteID = id};

            using (var mm = new ManufacturerManager())
            {
                manufacturers = mm.All().ToList();
            }

            using (var mm = new MaterialsManager())
            {
                productLines = mm.ActiveProductLines().ToList();
            }

            model.ManufacturerSelectionModels = manufacturers.Select(manufacturer => new ManufacturerSelectionModel
                {
                    IsSelected = false, 
                    ManufacturerID = manufacturer.ID, 
                    ManufacturerName = manufacturer.Name
                }).ToList();

            model.ProductLineSelectionModels = productLines.Select(productLine => new ProductLineSelectionModel
            {
                IsSelected = false,
                ProductLineID = productLine.ID,
                ProductLineName = productLine.Name
            }).ToList();


            return View("Options", model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager,Member")]
        public ActionResult Options(QuoteOptionsModel model)
        {
            using (var qm = new QuoteManager())
            {
                foreach (var psm in model.ProductLineSelectionModels)
                {
                    if (psm.IsSelected)
                    {
                        // add it to the quote options database
                        var option = new QuoteOption
                            {
                                QuoteID = model.QuoteID,
                                ContentCategory = QuoteOptionCategory.ProductLineMapping,
                                ForeignKey = psm.ProductLineID,
                                TableName = TableName.ProductLine
                            };
                        option = qm.CreateOption(option);
                    }
                }

                foreach (var msm in model.ManufacturerSelectionModels)
                {
                    if (msm.IsSelected)
                    {
                        // add it to the quote options database
                        var option = new QuoteOption
                            {
                                QuoteID = model.QuoteID,
                                ContentCategory = QuoteOptionCategory.ManufacturerDefault,
                                ForeignKey = msm.ManufacturerID,
                                TableName = TableName.Manufacturer
                            };
                        option = qm.CreateOption(option);
                    }
                }
            }
            return RedirectToAction("Edit", new { id = model.QuoteID });
        }

    }
}
