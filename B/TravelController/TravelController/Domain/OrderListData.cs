using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;

namespace TravelController.Domain
{
    public class OrderListData : PageData
    {
        public OrderList orderList { get; private set; }

        public OrderListData()
        {
            orderList = new OrderList();
        }
    }
}