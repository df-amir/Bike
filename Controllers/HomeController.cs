using Bike.Classes;
using Bike.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Bike.Controllers
{
    public class HomeController : Controller
    {
        private BikeEntities db = new BikeEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "Id,PhoneNumber,Password,RoleId")] Account account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(account?.PhoneNumber) || string.IsNullOrEmpty(account?.Password))
                        ModelState.AddModelError(nameof(account.PhoneNumber), "Entering the contact number and password is mandatory!");

                    var _account = db.Account.Where(q => q.PhoneNumber == account.PhoneNumber)?.FirstOrDefault();
                    if (_account == null)
                    {
                        account.RoleId = (int)Enums.Roles.Client;
                        db.Account.Add(account);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                        ModelState.AddModelError(nameof(account.PhoneNumber), "This contact number has already been used!");
                }
                return View(account);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login([Bind(Include = "Id,PhoneNumber,Password,RoleId")] Account account, string ReturnUrl = "/")
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (account != null)
                    { 
                        string _phoneNumber = account.PhoneNumber?.Trim();
                        string _Password = account.Password;
                        var _account = db.Account.Where(q => q.PhoneNumber.Trim() == _phoneNumber && q.Password == _Password)?.FirstOrDefault();
                        if (_account != null)
                        {
                            FormsAuthentication.SetAuthCookie(account.PhoneNumber, true);
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(account.PhoneNumber), "Unfortunately, no user was found with this profile!");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}