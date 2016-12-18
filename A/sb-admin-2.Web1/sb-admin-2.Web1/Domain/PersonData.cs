using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class PersonData
    {
        public PersonData()
        {
            Person = new Person();
        }

        public Person Person { get; private set; }
    }
}