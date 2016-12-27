using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models.Mapping;

namespace sb_admin_2.Web1.Models
{
    public abstract class Document
    {
        private enum DocumentType { passport, viza }

        private DocumentType _documentType;

        public string documentType
        {
            get
            {
                switch(_documentType)
                {
                    case DocumentType.passport:
                        { 
                            return "Passport";
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
                    case "Passport":
                        {
                            _documentType = DocumentType.passport;
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

        public DateTime ValidTill { get; set; }

        public string ValidTillStr 
        {
            get { return ValidTill.ToString("ddMMMyy", Preferences.cultureInfo); }
            set { throw new NotImplementedException("ValidTillStr parser for document havnt been implemented"); }
        }

        public string Description { get; set; }

        public int ID { get; set; }
    }

    public class Passport : Document
    {
        public string SerialNumber { get; set; }

        public Person person { get; set; }

        public string PersonName { get; set; }

        public Country CountryOfEmmitation { get; set; }

        public List<Viza> VizaList { get; private set; }

        public Passport()
        {
            documentType = "Passport";
            VizaList = new List<Viza>();
        }

        public string VizaListStr 
        {
            get
            {
                string str = string.Empty;
                foreach (var viza in VizaList)
                {
                    if (str != string.Empty)
                        str += ", ";
                    str += viza.CountryOfInvintation.ISO;
                }
                return str;
            }
        }

        public Country Citizen { get; set; }

        public void AddViza(Country country, DateTime expired, int ID)
        {
            Viza viza = new Viza();
            VizaList.Add(viza);

            viza.CountryOfInvintation = country;
            viza.ValidTill = expired;
            viza.PassportSerial = SerialNumber;
            viza.ID = ID;
        }
    }

    public class Viza : Document
    {
        public string PassportSerial { get; set; }

        public Country CountryOfInvintation { get; set; }

        public Viza()
        {
            documentType = "Viza";
        }
    }
}