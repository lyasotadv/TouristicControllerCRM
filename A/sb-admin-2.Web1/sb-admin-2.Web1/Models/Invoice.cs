using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class InvoiceList : ModelList<Invoice>
    {

    }

    public class Invoice : IDebugModel
    {
        private enum InvoiceStatus { active, closed };

        private InvoiceStatus _Status;

        public string Status
        {
            get
            {
                switch(_Status)
                {
                    case InvoiceStatus.active:
                        {
                            return "active";
                        }
                    case InvoiceStatus.closed:
                        {
                            return "closed";
                        }
                    default:
                        {
                            throw new ArgumentException("Unhandled invoce status");
                        }
                }
            }
            set
            {
                switch(value)
                {
                    case "active":
                        {
                            _Status = InvoiceStatus.active;
                            break;
                        }
                    case "closed":
                        {
                            _Status = InvoiceStatus.closed;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Incorrect invoice status");
                        }
                }
            }
        }

        public string Description { get; set; }

        public Money Amount { get; set; }

        public Invoice() 
        {
            Amount = new Money();
        }

        public void CreateTestData()
        {
            Description = "Lorem ipsum";
            Amount.Val = 100;
            _Status = InvoiceStatus.active;
        }
    }
}