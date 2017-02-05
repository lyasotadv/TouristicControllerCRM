using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping;
using sb_admin_2.Web1.Models.Mapping.DBUtils;

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

    public class PassportList : DBObjectList<Passport>
    {
        public override void Load()
        {
            throw new NotImplementedException("Load method for passport list");
        }

        public Passport Create()
        {
            Passport passport = new Passport();
            Init(passport);
            return passport;
        }
    }

    public class Passport : Document, IDBObject
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

            Changed = false;
            ID = -1;
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


        public event EventHandler Updated;

        public void Load()
        {
            Changed = false;

            throw new NotImplementedException("Load methd for passport have not been implemented");
        }

        public void Save()
        {   
            if (Changed)
            {
                if (ID >= 0)
                { 
                    //To Do
                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    //To Do
                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = true });
                    }
                }

                Changed = false;
            }

            throw new NotImplementedException("Save method for passport have not been implemented");
        }

        public void Delete()
        {

        }

        public bool Changed { get; private set; }
    }

    public class VizaList : DBObjectList<Viza>
    {
        public override void Load()
        {
            throw new NotImplementedException("Load method for viza not implementted unitil respective table has not been created");
        }
    }

    public class Viza : Document, IDBObject
    {
        public string PassportSerial { get; set; }

        public Country CountryOfInvintation { get; set; }

        public Viza()
        {
            documentType = "Viza";

            Changed = false;
            ID = -1;
        }


        public event EventHandler Updated;

        public void Load()
        {
            Changed = false;
            throw new NotImplementedException("Load method for viza is not implemented");
        }

        public void Save()
        {
            if (Changed)
            {
                Changed = false;

                if (Updated != null)
                {
                    Updated(this, new DBEventArgs() { ForceUpdate = true });
                }
            }
            throw new NotImplementedException("Save method for viza not implemented");
        }

        public void Delete()
        {

        }

        public bool Changed { get; private set; }
    }
}