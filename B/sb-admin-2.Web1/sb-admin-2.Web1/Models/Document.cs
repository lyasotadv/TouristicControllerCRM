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
        public enum DocumentStatus { green = 0, yellow = 1, red = 2 }

        public DocumentStatus Status { get; protected set; }

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

        private bool _Changed;

        public bool Changed 
        { 
            get
            {
                return _Changed;
            }
            protected set
            {
                _Changed = value;
                Validate();
            }
        }

        protected virtual void Validate()
        {

        }

        private DateTime _ValidTill;

        public DateTime ValidTill 
        { 
            get
            {
                return _ValidTill;
            }
            set
            {
                if (value != _ValidTill)
                {
                    _ValidTill = value;
                    Changed = true;
                }
            }
        }

        public string ValidTillStr 
        {
            get { return ValidTill.ToString("ddMMMyy", Preferences.cultureInfo); }
            set 
            {
                if (value != ValidTillStr)
                {
                    try
                    {
                        ValidTill = DateTime.ParseExact(value, "ddMMMyy", Preferences.cultureInfo);
                    }
                    catch
                    {
                        throw new FormatException("Incorrect input date format. Please use: ddMMMyy. Example: 13Jun31");
                    }
                    Changed = true;
                } 
            }
        }

        private string _Description;

        public string Description 
        { 
            get
            {
                return _Description;
            }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    Changed = true;
                }
            }
        }

        public int ID { get; set; }

        protected Document()
        {
            Status = DocumentStatus.green;
        }
    }

    public class PassportList : DBObjectList<Passport>
    {
        public Person person { get; set; }

        public override void Load()
        {
            if (person != null)
            {
                DBInterface.CommandText = "select idPassport from passport where idPeople = @id";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, person.PersonID);
                DataTable tab = DBInterface.ExecuteSelection();
                if (tab != null)
                {
                    foreach (DataRow row in tab.Rows)
                    {
                        Passport passport = new Passport();
                        passport.ID = Convert.ToInt32(row["idPassport"]);
                        passport.Load();
                        Add(passport);
                    }
                }
            }
        }

        public Passport Create()
        {
            Passport passport = new Passport();
            Init(passport);
            passport.person = person;
            return passport;
        }
    }

    public class Passport : Document, IDBObject
    {
        public Person person { get; set; }

        private string _SerialNumber;

        public string SerialNumber 
        { 
            get
            {
                return _SerialNumber;
            }
            set
            {
                if (value != _SerialNumber)
                {
                    _SerialNumber = value;
                    Changed = true;
                }
            }
        }

        private string _PersonName;

        public string PersonName 
        { 
            get
            {
                return _PersonName;
            }
            set
            {
                if (value != _PersonName)
                {
                    _PersonName = value;
                    Changed = true;
                }
            }
        }

        private Country _CountryOfEmitation;

        public Country CountryOfEmmitation 
        { 
            get
            {
                return _CountryOfEmitation;
            }
            set
            {
                if ((_CountryOfEmitation == null) || (value != null && (value.ID != _CountryOfEmitation.ID)))
                {
                    _CountryOfEmitation = value;
                    Changed = true;
                }
            }
        }

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

        private Country _Citizen;

        public Country Citizen  
        { 
            get
            {
                return _Citizen;
            }
            set
            {
                if ((_Citizen == null) || (value != null && (value.ID != _Citizen.ID)))
                {
                    _Citizen = value;
                    Changed = true;
                }
            }
        }

        public void AddViza(Country country, DateTime expired, int ID)
        {
            Viza viza = new Viza();
            VizaList.Add(viza);

            viza.CountryOfInvintation = country;
            viza.ValidTill = expired;
            viza.PassportSerial = SerialNumber;
            viza.ID = ID;
        }

        protected override void Validate()
        {
            bool IsYellow = false;
            IsYellow = IsYellow || (ValidTill - DateTime.Now < Preferences.PassportExpiredYellow);

            bool IsRed = false;
            IsRed = IsRed || (ValidTill - DateTime.Now < Preferences.PassportExpiredRed);

            if (IsYellow)
                Status = DocumentStatus.yellow;

            if (IsRed)
                Status = DocumentStatus.red;

            if (!(IsYellow | IsRed))
                Status = DocumentStatus.green;
        }


        public event EventHandler Updated;

        public void Load()
        {
            Changed = false;

            DBInterface.CommandText = "select ownerName, number, idCitizen, idCountry, expireDate, note, isActive from passport where idPassport = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

            DataTable tab = DBInterface.ExecuteSelection();
            if ((tab != null) && (tab.Rows.Count == 1))
            {
                PersonName = tab.Rows[0]["ownerName"].ToString();
                SerialNumber = tab.Rows[0]["number"].ToString();
                ValidTill = Convert.ToDateTime(tab.Rows[0]["expireDate"]);
                Description = tab.Rows[0]["note"].ToString();

                int idCountry = Convert.ToInt32(tab.Rows[0]["idCountry"]);
                CountryOfEmmitation = new Country();
                CountryOfEmmitation.ID = idCountry;
                CountryOfEmmitation.Load();

                int idCitizen = Convert.ToInt32(tab.Rows[0]["idCitizen"]);
                Citizen = new Country();
                Citizen.ID = idCountry;
                Citizen.Load();
            }
        }

        public void Save()
        {   
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "update passport set " +
                        "ownerName = @name, " +
                        "number = @number, " + 
                        "expireDate = @date, " + 
                        "note = @note, " + 
                        "idCitizen = @idCitizen, " + 
                        "idCountry = @idCountry " + 
                        "where idPassport = @id;";

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, PersonName);
                    DBInterface.AddParameter("@number", MySql.Data.MySqlClient.MySqlDbType.String, SerialNumber);
                    DBInterface.AddParameter("@date", MySql.Data.MySqlClient.MySqlDbType.DateTime, ValidTill);
                    DBInterface.AddParameter("@note", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    
                    if (Citizen != null)
                        DBInterface.AddParameter("@idCitizen", MySql.Data.MySqlClient.MySqlDbType.Int32, Citizen.ID);
                    
                    if (CountryOfEmmitation != null)
                        DBInterface.AddParameter("@idCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, CountryOfEmmitation.ID);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    InsertRow insertRow = new InsertRow("passport");

                    insertRow.Add("ownerName", MySql.Data.MySqlClient.MySqlDbType.String, PersonName);
                    insertRow.Add("number", MySql.Data.MySqlClient.MySqlDbType.String, SerialNumber);
                    insertRow.Add("expireDate", MySql.Data.MySqlClient.MySqlDbType.DateTime, ValidTill);
                    insertRow.Add("note", MySql.Data.MySqlClient.MySqlDbType.String, Description);

                    if (Citizen != null)
                        insertRow.Add("idCitizen", MySql.Data.MySqlClient.MySqlDbType.Int32, Citizen.ID);

                    if (CountryOfEmmitation != null)
                        insertRow.Add("idCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, CountryOfEmmitation.ID);

                    insertRow.Add("isActive", MySql.Data.MySqlClient.MySqlDbType.Int32, 1);
                    insertRow.Add("idPeople", MySql.Data.MySqlClient.MySqlDbType.Int32, person.PersonID);

                    insertRow.Execute();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = true });
                    }
                }

                Changed = false;
            }
        }

        public void Delete()
        {
            if (ID >= 0)
            {
                DBInterface.CommandText = "DELETE FROM `sellcontroller`.`passport` WHERE `idPassport` = @id;";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();

                if (Updated != null)
                {
                    Updated(this, new DBEventArgs() { ForceUpdate = true });
                }
            }
        }
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
    }
}