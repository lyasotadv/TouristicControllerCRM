using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;
using TravelController.Domain;
using TravelController.Models.Mapping.DBUtils;

namespace TravelController.Models.Mapping
{
    public class MappingController
    {
        public OrderListData orderListData { get; set; }

        public PersonListData personListData { get; set; }

        public MappingController()
        {
            Catalog catalog = new Catalog();

            orderListData = new OrderListData();
            personListData = new PersonListData();

            orderListData.catalog = catalog;
            personListData.catalog = catalog;

            TestDataInit();

            personListData.personList.Add(new Person() { ID = 2 });
            orderListData.orderList.Add(new Order() { ID = 2 });
        }

        public OrderData ConstructOrderData(int ID)
        {
            Order order = orderListData.orderList.Find(item => item.ID == ID);
            if (order != null)
            {
                OrderData data = new OrderData(order);
                data.catalog = orderListData.catalog;
                return data;
            }
            else
            {
                throw new ArgumentException("Order with current id can not be found");
            }
        }

        public PersonData ConstructPersonData(int ID)
        {
            Person person = personListData.personList.Find(item => item.ID == ID);
            if (person != null)
            {
                PersonData data = new PersonData(person);
                data.catalog = personListData.catalog;
                return data;
            }
            else
            {
                throw new ArgumentException("Person with current id can not be found");
            }
        }

        private void TestDataInit()
        {
            Order order = new Order();
            orderListData.orderList.Add(order);

            order.Keys = "lorem, ipsum";
            order.Description = "Default";
            order.DescriptionWide = "Lorem ipsum bla-bla";
            order.Status = "active";
            order.ID = 1;

            TestDataInitInvoice(order);
            TestDataInitPerson();
        }

        private void TestDataInitInvoice(Order order)
        {
            Person ivan = new Person();
            ivan.FirstName = "Ivan";
            ivan.SecondName = "Ivanov";
            ivan.ID = 3;

            Person anna = new Person();
            anna.FirstName = "Anna";
            anna.SecondName = "Annova";
            anna.ID = 4;

            //1
            AviaTicketInvoice invoice1 = new AviaTicketInvoice();
            invoice1.ID = 1;
            invoice1.Client = ivan;
            invoice1.ResponsiblePerson = ivan;
            invoice1.StartPoint = "Kiev";
            invoice1.FinalDestination = "Paris";
            invoice1.Departure = new DateTime(2016, 12, 27);
            invoice1.DepartureBack = new DateTime(2017, 01, 05);
            invoice1.Amount.Val = 170;
            invoice1.Status = "Offer";
            invoice1.ServiceProvider = "KLM";
            invoice1.IsRoundTrip = true;
            order.invoiceList.Add(invoice1);

            //2
            AviaTicketInvoice invoice2 = new AviaTicketInvoice();
            invoice2.ID = 2;
            invoice2.Client = anna;
            invoice2.ResponsiblePerson = ivan;
            invoice2.StartPoint = "Kiev";
            invoice2.FinalDestination = "Paris";
            invoice2.Departure = new DateTime(2016, 12, 27);
            invoice2.DepartureBack = new DateTime(2017, 01, 05);
            invoice2.Amount.Val = 170;
            invoice2.Status = "Offer";
            invoice2.ServiceProvider = "KLM";
            order.invoiceList.Add(invoice2);

            //3
            AviaTicketInvoice invoice3 = new AviaTicketInvoice();
            invoice3.ID = 3;
            invoice3.Client = ivan;
            invoice3.ResponsiblePerson = ivan;
            invoice3.StartPoint = "Kiev";
            invoice3.FinalDestination = "Paris";
            invoice3.Departure = new DateTime(2016, 12, 28);
            invoice3.DepartureBack = new DateTime(2017, 01, 06);
            invoice3.Amount.Val = 185;
            invoice3.Status = "Offer";
            invoice3.ServiceProvider = "KLM";
            order.invoiceList.Add(invoice3);

            //4
            AviaTicketInvoice invoice4 = new AviaTicketInvoice();
            invoice4.ID = 4;
            invoice4.Client = anna;
            invoice4.ResponsiblePerson = ivan;
            invoice4.StartPoint = "Kiev";
            invoice4.FinalDestination = "Paris";
            invoice4.Departure = new DateTime(2016, 12, 28);
            invoice4.DepartureBack = new DateTime(2017, 01, 06);
            invoice4.Amount.Val = 185;
            invoice4.Status = "Offer";
            invoice4.ServiceProvider = "KLM";
            order.invoiceList.Add(invoice4);

            //5
            AviaTicketInvoice invoice5 = new AviaTicketInvoice();
            invoice5.ID = 5;
            invoice5.Client = ivan;
            invoice5.ResponsiblePerson = ivan;
            invoice5.StartPoint = "Kiev";
            invoice5.FinalDestination = "Berlin";
            invoice5.Departure = new DateTime(2016, 12, 27);
            invoice5.DepartureBack = new DateTime(2017, 01, 05);
            invoice5.Amount.Val = 180;
            invoice5.Status = "Booked";
            invoice5.ServiceProvider = "KLM";
            order.invoiceList.Add(invoice5);

            //6
            AviaTicketInvoice invoice6 = new AviaTicketInvoice();
            invoice6.ID = 6;
            invoice6.Client = anna;
            invoice6.ResponsiblePerson = ivan;
            invoice6.StartPoint = "Kiev";
            invoice6.FinalDestination = "Berlin";
            invoice6.Departure = new DateTime(2016, 12, 27);
            invoice6.DepartureBack = new DateTime(2017, 01, 05);
            invoice6.Amount.Val = 180;
            invoice6.Status = "Booked";
            invoice6.ServiceProvider = "KLM";
            order.invoiceList.Add(invoice6);

            ////7
            //Invoice invoice7 = new InsuranceInvoice();
            //invoice7.ID = 7;
            //invoice7.Client = ivan;
            //invoice7.ResponsiblePerson = anna;
            //invoice7.Path = "EU";
            //invoice7.Date = "25dec16 - 10jan17";
            //invoice7.Amount.Val = 15;
            //invoice7.Status = "Sold";
            //invoice7.ServiceProvider = "PZU";
            //orderData.InvoiceList.Add(invoice7);


            ////8
            //Invoice invoice8 = new InsuranceInvoice();
            //invoice8.ID = 8;
            //invoice8.Client = anna;
            //invoice8.ResponsiblePerson = anna;
            //invoice8.Path = "EU";
            //invoice8.Date = "25dec16 - 10jan17";
            //invoice8.Amount.Val = 15;
            //invoice8.Status = "Sold";
            //invoice8.ServiceProvider = "PZU";
            //orderData.InvoiceList.Add(invoice8);

            ////9
            //Invoice invoice9 = new HotelInvoice();
            //invoice9.ID = 9;
            //invoice9.Client = ivan;
            //invoice9.ResponsiblePerson = ivan;
            //invoice9.Path = "Berlin";
            //invoice9.Date = "28dec16 - 04jan17";
            //invoice9.Amount.Val = 900;
            //invoice9.Status = "Sold";
            //invoice9.ServiceProvider = "Verona hotel";
            //orderData.InvoiceList.Add(invoice9);

            ////10
            //Invoice invoice10 = new HotelInvoice();
            //invoice10.ID = 10;
            //invoice10.Client = anna;
            //invoice10.ResponsiblePerson = ivan;
            //invoice10.Path = "Berlin";
            //invoice10.Date = "28dec16 - 04jan17";
            //invoice10.Amount.Val = 900;
            //invoice10.Status = "Sold";
            //invoice10.ServiceProvider = "Verona hotel";
            //orderData.InvoiceList.Add(invoice10);
        }

