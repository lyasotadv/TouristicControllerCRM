using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class OrderData : PageData
    {
        public OrderData(Order order)
        {
            this.order = order;
        }

        public Order order { get; private set; }
    }
}