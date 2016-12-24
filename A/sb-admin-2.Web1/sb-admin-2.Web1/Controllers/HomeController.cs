﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using sb_admin_2.Web1.Domain;
using sb_admin_2.Web1.Models.Mapping;
using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FlotCharts()
        {
            return View("FlotCharts");
        }

        public ActionResult MorrisCharts()
        {
            return View("MorrisCharts");
        }

        public ActionResult Tables()
        {
            return View("Tables");
        }

        public ActionResult Forms()
        {
            return View("Forms");
        }

        public ActionResult Panels()
        {
            return View("Panels");
        }

        public ActionResult Buttons()
        {
            return View("Buttons");
        }

        public ActionResult Notifications()
        {
            return View("Notifications");
        }

        public ActionResult Typography()
        {
            return View("Typography");
        }

        public ActionResult Icons()
        {
            return View("Icons");
        }

        public ActionResult Grid()
        {
            return View("Grid");
        }

        public ActionResult Blank()
        {
            return View("Blank");
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        static HomeController()
        {
            mappingController = new MappingController();
        }

        static private MappingController mappingController { get; set; }

        public ActionResult Person()
        {
            return View("Person", mappingController.personData);
        }

        public ActionResult Order()
        {
            return View("Order", mappingController.orderData);
        }

        public ActionResult Main()
        {
            MainData data = new MainData();
            data.OrderList.CreateTestData(20);
            return View("Main", data);
        }

        public ActionResult Actions()
        {
            ActionData data = new ActionData();
            data.ActionInform.CreateTestData();
            return View("Actions", data);
        }

        [HttpPost]
        public ActionResult SaveButtonAvia(int id, string Status)
        {
            Invoice invoice = mappingController.orderData.InvoiceList.Find((item) => { return item.ID == id; });
            if (invoice != null)
            {
                invoice.Status = Status;
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SaveOrderInformation(string Description, string DescriptionWide, string Keys)
        {
            mappingController.orderData.Order.Description = Description;
            mappingController.orderData.Order.DescriptionWide = DescriptionWide;
            mappingController.orderData.Order.Keys = Keys;
            return Json("");
        }

        [HttpPost]
        public ActionResult GetOrderInformation()
        {
            return Json(mappingController.orderData.Order);
        }

        [HttpPost]
        public ActionResult UpdateDescriptionWide(string val)
        {
            mappingController.orderData.Order.DescriptionWide = val;
            return Json(val);
        }

        [HttpPost]
        public ActionResult UpdateKeys(string val)
        {
            mappingController.orderData.Order.Keys = val;
            return Json(val);
        }

        [HttpPost]
        public ActionResult FindIncoice(int id)
        {
            Invoice invoice = mappingController.orderData.InvoiceList.Find((item) => { return item.ID == id; });
            return Json(invoice);
        }

        [HttpPost]
        public ActionResult FindPassport(int id)
        {
            Passport passport = mappingController.personData.Person.PassportList.Find((item) => { return item.ID == id; });
            return Json(passport);
        }

        [HttpPost]
        public ActionResult GetPersonalData()
        {
            return Json(mappingController.personData.Person);
        }

        [HttpPost]
        public ActionResult FindContact(int id)
        {
            Contact contact = mappingController.personData.Person.ContactList.Find((item) => { return item.ID == id; });
            return Json(contact);
        }

        public ActionResult FindViza(int id, string PassportSerial)
        {
            Viza viza = null;
            Passport passport = mappingController.personData.Person.PassportList.Find((item) => { return item.SerialNumber == PassportSerial; });
            if (passport != null)
                viza = passport.VizaList.Find((item) => { return item.ID == id; });
            return Json(viza);
        }
    }
}