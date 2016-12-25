using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class PersonData : PageData
    {
        public PersonData(Person person)
        {
            this.person = person;
        }

        public Person person { get; private set; }
    }
}