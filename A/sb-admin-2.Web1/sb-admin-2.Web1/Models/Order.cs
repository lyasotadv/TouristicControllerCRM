using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class OrderList : ModelList<Order>
    {
        
    }

    public class Order : IDebugModel
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

        public void CreateTestData()
        {
            Description = "Default";
            DescriptionWide = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
            Status = "active";
            LastUpdate = DateTime.Now;
            Keys = "lorem, ipsum";
        }
    }
}