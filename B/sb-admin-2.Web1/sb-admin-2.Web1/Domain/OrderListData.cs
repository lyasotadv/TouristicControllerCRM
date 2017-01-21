using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
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