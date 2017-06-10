using sb_admin_2.Web1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Domain
{
    public class Data
    {
        private class IDEnumerator
        {
            private int _current = 0;

            public int Current
            {
                get
                {
                    return ++_current;
                }
            }

            public void Reset()
            {
                _current = 0;
            }

            public IDEnumerator()
            {
                Reset();
            }
        }

        private IDEnumerator idEnumerator;

        public Data()
        {
            idEnumerator = new IDEnumerator();
        }

        private List<Navbar> GetMenuCommon()
        {
            idEnumerator.Reset();

            var menu = new List<Navbar>();
            menu.Add(new Navbar { Id = idEnumerator.Current, nameOption = "Person", controller = "Home", action = "PersonList", status = true, isParent = false, parentId = 0, imageClass = "fa fa-users fa-fw" });
            //menu.Add(new Navbar { Id = idEnumerator.Current, nameOption = "Order", controller = "Home", action = "Order", status = true, isParent = false, parentId = 0, imageClass = "fa fa-space-shuttle  fa-fw" });
            menu.Add(new Navbar { Id = idEnumerator.Current, nameOption = "Settings", controller = "Home", action = "Settings", status = true, isParent = false, parentId = 0, imageClass = "fa fa-gears  fa-fw" });
            
            return menu;
        }

        public IEnumerable<Navbar> navbarItems()
        {
            var menu = GetMenuCommon();
            return menu.ToList();
        }

        public IEnumerable<Navbar> navbarItemsAdmin()
        {
            var menu = GetMenuCommon();
            menu.Add(new Navbar { Id = idEnumerator.Current, nameOption = "Admin", controller = "Admin", action = "AdminPage", status = true, isParent = false, parentId = 0, imageClass = "fa fa-fighter-jet  fa-fw" });
            return menu.ToList();
        }
    }
}