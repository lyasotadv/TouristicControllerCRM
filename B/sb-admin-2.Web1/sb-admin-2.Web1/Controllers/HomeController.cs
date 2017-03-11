using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;

using sb_admin_2.Web1.Domain;
using sb_admin_2.Web1.Models.Mapping;
using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Controllers
{
    public class HomeController : Controller
    {
        //[HttpGet]
        public ActionResult Login()
        {
            LoginData data = new LoginData();
            return View("Login", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginData data)
        {
            if (ModelState.IsValid)
            {
                if ((data != null) && (data.Check()))
                {
                    Session["UserID"] = data.ID;
                    Session["UserName"] = data.Name;
                    return RedirectToAction("PersonList");
                }
            }
            return View("Login", data);
        }

        private MappingController _mappingController;

        private MappingController mappingController 
        { 
            get
            {
                if (_mappingController == null)
                {
                    _mappingController = new MappingController();
                }
                return _mappingController;
            }
        }

        public ActionResult Person(string id)
        {
            if (Session["UserID"] != null)
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

                    PageData data = mappingController.ConstructPersonData(ID);
                    if (data is PersonData)
                        return View("Person", data);
                    else if (data is CompanyData)
                        return View("Company", data);
                    else
                        throw new NotImplementedException("View for current person type have not been created");
                }
                return RedirectToAction("PersonList");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Company()
        {
            if (Session["UserID"] != null)
            {
                return View("PersonList", mappingController.personListData);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult PersonList()
        {
            if (Session["UserID"] != null)
            {
                return View("PersonList", mappingController.personListData);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Order(string id)
        {
            if (Session["UserID"] != null)
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
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult OrderList()
        {
            if (Session["UserID"] != null)
            {
                return View("OrderList", mappingController.orderListData);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Main()
        {
            MainData data = new MainData();
            return View("Main", data);
        }

        public ActionResult Settings()
        {
            if (Session["UserID"] != null)
            {
                return View("Settings", mappingController.settingsData);
            }
            else
            {
                return RedirectToAction("Login");
            }
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
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                person.Load();
                Passport passport = (person as Person).PassportList.Find((item) => { return item.ID == id; });
                return Json(passport);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult SavePassport(int PersonID, int id, string passportSerial, string expireDate,
            string personName, string personSurname, string countryOfEmitation, string countryOfCitizen, string description)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                person.Load();
                Passport passport = (person as Person).PassportList.Find((item) => { return item.ID == id; });
                if (passport == null)
                {
                    passport = (person as Person).PassportList.Create();
                }
                else
                {
                    passport.Load();
                }

                try
                {
                    passport.SerialNumber = passportSerial;
                    passport.ValidTillStr = expireDate;
                    passport.PersonName = personName;
                    passport.PersonSurname = personSurname;
                    passport.Description = description;

                    mappingController.settingsData.catalog.countryList.Load();
                    passport.CountryOfEmmitation = mappingController.settingsData.catalog.countryList.Find(item => item.Name == countryOfEmitation);
                    passport.Citizen = mappingController.settingsData.catalog.countryList.Find(item => item.Name == countryOfCitizen);
                }
                catch (FormatException e)
                {
                    return Json(e.Message);
                }
                catch (ArgumentNullException e)
                {
                    return Json(e.Message);
                }
                
                passport.Save();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult DeletePassport(int PersonID, int id)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                person.Load();
                Passport passport = (person as Person).PassportList.Find((item) => { return item.ID == id; });
                if (passport != null)
                {
                    passport.Delete();
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult GetPersonalData(int PersonID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.Load();
                if (person is Person)
                    return Json(person as Person);
                else if (person is Company)
                    return Json(person as Company);
                else
                    return Json("");
            }
            else
                return Json("");
        }

        [HttpPost]
        public ActionResult SavePersonDetails(int PersonID, string FirstName, string MiddleName, string SecondName, string NameUA,
            string BirthDay, string Description, string Gender, string itn)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                try
                {
                    (person as Person).FirstName = FirstName;
                    (person as Person).MiddleName = MiddleName;
                    (person as Person).SecondName = SecondName;
                    (person as Person).FullNameUA = NameUA;
                    (person as Person).BirthStr = BirthDay;
                    (person as Person).Description = Description;
                    (person as Person).Gender = Gender;
                    (person as Person).itn = itn;
                }
                catch (FormatException e)
                {
                    return Json(e.Message);
                }

                person.Save();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult CreatePerson()
        {
            return Json(mappingController.CreatePerson());
        }

        [HttpPost]
        public ActionResult CreateCompany()
        {
            return Json(mappingController.CreateCompany());
        }

        [HttpPost]
        public ActionResult DeletePerson(int PersonID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.Delete();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SaveCompanyDetails(int PersonID, string Name, string Description)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Company))
            {
                try
                {
                    (person as Company).FullName = Name;
                    (person as Company).Description = Description;
                }
                catch (FormatException e)
                {
                    return Json(e.Message);
                }

                person.Save();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SavePaymentDetails(int PersonID, string MFO, string EDRPOU, string Account, string BankName)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Company))
            {
                try
                {
                    (person as Company).MFO = MFO;
                    (person as Company).EDRPOU = EDRPOU;
                    (person as Company).Account = Account;
                    (person as Company).BankName = BankName;
                }
                catch (FormatException e)
                {
                    return Json(e.Message);
                }

                person.Save();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult FindContact(int PersonID, int id)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.Load();
                Contact contact = person.ContactList.Find((item) => { return item.ID == id; });
                if (contact != null)
                {
                    contact.Load();
                }
                return Json(contact);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public ActionResult SaveContact(int PersonID, int id, string content, string description, string contactType)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.Load();
                Contact contact = person.ContactList.Find((item) => { return item.ID == id; });
                if (contact == null)
                {
                    contact = person.ContactList.Create(contactType);
                }
                else
                {
                    contact.Load();
                }

                contact.Content = content;
                contact.Description = description;
                contact.Save();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteContact(int PersonID, int id)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.Load();
                Contact contact = person.ContactList.Find((item) => { return item.ID == id; });
                if (contact != null)
                {
                    contact.Delete();
                }
            }
            return Json("");
        }

        public ActionResult FindViza(int PersonID, int id, string PassportSerial)
        {
            Viza viza = null;
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                Passport passport = (person as Person).PassportList.Find((item) => { return item.SerialNumber == PassportSerial; });
                if (passport != null)
                    viza = passport.VizaList.Find((item) => { return item.ID == id; });
            }
            return Json(viza);
        }

        [HttpPost]
        public ActionResult FindCountry(int id)
        {
            Country country = mappingController.settingsData.catalog.countryList.Find(item => item.ID == id);
            return Json(country);
        }

        [HttpPost]
        public ActionResult SaveCountry(int id, string Name, string ISO, string ISO3, string Citizen)
        {
            Country country = mappingController.settingsData.catalog.countryList.Find(item => item.ID == id);
            if (country == null)
            {
                country = mappingController.settingsData.catalog.countryList.Create();
            }

            country.Name = Name;
            country.ISO = ISO;
            country.ISO3 = ISO3;
            country.Nationality = Citizen;
            country.Save();

            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteCountry(int id)
        {
            Country country = mappingController.settingsData.catalog.countryList.Find(item => item.ID == id);
            if (country != null)
            {
                country.Delete();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult FindLabel(int id)
        {
            Label label = mappingController.settingsData.catalog.labelList.Find(item => item.ID == id);
            return Json(label);
        }

        [HttpPost]
        public ActionResult SaveLabel(int id, string Name, string Comment, string ColorHTML, string ParentName)
        {
            Label label = mappingController.settingsData.catalog.labelList.Find(item => item.ID == id);
            if (label == null)
            {
                label = mappingController.settingsData.catalog.labelList.Create();
            }

            label.Name = Name;
            label.Comment = Comment;
            label.ColorHtml = ColorHTML;
            label.ParentName = ParentName;
            label.Save();

            return Json("");
        }

        public ActionResult DeleteLabel(int id)
        {
            Label label = mappingController.settingsData.catalog.labelList.Find(item => item.ID == id);
            if (label != null)
            {
                label.Delete();
            }
            return Json("");
        }

        public ActionResult AddLabelPerson(int PersonID, string Name, string Note)
        {
            Label label = mappingController.personListData.catalog.labelList.Find(item => item.Name == Name);
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((label != null) && (person != null))
            {
                person.labelList.Load();
                person.labelList.AttachLabelToPerson(label, Note);
            }
            return Json("");
        }

        public ActionResult DeleteLabelPerson(int PersonID, int LabelID)
        {
            Label label = mappingController.personListData.catalog.labelList.Find(item => item.ID == LabelID);
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((label != null) && (person != null))
            {
                person.labelList.Load();
                person.labelList.ReAttachLabelFromPerson(label);
            }
            return Json("");
        }

        public void UploadFileAndSave(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string fileFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/temp"), fileName);
                file.SaveAs(fileFullName);
                
                string newName = Guid.NewGuid().ToString();
                string newFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/data"), newName);
                System.IO.File.Move(fileFullName, newFullName);
            }
        }

        public void UploadFileAndParse(HttpPostedFileBase file, Func<string, bool> Parser)
        {
            if (file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string fileFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/temp"), fileName);
                file.SaveAs(fileFullName);
                
                try
                {
                    if (!Parser(fileFullName)) 
                        throw new FileLoadException("File structure is incorrect");
                }
                finally
                {
                    System.IO.File.Delete(fileFullName);
                }
            }
        }

        [HttpPost]
        public ActionResult UploadCountry(HttpPostedFileBase file)
        {
            UploadFileAndParse(file, mappingController.settingsData.catalog.countryList.Export);

            return RedirectToAction("Settings");
        }
    }
}