        private void TestDataInitPerson()
        {
            Person person = new Person();
            personListData.personList.Add(person);

            person.FirstName = "Ivan";
            person.SecondName = "Ivanov";
            person.MiddleName = "Ibn";

            person.Birth = new DateTime(1980, 07, 25);
            person.Gender = "male";
            person.Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            person.ID = 1;

            Country ukr = new Country() { Name = "Ukraine", ISO = "UA" };
            Country eng = new Country() { Name = "United Kingdom", ISO = "GB" };
            Country usa = new Country() { Name = "USA", ISO = "US" };
            Country eu = new Country() { Name = "EU", ISO = "EU" };
            Country uae = new Country() { Name = "UAE", ISO = "AE" };

            person.Citizen = ukr;

            Passport passportA = new Passport();
            passportA.CountryOfEmmitation = ukr;
            passportA.SerialNumber = "XX000000";
            passportA.ValidTill = new DateTime(2020, 05, 30);
            passportA.Description = "First passport";
            passportA.ID = 1;
            passportA.PersonName = "Ivanov Ivan";
            passportA.Citizen = ukr;

            Passport passportB = new Passport();
            passportB.CountryOfEmmitation = ukr;
            passportB.SerialNumber = "YY555555";
            passportB.ValidTill = new DateTime(2025, 10, 10);
            passportB.Description = "Second passport";
            passportB.ID = 2;
            passportB.PersonName = "Ivanov Ivan";
            passportB.Citizen = ukr;


            passportA.AddViza(eng, new DateTime(2018, 05, 10), 1);
            passportA.AddViza(usa, new DateTime(2024, 12, 20), 2);
            passportA.AddViza(uae, new DateTime(2017, 02, 08), 3);
            passportB.AddViza(eu, new DateTime(2017, 08, 13), 4);

            person.PassportList.Add(passportA);
            person.PassportList.Add(passportB);

            Contact contactA = Contact.Create("e-mail");
            contactA.Description = "Personal";
            contactA.Content = "blablabla@bla.com";
            contactA.ID = 1;

            Contact contactB = Contact.Create("mobile");
            contactB.Description = "Mobile";
            contactB.Content = "+380000000000";
            contactB.ID = 2;

            person.ContactList.Add(contactA);
            person.ContactList.Add(contactB);
        }
    }
}