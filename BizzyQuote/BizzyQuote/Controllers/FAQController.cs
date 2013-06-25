using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Managers;

namespace BizzyQuote.Controllers
{
    public class FAQController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult List()
        {
            List<FAQ> faqs;

            using (var fm = new FAQManager())
            {
                faqs = fm.All().ToList();
            }

            ViewBag.FAQs = faqs;
            return View("FAQList");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.FAQ = new FAQ();
            return View("Edit");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Create(FAQ faq)
        {
            using (var fm = new FAQManager())
            {
                faq = fm.Create(faq);
            }
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            using (var fm = new FAQManager())
            {
                var faq = fm.Single(id);
                ViewBag.FAQ = faq;
            }
            return View("Edit");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Edit(FAQ faq)
        {
            using (var fm = new FAQManager())
            {
                faq = fm.Edit(faq);
            }
            return RedirectToAction("List");
        }

    }
}
