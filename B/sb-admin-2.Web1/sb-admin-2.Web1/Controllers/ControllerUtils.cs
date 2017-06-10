using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Controllers
{
    public class ControllerUtils
    {
        private HttpServerUtilityBase _Server;

        public HttpServerUtilityBase Server
        {
            get
            {
                return _Server;
            }
            set
            {
                _Server = value;
            }
        }

        public AttachFileTransferData UploadFileAndSave(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string fileFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/temp"), fileName);
                file.SaveAs(fileFullName);

                string newName = Guid.NewGuid().ToString();
                string newFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/data"), newName);
                System.IO.File.Move(fileFullName, newFullName);

                AttachFileTransferData data = new AttachFileTransferData();
                data.guid = newName;
                data.Name = Path.GetFileNameWithoutExtension(fileName);
                data.Extension = Path.GetExtension(fileName);
                return data;
            }
            return null;
        }

        public void DeleteFile(string guid)
        {
            string FullName = Path.Combine(Server.MapPath("~/App_Data/uploads/data"), guid);
            if (System.IO.File.Exists(FullName))
                System.IO.File.Delete(FullName);
        }

        public void UploadFileAndParse(HttpPostedFileBase file, Func<string, bool> Parser)
        {
            if ((file.ContentLength > 0) & (Parser != null))
            {
                string fileName = Path.GetFileName(file.FileName);
                string fileFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/temp"), fileName);
                file.SaveAs(fileFullName);

                try
                {
                    if (!Parser(fileFullName))
                        throw new FileLoadException("File structure is incorrect");
                }
                finally
                {
                    System.IO.File.Delete(fileFullName);
                }
            }
        }

        public void DataLog(string data)
        {
            string fileFullName = Path.Combine(Server.MapPath("~/App_Data/"), "data.log");
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileFullName);
            file.WriteLine(data);
            file.Close();
        }
    }
}