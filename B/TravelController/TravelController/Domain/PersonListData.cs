using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;

namespace TravelController.Domain
{
    public class PersonListData : PageData
    {
        public PersonList personList { get; private set; }

        public PersonListData()
        {
            personList = new PersonList();
        }
    }
}