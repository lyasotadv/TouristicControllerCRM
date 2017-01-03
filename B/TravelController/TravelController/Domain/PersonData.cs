using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;

namespace TravelController.Domain
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