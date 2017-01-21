using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class MainData
    {
        public MainData()
        {
            PersonList = new PersonList();
            OrderList = new OrderList();
        }

        public PersonList PersonList { get; private set; }

        public OrderList OrderList { get; private set; }
    }
}