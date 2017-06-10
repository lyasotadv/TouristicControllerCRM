using sb_admin_2.Web1.Domain;
using sb_admin_2.Web1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sb_admin_2.Web1.Controllers
{
    public class NavbarController : Controller
    {
        // GET: Navbar
        public ActionResult Index()
        {
            var data = new Data();

            if ((Session["UserName"] != null) && (Session["UserName"].ToString() == "dimon"))
            {
                return PartialView("_Navbar", data.navbarItemsAdmin().ToList());
            }
            else
            {
                return PartialView("_Navbar", data.navbarItems().ToList());
            }
        }
    }
}