using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using sb_admin_2.Web1.Domain;

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

        public ActionResult Person()
        {
            PersonData data = new PersonData();
            data.Person.CreateTestData();
            data.ActionList.CreateTestData(10);
            data.ContactList.CreateTestData(2);
            data.DocumentList.CreateTestData(2);
            return View("Person", data);
        }

        public ActionResult Order()
        {
            OrderData data = new OrderData();
            data.Order.CreateTestData();
            data.PersonList.CreateTestData(10);
            data.InvoiceList.CreateTestData(10);
            data.ActionList.CreateTestData(10);
            return View("Order", data);
        }

        public ActionResult Main()
        {
            MainData data = new MainData();
            data.PersonList.CreateTestData(20);
            data.OrderList.CreateTestData(20);
            return View("Main", data);
        }

        public ActionResult Actions()
        {
            ActionData data = new ActionData();
            data.ActionInform.CreateTestData();
            return View("Actions", data);
        }
    }
}