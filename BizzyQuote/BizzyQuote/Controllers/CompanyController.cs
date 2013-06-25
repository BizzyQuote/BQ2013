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
    [Authorize(Roles = "Administrator,Manager")]
    public class CompanyController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult List()
        {
            List<Company> companies;

            using (var cm = new CompanyManager())
            {
                companies = cm.All().ToList();
            }

            ViewBag.Companies = companies;
            return View("CompanyList");
        }

        public ActionResult Create()
        {
            ViewBag.Company = new Company();
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Create(Company company)
        {
            using (var cm = new CompanyManager())
            {
                company = cm.Create(company);
            }
            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            using (var cm = new CompanyManager())
            {
                var company = cm.Single(id);
                ViewBag.Company = company;
                return View("Edit");
            }
        }

        [HttpPost]
        public ActionResult Edit(Company company)
        {
            using (var cm = new CompanyManager())
            {
                company = cm.Edit(company);
            }
            return RedirectToAction("List");
        }

        public ActionResult Suppliers(int companyID)
        {
            using (var sm = new SupplierManager())
            {
                List<Supplier> suppliers = sm.All().ToList();
                ViewBag.Suppliers = suppliers.AsEnumerable();
                List<CompanyToSupplier> companySuppliers = sm.ByCompanyID(companyID).ToList();
                ViewBag.CompanySuppliers = companySuppliers.AsEnumerable();
            }
            return View("Suppliers");
        }

        [HttpPost]
        public ActionResult Suppliers(Company company)
        {
            return RedirectToAction("List");
        }

        public ActionResult WasteFactors(int companyID)
        {
            var wasteFactors = new List<WasteFactor>();
            var model = new List<WasteFactorModel>();
            var prodParts = new List<ProductToPartOfHouse>();
            List<Product> products = new List<Product>();
            List<PartOfHouse> partsOfHouse = new List<PartOfHouse>();
            using (var mm = new MaterialsManager())
            {
                prodParts = mm.AllProductToPartOfHouse().ToList();
                products = mm.AllProducts().ToList();
                partsOfHouse = mm.ActivePartsOfHouse().ToList();
            }

            using (var wfm = new WasteFactorManager())
            {
                wasteFactors = wfm.ByCompany(companyID).ToList();
            }

            foreach (var pph in prodParts)
            {
                if (!wasteFactors.Any(wf => wf.ProductID == pph.ProductID && wf.PartOfHouseID == pph.PartOfHouseID))
                {
                    model.Add(new WasteFactorModel
                        {
                            CompanyID = companyID,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            PartOfHouseID = pph.PartOfHouseID,
                            ProductID = pph.ProductID,
                            ProductName = products.FirstOrDefault(p => p.ID == pph.ProductID).Name,
                            PartOfHouseName = partsOfHouse.FirstOrDefault(p => p.ID == pph.ProductID).Name
                        });
                }
                else
                {
                    var fact =
                        wasteFactors.FirstOrDefault(
                            wf => wf.ProductID == pph.ProductID && wf.PartOfHouseID == pph.PartOfHouseID);
                    model.Add(new WasteFactorModel
                        {
                            ID = fact.ID,
                            CompanyID = companyID,
                            CreatedOn = fact.CreatedOn,
                            ModifiedOn = fact.CreatedOn,
                            ModifiedBy = fact.ModifiedBy,
                            PartOfHouseID = pph.PartOfHouseID,
                            ProductID = pph.ProductID,
                            ProductName = products.FirstOrDefault(p => p.ID == pph.ProductID).Name,
                            PartOfHouseName = partsOfHouse.FirstOrDefault(p => p.ID == pph.ProductID).Name,
                            Factor = fact.WasteFactor1.GetValueOrDefault()
                        });
                }
            }

            return View("WasteFactors", model);
        }

        [HttpPost]
        public ActionResult WasteFactors(List<WasteFactorModel> models)
        {
            var factors = models.Select(model => new WasteFactor
                {
                    ID = model.ID, 
                    CompanyID = model.CompanyID, 
                    CreatedOn = model.CreatedOn, 
                    ModifiedOn = model.ModifiedOn, 
                    ModifiedBy = model.ModifiedBy, 
                    WasteFactor1 = model.Factor, 
                    PartOfHouseID = model.PartOfHouseID, 
                    ProductID = model.ProductID
                }).ToList();

            using (var wfm = new WasteFactorManager())
            {
                var result = wfm.UpdateWasteFactors(factors);
            }
            return RedirectToAction("List");
        }
    }
}
