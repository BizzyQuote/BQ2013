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
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ListAll(int? companyID)
        {
            List<User> users = new List<User>();
            using (var um = new UserManager())
            {
                users = companyID != null ? um.ByCompany(companyID.GetValueOrDefault()).OrderByDescending(u => u.CreatedOn).ToList()
                    : um.All().OrderByDescending(u => u.CreatedOn).ToList();
                ViewBag.Users = users;
            }
            return View("List");
        }

        public ActionResult List(int? companyID)
        {
            List<User> users = new List<User>();
            using (var um = new UserManager())
            {
                // any user tied to a company can only see their users
                var currentUser = um.ByUsername(User.Identity.Name);
                if (currentUser.CompanyID != null)
                {
                    companyID = currentUser.CompanyID;
                }

                users = companyID != null ? um.ByCompany(companyID.GetValueOrDefault()).OrderByDescending(u => u.CreatedOn).ToList() 
                    : um.All().OrderByDescending(u => u.CreatedOn).ToList();
                ViewBag.Users = users;
            }
            return View("List");
        }

        public ActionResult ListBySupplier(int supplierID)
        {
            List<User> users = new List<User>();
            using (var um = new UserManager())
            {

                users = um.BySupplier(supplierID).OrderByDescending(u => u.CreatedOn).ToList();
                ViewBag.Users = users;
            }
            return View("List");
        }

        public ActionResult Create()
        {
            using (var cm = new CompanyManager())
            {
                ViewBag.CompanyList = cm.All().ToList();
            }
            ViewBag.User = new User();
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Create(User user)
        {
            using (var um = new UserManager())
            {
                user = um.Create(user);
            }
            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            using (var cm = new CompanyManager())
            {
                ViewBag.CompanyList = cm.All().OrderBy(c => c.Name).ToList();
            }
            using (var um = new UserManager())
            {
                var user = um.Single(id);
                ViewBag.User = user;
                return View("Edit");
            }
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            using (var um = new UserManager())
            {
                user = um.Edit(user);
            }
            return RedirectToAction("List");
        }

    }
}
