using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;

namespace TravelController.Domain
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