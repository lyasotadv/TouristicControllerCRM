using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
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