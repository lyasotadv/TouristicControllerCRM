using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class DocumentList : ModelList<Document>
    {

    }

    public class Document : IDebugModel
    {
        private enum DocumentType { pasport, viza }

        private DocumentType _documentType;

        public string documentType
        {
            get
            {
                switch(_documentType)
                {
                    case DocumentType.pasport:
                        { 
                            return "Pasport";
                        }
                    case DocumentType.viza:
                        {
                            return "Viza";
                        }
                    default:
                        {
                            throw new ArgumentException("Unhandled document type");
                        }
                }
            }
            set
            {
                switch(value)
                {
                    case "Pasport":
                        {
                            _documentType = DocumentType.pasport;
                            break;
                        }
                    case "Viza":
                        {
                            _documentType = DocumentType.viza;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Incorrect document type");
                        }
                }
            }
        }

        public DateTime Valid { get; set; }

        public void CreateTestData()
        {
            _documentType = DocumentType.pasport;
            Valid = DateTime.Now;
        }
    }
}