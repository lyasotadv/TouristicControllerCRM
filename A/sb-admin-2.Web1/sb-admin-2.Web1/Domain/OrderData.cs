using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class OrderData
    {
        public OrderData()
        {
            Order = new Order();
            PersonList = new PersonList();
            InvoiceList = new InvoiceList();
            ActionList = new ActionHistoryList();
        }

        public Order Order { get; private set; }

        public PersonList PersonList { get; private set; }

        public InvoiceList InvoiceList { get; private set; }

        public ActionHistoryList ActionList { get; private set; }
    }
}