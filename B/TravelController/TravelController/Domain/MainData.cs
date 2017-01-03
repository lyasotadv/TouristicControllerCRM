using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;

namespace TravelController.Domain
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