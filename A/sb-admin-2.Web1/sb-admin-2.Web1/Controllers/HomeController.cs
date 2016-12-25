using System;
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

        public ActionResult Person(string id)
        {
            if (id != null)
            {
                int ID = 0;
                try
                {
                    ID = Convert.ToInt32(id);
                }
                catch
                {
                    throw new ArgumentException("Person ID is not convertable to valid type");
                }

                PersonData data = mappingController.ConstructPersonData(ID);
                return View("Person", data);
            }
            return View("PersonList", mappingController.personListData);
        }

        public ActionResult PersonList()
        {
            return RedirectToAction("Person");
        }

        public ActionResult Order(string id)
        {
            if (id != null)
            {
                int ID = 0;
                try
                {
                    ID = Convert.ToInt32(id);
                }
                catch
                {
                    throw new ArgumentException("Order ID is not convertable to valid type");
                }

                OrderData data = mappingController.ConstructOrderData(ID);
                return View("Order", data);
            }
            return View("OrderList", mappingController.orderListData);
        }

        public ActionResult OrderList()
        {
            return RedirectToAction("Order");
        }

        public ActionResult Main()
        {
            MainData data = new MainData();
            return View("Main", data);
        }

        public ActionResult Actions()
        {
            ActionData data = new ActionData();
            return View("Actions", data);
        }

        [HttpPost]
        public ActionResult SaveButtonAvia(int OrderID, int id, string Status)
        {
            Order order = mappingController.orderListData.orderList.Find(item => item.ID == OrderID);
            if (order != null)
            {
                Invoice invoice = order.invoiceList.Find((item) => { return item.ID == id; });
                if (invoice != null)
                {
                    invoice.Status = Status;
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SaveOrderInformation(int OrderID, string Description, string DescriptionWide, string Keys)
        {
            Order order = mappingController.orderListData.orderList.Find(item => item.ID == OrderID);
            if (order != null)
            {
                order.Description = Description;
                order.DescriptionWide = DescriptionWide;
                order.Keys = Keys;
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult GetOrderInformation(int OrderID)
        {
            Order order = mappingController.orderListData.orderList.Find(item => item.ID == OrderID);
            return Json(order);
        }

        [HttpPost]
        public ActionResult UpdateDescriptionWide(int OrderID, string val)
        {
            Order order = mappingController.orderListData.orderList.Find(item => item.ID == OrderID);
            if (order != null)
            {
                order.DescriptionWide = val;
            }
            return Json(val);
        }

        [HttpPost]
        public ActionResult UpdateKeys(int OrderID, string val)
        {
            Order order = mappingController.orderListData.orderList.Find(item => item.ID == OrderID);
            if (order != null)
            {
                order.Keys = val;
            }
            return Json(val);
        }

        [HttpPost]
        public ActionResult FindIncoice(int OrderID, int id)
        {
            Order order = mappingController.orderListData.orderList.Find(item => item.ID == OrderID);
            if (order != null)
            {
                Invoice invoice = order.invoiceList.Find((item) => { return item.ID == id; });
                return Json(invoice);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult FindPassport(int PersonID, int id)
        {
            Person person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                Passport passport = person.PassportList.Find((item) => { return item.ID == id; });
                return Json(passport);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult GetPersonalData(int PersonID)
        {
            Person person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            return Json(person);
        }

        [HttpPost]
        public ActionResult FindContact(int PersonID, int id)
        {
            Person person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                Contact contact = person.ContactList.Find((item) => { return item.ID == id; });
                return Json(contact);
            }
            else
            {
                return Json("");
            }
        }

        public ActionResult FindViza(int PersonID, int id, string PassportSerial)
        {
            Viza viza = null;
            Person person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                Passport passport = person.PassportList.Find((item) => { return item.SerialNumber == PassportSerial; });
                if (passport != null)
                    viza = passport.VizaList.Find((item) => { return item.ID == id; });
            }
            return Json(viza);
        }
    }
}