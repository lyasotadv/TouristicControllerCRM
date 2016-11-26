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
            ActionList = new ActionHistoryList();
            ContactList = new ContactList();
            DocumentList = new DocumentList();
        }

        public Person Person { get; private set; }

        public ContactList ContactList { get; private set; }

        public DocumentList DocumentList { get; private set; }

        public ActionHistoryList ActionList { get; private set; }
    }
}