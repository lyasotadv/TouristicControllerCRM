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
        public bool SilentDeleteFileIsExists; 

        public HttpServerUtilityBase Server { get; set; }

        public ControllerUtils()
        {
            SilentDeleteFileIsExists = true;
        }

        public AttachFileTransferData UploadFileAndSave(HttpPostedFileBase file)
        {
            string newName = Guid.NewGuid().ToString();
            return UploadFileAndSave(file, newName);
        }

        public AttachFileTransferData UploadFileAndSave(HttpPostedFileBase file, string newName)
        {
            if (file.ContentLength > 0)
            {
                if (SilentDeleteFileIsExists)
                {
                    DeleteFile(newName);
                }
                else
                {
                    if (FileExists(newName))
                        throw new FileLoadException("File with current name is exists");
                }

                string fileName = Path.GetFileName(file.FileName);
                string fileFullName = Path.Combine(Server.MapPath("~/App_Data/uploads/temp"), fileName);
                file.SaveAs(fileFullName);

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

        public bool FileExists(string fileName)
        {
            string fullFileName;
            return FileExists(fileName, out fullFileName);
        }

        public bool FileExists(string fileName, out string fullFileName)
        {
            fullFileName = Path.Combine(Server.MapPath("~/App_Data/uploads/data"), fileName);
            return System.IO.File.Exists(fullFileName);
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
            using (StreamWriter sw = File.AppendText(fileFullName))
            {
                sw.WriteLine(DateTime.Now.ToString() + " " + data);
            }
        }
    }
}