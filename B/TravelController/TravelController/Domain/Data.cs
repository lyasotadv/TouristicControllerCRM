using TravelController.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelController.Domain
{
    public class Data
    {
        public IEnumerable<Navbar> navbarItems()
        {
            var menu = new List<Navbar>();
            
            menu.Add(new Navbar { Id = 20, nameOption = "Person", controller = "Home", action = "Person", status = true, isParent = false, parentId = 0, imageClass = "fa fa-users fa-fw" });
            menu.Add(new Navbar { Id = 21, nameOption = "Order", controller = "Home", action = "Order", status = true, isParent = false, parentId = 0, imageClass = "fa fa-space-shuttle  fa-fw" });
            
            return menu.ToList();
        }
    }
}