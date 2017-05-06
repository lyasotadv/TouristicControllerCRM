using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;
using sb_admin_2.Web1.Domain;
using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models.Mapping
{
    public class MappingController
    {
        public OrderListData orderListData { get; set; }

        public PersonListData personListData { get; set; }

        public SettingsData settingsData { get; set; }

        public MappingController()
        {
            Catalog catalog = new Catalog();

            orderListData = new OrderListData();
            personListData = new PersonListData();
            settingsData = new SettingsData();

            orderListData.catalog = catalog;
            personListData.catalog = catalog;
            settingsData.catalog = catalog;

            personListData.personList.Load();

            settingsData.aviaCompanyList.Load();
            settingsData.aviaCompanyUnionList.Load();
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

        public PageData ConstructPersonData(int ID)
        {
            PersonGeneral person = personListData.personList.Find(item => item.ID == ID);
            if (person != null)
            {
                person.Load();
                if (person is Person)
                {
                    PersonData data = new PersonData(person as Person);
                    data.catalog = personListData.catalog;
                    return data;
                }
                else if (person is Company)
                {
                    CompanyData data = new CompanyData(person as Company);
                    data.catalog = personListData.catalog;
                    return data;
                }
                else
                {
                    throw new NotImplementedException("Unhandled person type");
                }
            }
            else
            {
                throw new ArgumentException("Person with current id can not be found");
            }
        }

        public int CreatePerson()
        {
            Person person = Person.CreatePerson();
            person.Save();
            return person.ID;
        }

        public int CreateCompany()
        {
            Company company = Company.Create();
            company.Save();
            return company.ID;
        }
    }
}