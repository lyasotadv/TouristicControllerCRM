using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelController.Models
{
    public class Money
    {
        private int _Val;

        public int Val
        {
            get 
            { 
                return _Val;
            }
            set
            {
                if (value >= 0)
                    _Val = value;
                else
                    throw new ArgumentException("Cost cannot be negative");
            }
        }

        public string ValStr
        {
            get
            {
                return ToString();
            }
        }

        public Money()
        {
            Val = 0;
        }

        public override string ToString()
        {
            return @"$" + Val.ToString();
        }
    }
}