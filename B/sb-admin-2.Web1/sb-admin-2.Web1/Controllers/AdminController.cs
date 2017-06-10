using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using sb_admin_2.Web1.Domain;
using sb_admin_2.Web1.Models.Mapping.ExpImpUtils;

namespace sb_admin_2.Web1.Controllers
{
    public class AdminController : Controller
    {
        private ControllerUtils _controllerUtils;

        private ControllerUtils controllerUtils
        {
            get
            {
                if (_controllerUtils == null)
                {
                    _controllerUtils = new ControllerUtils();
                    _controllerUtils.Server = Server;
                }
                return _controllerUtils;
            }
        }

        private BulkMapping _bulkMapping;

        private BulkMapping bulkMapping
        {
            get
            {
                if (_bulkMapping == null)
                    _bulkMapping = new BulkMapping();
                return _bulkMapping;
            }
        }

        public ActionResult AdminPage()
        {
            AdminData data = new AdminData();
            return View("AdminPage", data);
        }

        [HttpPost]
        public ActionResult UploadPassportData(HttpPostedFileBase file)
        {
            controllerUtils.UploadFileAndParse(file, bulkMapping.rawPassportList.Export);
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult UploadPersonData(HttpPostedFileBase file)
        {
            controllerUtils.UploadFileAndParse(file, bulkMapping.rawPersonList.Export);
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult UploadMileCardData(HttpPostedFileBase file)
        {
            controllerUtils.UploadFileAndParse(file, bulkMapping.rawMileCardList.Export);
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult UploadCompanyData(HttpPostedFileBase file)
        {
            controllerUtils.UploadFileAndParse(file, bulkMapping.rawCompanyList.Export);
            return RedirectToAction("AdminPage");
        }
    }
}