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
            HandleRawDataFile(file, BulkMapping.RawFileType.passport);
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult UploadPersonData(HttpPostedFileBase file)
        {
            HandleRawDataFile(file, BulkMapping.RawFileType.person);
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult UploadMileCardData(HttpPostedFileBase file)
        {
            HandleRawDataFile(file, BulkMapping.RawFileType.milecard);
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult UploadCompanyData(HttpPostedFileBase file)
        {
            HandleRawDataFile(file, BulkMapping.RawFileType.company);
            return RedirectToAction("AdminPage");
        }

        private void HandleRawDataFile(HttpPostedFileBase file, BulkMapping.RawFileType filetype)
        {
            bulkMapping.DataExported += CleanRawDataFile;
            controllerUtils.UploadFileAndSave(file, bulkMapping.GetStoredFileName(filetype));
            CheckIfUploaded();
        }

        private void CleanRawDataFile(object sender, EventArgs e)
        {
            if (e is BulkMapping.EventArgsRawData)
            {
                BulkMapping.EventArgsRawData arg = e as BulkMapping.EventArgsRawData;

                foreach(var str in arg.FileName)
                {
                    controllerUtils.DeleteFile(str);
                }
            }
        }

        private void CheckIfUploaded()
        {   
            foreach (var str in bulkMapping.GetStoredFileName())
            {
                string fullFileName = string.Empty;
                if (controllerUtils.FileExists(str, out fullFileName))
                {
                    bulkMapping.CheckIfLoaded(str, fullFileName);
                }
            }
        }
    }
}