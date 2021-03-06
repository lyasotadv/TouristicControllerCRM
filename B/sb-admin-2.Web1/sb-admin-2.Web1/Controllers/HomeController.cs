﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Net;

using sb_admin_2.Web1.Domain;
using sb_admin_2.Web1.Models.Mapping;
using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Controllers
{
    public class HomeController : Controller
    {
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
                if (data != null)
                {
                    string hash = string.Empty;
                    if (data.Check(out hash))
                    {
                        data.Load();
                        Session["UserID"] = data.ID;
                        Session["UserName"] = data.Name;
                        Session["UserRole"] = data.role.RoleString;
                        Session.Timeout = 240;
                        controllerUtils.DataLog("User " + data.Name + " loged as " + data.role.RoleString + " with hash = " + hash + ": success");
                        return RedirectToAction("PersonList");
                    }
                    else
                    {
                        controllerUtils.DataLog("User " + data.Name + " loged as " + data.role.RoleString + " with hash = " + hash + ": fail");
                    }
                }
            }
            return View("Login", data);
        }

        public ActionResult UserProfile()
        {
            if (Session["UserID"] != null)
            {
                LoginData data = new LoginData();
                data.ID = Convert.ToInt32(Session["UserID"]);
                data.Name = Convert.ToString(Session["UserName"]);
                data.role.RoleString = Convert.ToString(Session["UserRole"]);
                data.NameNew = data.Name;
                data.Load();
                return View("UserProfile", data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult UserProfile(LoginData data)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (data != null)
                    {
                        data.ID = Convert.ToInt32(Session["UserID"]);
                        data.Name = Convert.ToString(Session["UserName"]);
                        data.role.RoleString = Convert.ToString(Session["UserRole"]);
                        data.Save();
                    }
                }
                return View("UserProfile", data); 
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult AddNewUser(string FullName, string Name, bool IsAdmin)
        {
            if (Session["UserID"] != null)
            {
                LoginData data = new LoginData();
                data.ID = Convert.ToInt32(Session["UserID"]);
                data.Name = Convert.ToString(Session["UserName"]);
                data.role.RoleString = Convert.ToString(Session["UserRole"]);
                data.Load();
                data.AddNewUser(FullName, Name, IsAdmin);
            }
            return Json("");
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

        private ControllerUtils _controllerUtils;

        private ControllerUtils controllerUtils
        {
            get
            {
                if (_controllerUtils == null)
                {
                    _controllerUtils = new ControllerUtils();
                    _controllerUtils.Server = Server;
                }
                return _controllerUtils;
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

                    try
                    {
                        PageData data = mappingController.ConstructPersonData(ID);
                        if (data is PersonData)
                            return View("Person", data);
                        else if (data is CompanyData)
                            return View("Company", data);
                        else
                            throw new NotImplementedException("View for current person type have not been created");
                    }
                    catch
                    {
                        return RedirectToAction("PersonList");
                    }
                }
                return RedirectToAction("PersonList");
            }
            else
            {
                controllerUtils.DataLog("Session is failed");
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
        public ActionResult FindViza(int PersonID, int ID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                person.Load();
                Viza viza = (person as Person).PassportList.FindViza(item => item.ID == ID);
                if (viza != null)
                {
                    viza.Load();
                    return Json(viza);
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SaveVizaDetails(int PersonID, int ID, 
            string DateApprovedStr, string ValidFromStr, string ValidTillStr, 
            string TargetName, string CountryOfEmmitationName,
            string Number, string VizaType,
            int EntriesNumber, int DaysCount, int DaysUsed,
            string Description, string PassportSerial)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                person.Load();
                Passport passport = (person as Person).PassportList.Find(item => item.SerialNumber == PassportSerial);
                if (passport != null)
                {
                    Viza viza = (person as Person).PassportList.FindViza(item => item.ID == ID);
                    if (viza == null)
                    {
                        viza = passport.vizaList.Create();
                    }

                    viza.PassportID = passport.ID;

                    viza.DateApprovedStr = DateApprovedStr;
                    viza.ValidFromStr = ValidFromStr;
                    viza.ValidTillStr = ValidTillStr;

                    viza.TargetName = TargetName;
                    Country countryOfEmmitation = mappingController.personListData.catalog.countryList.Find(item => item.Name == CountryOfEmmitationName);
                    if (countryOfEmmitation != null)
                    {
                        viza.CountryOfEmmitationID = countryOfEmmitation.ID;
                    }

                    viza.Number = Number;
                    viza.VizaType = VizaType;
                    viza.EntriesNumber = EntriesNumber;
                    viza.DaysCount = DaysCount;
                    viza.DaysUsed = DaysUsed;
                    viza.Description = Description;

                    viza.Save();
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteViza(int PersonID, int ID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (person is Person))
            {
                person.Load();
                Viza viza = (person as Person).PassportList.FindViza(item => item.ID == ID);
                if (viza != null)
                {
                    viza.Delete();
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
                    viza = passport.vizaList.Find((item) => { return item.ID == id; });
            }
            return Json(viza);
        }

        [HttpPost]
        public ActionResult FindCountry(int id)
        {
            Country country = mappingController.settingsData.catalog.countryList.Find(item => item.ID == id);
            if (country != null)
            {
                country.Load();
            }
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
        public ActionResult FindCountryUnion(int id)
        {
            CountryUnion union = mappingController.settingsData.catalog.countryUnionList.Find(item => item.ID == id);
            if (union != null)
            {
                union.Load();
            }
            return Json(union);
        }

        [HttpPost]
        public ActionResult SaveCountryUnion(int id, string Name, string ShortName, string Note)
        {
            CountryUnion union = mappingController.settingsData.catalog.countryUnionList.Find(item => item.ID == id);
            
            if (union == null)
            {
                union = mappingController.settingsData.catalog.countryUnionList.Create();
            }

            union.Name = Name;
            union.ShortName = ShortName;
            union.Note = Note;
            union.Save();

            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteCountryUnion(int id)
        {
            CountryUnion union = mappingController.settingsData.catalog.countryUnionList.Find(item => item.ID == id);
            if (union != null)
            {
                union.Delete();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult FindAviacompany(int id)
        {
            AviaCompany ac = mappingController.settingsData.aviaCompanyList.Find(item => item.ID == id);
            ac.Load();
            return Json(ac);
        }

        [HttpPost]
        public ActionResult SaveAviacompany(int id, string Name, string ICAO, string Note)
        {
            AviaCompany ac = mappingController.settingsData.aviaCompanyList.Find(item => item.ID == id);
            
            if (ac == null)
            {
                ac = mappingController.settingsData.aviaCompanyList.Create(); 
            }

            ac.FullName = Name;
            ac.ICAO = ICAO;
            ac.Description = Note;

            ac.Save();

            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteAviacompany(int id)
        {
            AviaCompany ac = mappingController.settingsData.aviaCompanyList.Find(item => item.ID == id);
            if (ac != null)
            {
                ac.Delete();
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult FindAviacompanyUnion(int id)
        {
            AviaCompanyUnion acu = mappingController.settingsData.aviaCompanyUnionList.Find(item => item.ID == id);
            acu.Load();
            return Json(acu);
        }

        [HttpPost]
        public ActionResult SaveAviacompanyUnion(int id, string Name, string Note)
        {
            AviaCompanyUnion acu = mappingController.settingsData.aviaCompanyUnionList.Find(item => item.ID == id);

            if (acu == null)
            {
                acu = mappingController.settingsData.aviaCompanyUnionList.Create();
            }

            acu.Name = Name;
            acu.Note = Note;

            acu.Save();

            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteAviacompanyUnion(int id)
        {
            AviaCompanyUnion acu = mappingController.settingsData.aviaCompanyUnionList.Find(item => item.ID == id);
            if (acu != null)
            {
                acu.Delete();
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult JoinAviaCompanyUnion(int idAC, string nameACU, bool status)
        {
            AviaCompany ac = mappingController.settingsData.aviaCompanyList.Find(item => item.ID == idAC);
            AviaCompanyUnion acu = mappingController.settingsData.aviaCompanyUnionList.Find(item => item.Name == nameACU);

            if ((ac != null) & (acu != null))
            {
                ac.Load();
                acu.Load();

                if (status)
                {
                    if (ac.aviaCompanyUnionList.Find(item => item.ID == acu.ID) == null)
                    {
                        ac.aviaCompanyUnionList.AddElement(ac, acu);
                    }
                }
                else
                {
                    if (ac.aviaCompanyUnionList.Find(item => item.ID == acu.ID) != null)
                    {
                        ac.aviaCompanyUnionList.RemoveElement(ac, acu);
                    }
                }

                ac.Load();
                acu.Load();
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult JoinCountryUnion(int idCountry, string nameUnion, bool status)
        {
            Country country = mappingController.settingsData.catalog.countryList.Find(item => item.ID == idCountry);
            CountryUnion union = mappingController.settingsData.catalog.countryUnionList.Find(item => item.Name == nameUnion);

            if ((country != null) & (union != null))
            {
                country.Load();
                union.Load();

                if (status)
                {
                    if (country.UnionList.Find(item => item.ID == union.ID) == null)
                    {
                        country.UnionList.AddElement(country, union);
                    }
                }
                else
                {
                    if (country.UnionList.Find(item => item.ID == union.ID) != null)
                    {
                        country.UnionList.RemoveElement(country, union);
                    }
                }

                country.Load();
                union.Load();
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult FindMileCardStatus(int id)
        {
            MileCardStatus mcs = mappingController.settingsData.mileCardStatusList.Find(item => item.ID == id);
            mcs.Load();
            return Json(mcs);
        }

        [HttpPost]
        public ActionResult SaveMileCardStatus(int id, string Name, int MinVal, int MaxVal, string nameAC, string nameACU, string Note)
        {
            MileCardStatus mcs = mappingController.settingsData.mileCardStatusList.Find(item => item.ID == id);
            if (mcs == null)
            {
                mcs = mappingController.settingsData.mileCardStatusList.Create();
            }

            mcs.Name = Name;
            mcs.MinVal = MinVal;
            mcs.MaxVal = MaxVal;
            mcs.Note = Note;

            AviaCompany ac = mappingController.settingsData.aviaCompanyList.Find(item => item.FullName == nameAC);
            if (ac != null)
            {
                mcs.AviaCompanyID = ac.ID;
            }
            else
            {
                mcs.AviaCompanyID = -1;
            }

            AviaCompanyUnion acu = mappingController.settingsData.aviaCompanyUnionList.Find(item => item.Name == nameACU);
            if (acu != null)
            {
                mcs.AviaCompanyUnionID = acu.ID;
            }
            else
            {
                mcs.AviaCompanyUnionID = -1;
            }

            mcs.Save();

            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteMileCardStatus(int id)
        {
            MileCardStatus mcs = mappingController.settingsData.mileCardStatusList.Find(item => item.ID == id);
            if (mcs != null)
            {
                mcs.Delete();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult FindMileCard(int PersonID, int id)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.mileCardList.Load();
                MileCard mc = person.mileCardList.Find(item => item.ID == id);
                if (mc != null)
                {
                    mc.Load();
                    return Json(mc);
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SaveMileCard(int PersonID, int id, string Number, string Password, string nameACU, string nameAC, string nameResondedPerson, 
                                            string note, int MilesCount)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.mileCardList.Load();
                MileCard mc = person.mileCardList.Find(item => item.ID == id);
                if (mc == null)
                {
                    mc = person.mileCardList.Create();
                }

                mc.Number = Number;
                mc.Password = Password;
                mc.Note = note;
                mc.MilesCount = MilesCount;

                mappingController.settingsData.aviaCompanyUnionList.Load();
                AviaCompanyUnion acu = mappingController.settingsData.aviaCompanyUnionList.Find(item => item.Name == nameACU);
                if (acu != null)
                {
                    mc.AviaCompanyUnionID = acu.ID;
                }

                mappingController.settingsData.aviaCompanyList.Load();
                AviaCompany ac = mappingController.settingsData.aviaCompanyList.Find(item => item.FullName == nameAC);
                if (ac != null)
                {
                    mc.AviaCompanyID = ac.ID;
                }

                PersonGeneral personResponded = mappingController.personListData.personList.Find(item => item.FullName == nameResondedPerson);
                if (personResponded != null)
                {
                    mc.PersonRespondedID = personResponded.ID;
                }

                mc.Save();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteMileCard(int PersonID, int id)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.mileCardList.Load();
                MileCard mc = person.mileCardList.Find(item => item.ID == id);
                if (mc != null)
                {
                    mc.Delete();
                }
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

        

        [HttpPost]
        public ActionResult UploadCountry(HttpPostedFileBase file)
        {
            controllerUtils.UploadFileAndParse(file, mappingController.settingsData.catalog.countryList.Export);

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            List<AttachFileTransferData> data = new List<AttachFileTransferData>();
            for (int n = 0; n < Request.Files.Count; n++ )
            {
                AttachFileTransferData atd = controllerUtils.UploadFileAndSave(Request.Files[n]);
                if (atd != null)
                    data.Add(atd);
            }
            return Json(data);
        }

        [HttpPost]
        public ActionResult UploadFileToPerson(int PersonID, List<AttachFileTransferData> data)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if ((person != null) && (data != null))
            {
                person.attachFileList.Load();
                foreach(var item in data)
                {
                    AttachFilePerson atp = person.attachFileList.Create();
                    item.FillData(atp);
                    atp.Save();
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult FindFilePerson(int PersonID, int FileID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.attachFileList.Load();
                AttachFilePerson atp = person.attachFileList.Find(item => item.ID == FileID);
                return Json(atp);
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult DeleteFilePerson(int PersonID, int FileID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.attachFileList.Load();
                AttachFilePerson atp = person.attachFileList.Find(item => item.ID == FileID);
                if (atp != null)
                {
                    atp.Delete();
                    controllerUtils.DeleteFile(atp.guid);
                }
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult SaveFilePerson(int PersonID, int FileID, string Description)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.attachFileList.Load();
                AttachFilePerson atp = person.attachFileList.Find(item => item.ID == FileID);
                if (atp != null)
                {
                    atp.Description = Description;
                    atp.Save();
                }
            }
            return Json("");
        }

        public FileResult DownloadFile(int PersonID, int FileID)
        {
            PersonGeneral person = mappingController.personListData.personList.Find(item => item.ID == PersonID);
            if (person != null)
            {
                person.attachFileList.Load();
                AttachFilePerson atp = person.attachFileList.Find(item => item.ID == FileID);
                if (atp != null)
                {
                    byte[] filebytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/App_Data/uploads/data"), atp.guid));
                    return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.ChangeExtension(atp.Name, atp.Extension));
                }
            }
            return null;
        }
    }
}