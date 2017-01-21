using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class OrderList : List<Order>
    {
        
    }

    public class Order
    {
        private enum OrderStatus { active, closed };

        private OrderStatus _status;

        public string Description { get; set; }

        public string DescriptionWide { get; set; }

        public string Status
        {
            get
            {
                switch (_status)
                {
                    case OrderStatus.active:
                        {
                            return "active";
                        }
                    case OrderStatus.closed:
                        {
                            return "closed";
                        }
                    default:
                        {
                            throw new ArgumentException("Unhandled order status");
                        }
                }
            }
            set
            {
                switch (value)
                {
                    case "active":
                        {
                            _status = OrderStatus.active;
                            break;
                        }
                    case "closed":
                        {
                            _status = OrderStatus.closed;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Incorrect order status");
                        }
                }
            }
        }

        public DateTime LastUpdate { get; set; }

        public string Keys { get; set; }

        public InvoiceList invoiceList { get; private set; }

        public int ID { get; set; }

        public Order()
        {
            invoiceList = new InvoiceList();
        }
    }
}