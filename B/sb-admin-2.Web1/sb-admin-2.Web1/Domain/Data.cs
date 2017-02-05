using sb_admin_2.Web1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Domain
{
    public class Data
    {
        public IEnumerable<Navbar> navbarItems()
        {
            var menu = new List<Navbar>();
            menu.Add(new Navbar { Id = 20, nameOption = "Person", controller = "Home", action = "PersonList", status = true, isParent = false, parentId = 0, imageClass = "fa fa-users fa-fw" });
            menu.Add(new Navbar { Id = 21, nameOption = "Order", controller = "Home", action = "Order", status = true, isParent = false, parentId = 0, imageClass = "fa fa-space-shuttle  fa-fw" });
            menu.Add(new Navbar { Id = 22, nameOption = "Settings", controller = "Home", action = "Settings", status = true, isParent = false, parentId = 0, imageClass = "fa fa-gears  fa-fw" });
            return menu.ToList();
        }
    }
}