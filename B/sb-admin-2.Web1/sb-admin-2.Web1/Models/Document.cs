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
                        Passport passport = Create();
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
            passport.SetPerson(person);
            return passport;
        }
    }

    public class Passport : Document, IDBObject
    {
        public void SetPerson(Person person)
        {
            this.person = person;
        }

        private Person person { get; set; }

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

        private string _PersonSurname;

        public string PersonSurname
        {
            get
            {
                return _PersonSurname;
            }
            set
            {
                if (value != _PersonSurname)
                {
                    _PersonSurname = value;
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
                if (value == null)
                {
                    throw new ArgumentNullException("Country of passport emitation must be set");
                }
                if ((_CountryOfEmitation == null) || (value != null && (value.ID != _CountryOfEmitation.ID)))
                {
                    _CountryOfEmitation = value;
                    Changed = true;
                }
            }
        }

        public VizaList vizaList { get; private set; }

        public Passport()
        {
            documentType = "Passport";
            vizaList = new VizaList(this);

            Changed = false;
            ID = -1;
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
                if (value == null)
                {
                    throw new ArgumentNullException("Please, set citizen field");
                }
                if ((_Citizen == null) || (value != null && (value.ID != _Citizen.ID)))
                {
                    _Citizen = value;
                    Changed = true;
                }
            }
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

            DBInterface.CommandText = "select ownerName, ownerSurname, number, idCitizen, idCountry, expireDate, note, isActive from passport where idPassport = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

            DataTable tab = DBInterface.ExecuteSelection();
            if ((tab != null) && (tab.Rows.Count == 1))
            {
                PersonName = tab.Rows[0]["ownerName"].ToString();
                PersonSurname = tab.Rows[0]["ownerSurname"].ToString();
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

                vizaList.Load();
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
                        "ownerSurname = @surname, " +
                        "number = @number, " + 
                        "expireDate = @date, " + 
                        "note = @note, " + 
                        "idCitizen = @idCitizen, " + 
                        "idCountry = @idCountry " + 
                        "where idPassport = @id;";

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, PersonName);
                    DBInterface.AddParameter("@surname", MySql.Data.MySqlClient.MySqlDbType.String, PersonSurname);
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
                    insertRow.Add("ownerSurname", MySql.Data.MySqlClient.MySqlDbType.String, PersonName);
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

        // SRDOCS HK1-P-UKR-EH123456-UKR-30JUN73-M-14APR20-LYSENKO-MYKOLA
        public string AmadeusString
        {
            get
            {
                string str = null;
                if ((PersonName != null) & (PersonSurname != null) & (ValidTillStr != null) & (person.BirthStr != null))
                {
                    str = "SRDOCS HK1-P-";
                    str += Citizen.ISO3 + "-";
                    str += SerialNumber + "-";
                    str += CountryOfEmmitation.ISO3 + "-";
                    str += person.BirthStr.ToUpper() + "-";
                    str += person.AviaStatus() + "-";
                    str += ValidTillStr.ToUpper() + "-";
                    str += PersonSurname.ToUpper() + "-";
                    str += PersonName.ToUpper();
                }
                return str;
            }
        }

        // SI.P1/DOCS*P/UA/EH123456/UA/30JUN73/M/14APR20/LYSENKO/MYKOLA
        public string Galileo
        {
            get
            {
                string str = null;
                if ((PersonName != null) & (PersonSurname != null) & (ValidTillStr != null) & (person.BirthStr != null))
                {
                    str = "SI.P1/DOCS*P/";
                    str += Citizen.ISO + "/";
                    str += SerialNumber + "/";
                    str += CountryOfEmmitation.ISO + "/";
                    str += person.BirthStr.ToUpper() + "/";
                    str += person.AviaStatus() + "/";
                    str += ValidTillStr.ToUpper() + "/";
                    str += PersonSurname.ToUpper() + "/";
                    str += PersonName.ToUpper();
                }
                return str;
            }
        }

        // 3DOCS/P/UKR/EH123456/UKR/30JUN73/M/14APR20/LYSENKO/MYKOLA
        public string Sabre
        {
            get
            {
                string str = null;
                if ((PersonName != null) & (PersonSurname != null) & (ValidTillStr != null) & (person.BirthStr != null))
                {
                    str = "3DOCS/P/";
                    str += Citizen.ISO3 + "/";
                    str += SerialNumber + "/";
                    str += CountryOfEmmitation.ISO3 + "/";
                    str += person.BirthStr.ToUpper() + "/";
                    str += person.AviaStatus() + "/";
                    str += ValidTillStr.ToUpper() + "/";
                    str += PersonSurname.ToUpper() + "/";
                    str += PersonName.ToUpper();
                }
                return str;
            }
        }
    }

    public class VizaList : DBObjectList<Viza>
    {
        private Passport passport { get; set; }

        public VizaList(Passport passport)
        {
            if (passport == null)
                throw new ArgumentException("Passport must be not null in viza list");

            this.passport = passport;
        }

        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.visa where idPassport = @idPassport;";
            DBInterface.AddParameter("@idPassport", MySql.Data.MySqlClient.MySqlDbType.Int32, passport.ID);
            DataTable tab = DBInterface.ExecuteSelection();

            foreach(DataRow row in tab.Rows)
            {
                Viza viza = Create();
                viza.ID = Convert.ToInt32(row["idViza"]);
                viza.Load();
            }
        }

        public Viza Create()
        {
            Viza viza = new Viza();
            Init(viza);
            Add(viza);
            return viza;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach (var viza in this)
            {
                if (str != string.Empty)
                    str += ", ";
                str += viza.TargetName;
            }
            return str;
        }

        public string StrFormat
        {
            get
            {
                return ToString();
            }
        }
    }

    public class Viza : Document, IDBObject
    {
        private Passport passport { get; set; }

        public int PassportID
        {
            get
            {
                return passport.ID;
            }
            set
            {
                if (value != PassportID)
                {
                    passport.ID = value;
                    passport.Load();
                    Changed = true;
                }
            }
        }

        public string PassportSerial
        {
            get
            {
                return passport.SerialNumber;
            }
        }

        private string _OwnerName;

        public string OwnerName
        {
            get
            {
                return _OwnerName;
            }
            set
            {
                if (value != _OwnerName)
                {
                    _OwnerName = value;
                    Changed = true;
                }
            }
        }

        private string _Number;

        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                if (value != _Number)
                {
                    _Number = value;
                    Changed = true;
                }
            }
        }

        private DateTime _ValidFrom;

        public DateTime ValidFrom
        {
            get
            {
                return _ValidFrom;
            }
            set
            {
                if (value != _ValidFrom)
                {
                    _ValidFrom = value;
                    Changed = true;
                }
            }
        }

        public string ValidFromStr
        {
            get { return ValidFrom.ToString("ddMMMyy", Preferences.cultureInfo); }
            set
            {
                if (value != ValidFromStr)
                {
                    try
                    {
                        ValidFrom = DateTime.ParseExact(value, "ddMMMyy", Preferences.cultureInfo);
                    }
                    catch
                    {
                        throw new FormatException("Incorrect input date format. Please use: ddMMMyy. Example: 13Jun31");
                    }
                    Changed = true;
                }
            }
        }

        private DateTime _DateApproved;

        public DateTime DateApproved
        {
            get
            {
                return _DateApproved;
            }
            set
            {
                if (value != _DateApproved)
                {
                    _DateApproved = value;
                    Changed = true;
                }
            }
        }

        public string DateApprovedStr
        {
            get { return DateApproved.ToString("ddMMMyy", Preferences.cultureInfo); }
            set
            {
                if (value != DateApprovedStr)
                {
                    try
                    {
                        DateApproved = DateTime.ParseExact(value, "ddMMMyy", Preferences.cultureInfo);
                    }
                    catch
                    {
                        throw new FormatException("Incorrect input date format. Please use: ddMMMyy. Example: 13Jun31");
                    }
                    Changed = true;
                }
            }
        }

        private Country CountryOfEmmitation { get; set; }

        public int CountryOfEmmitationID
        {
            get
            {
                return CountryOfEmmitation.ID;
            }
            set
            {
                if (value != CountryOfEmmitationID)
                {
                    CountryOfEmmitation.ID = value;
                    CountryOfEmmitation.Load();
                    Changed = true;
                }
            }
        }

        public string CountryOfEmmitationName
        {
            get
            {
                return CountryOfEmmitation.Name;
            }
        }

        private string _VizaType;

        public string VizaType
        {
            get
            {
                return _VizaType;
            }
            set
            {
                if (value != _VizaType)
                {
                    _VizaType = value;
                    Changed = true;
                }
            }
        }

        private int _EntriesNumber;

        public int EntriesNumber
        {
            get
            {
                return _EntriesNumber;
            }
            set
            {
                if (value != _EntriesNumber)
                {
                    _EntriesNumber = value;
                    Changed = true;
                }
            }
        }

        private int _DaysCount;

        public int DaysCount
        {
            get
            {
                return _DaysCount;
            }
            set
            {
                if (value != _DaysCount)
                {
                    _DaysCount = value;
                    Changed = true;
                }
            }
        }

        private int _DaysUsed;

        public int DaysUsed
        {
            get
            {
                return _DaysUsed;
            }
            set
            {
                if (value != _DaysUsed)
                {
                    _DaysUsed = value;
                    Changed = true;
                }
            }
        }

        private string _Issued;

        public string Issued
        {
            get
            {
                return _Issued;
            }
            set
            {
                if (value != _Issued)
                {
                    _Issued = value;
                    Changed = true;
                }
            }
        }

        private IVizaFormation Target { get; set; }

        public string TargetName
        {
            get
            {
                if (Target == null)
                    return string.Empty;
                return Target.Name;
            }
        }

        public bool IsTargetUnion
        {
            get
            {
                return (Target != null) && (Target is CountryUnion);
            }
        }

        public Viza()
        {
            documentType = "Viza";

            Changed = false;
            ID = -1;

            CountryOfEmmitation = new Country();
            passport = new Passport();
        }


        public event EventHandler Updated;

        private void SetTargetCountryOrUnion(int countryID, int unionID)
        {
            if (countryID * unionID > 0)
            {
                throw new ArgumentException("Viza must be targeted on country or union");
            }

            if (countryID > -1)
            {
                Target = new Country();
                Target.ID = countryID;
            }

            if (unionID > -1)
            {
                Target = new CountryUnion();
                Target.ID = unionID;
            }

            Target.Load();
            Changed = true;
        }

        private void GetTargetCountryOrUnion(out int countryID, out int unionID)
        {
            countryID = -1;
            unionID = -1;

            if (Target != null)
            {
                if (Target is Country)
                {
                    countryID = Target.ID;
                }

                if (Target is CountryUnion)
                {
                    unionID = Target.ID;
                }
            }
        }

        public void SetTarget(int id, bool IsUnion)
        {
            if (IsUnion)
                SetTargetCountryOrUnion(-1, id);
            else
                SetTargetCountryOrUnion(id, -1);
        }

        public void SetTarget(string Name, bool IsUnion)
        {
            if (IsUnion)
            {
                CountryUnionList cul = new CountryUnionList();
                cul.Load();
                Target = cul.Find(item => item.Name == Name);
            }
            else
            {
                CountryList cl = new CountryList();
                cl.Load();
                Target = cl.Find(item => item.Name == Name);
            }
            if (Target != null)
            {
                Target.Load();
                Changed = true;
            }
        }

        public void Load()
        {
            if (ID != -1)
            {
                IDBInterface db = DBInterface.CreatePointer();

                db.StoredProcedure("Visa_select_by_id");

                db.AddParameter("@inVisa", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

                db.AddOutParameter("@outIdPassport", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outName", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outVisaNumber", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outDateOn", MySql.Data.MySqlClient.MySqlDbType.DateTime);
                db.AddOutParameter("@outdateFrom", MySql.Data.MySqlClient.MySqlDbType.DateTime);
                db.AddOutParameter("@outDateUntil", MySql.Data.MySqlClient.MySqlDbType.DateTime);
                db.AddOutParameter("@outIdCountry", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outIdCountryEsquire", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outTypeVisa", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outEntriesNumber", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outDaysCount", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outUsedDays", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outIssuedIn", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outIdDocument", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outNote", MySql.Data.MySqlClient.MySqlDbType.String);

                db.Execute();

                PassportID = db.GetOutParameterInt("@outIdPassport");
                int countryID = db.GetOutParameterInt("@outIdCountry");
                int unionID = db.GetOutParameterInt("@outIdCountryUnion");
                CountryOfEmmitationID = db.GetOutParameterInt("@outIdCountryEsquire");
                EntriesNumber = db.GetOutParameterInt("@outEntriesNumber");
                DaysCount = db.GetOutParameterInt("@outDaysCount");
                DaysUsed = db.GetOutParameterInt("@outUsedDays");

                OwnerName = db.GetOutParameterStr("@outName");
                Number = db.GetOutParameterStr("@outVisaNumber");
                VizaType = db.GetOutParameterStr("@outTypeVisa");
                Issued = db.GetOutParameterStr("@outIssuedIn");
                Description = db.GetOutParameterStr("@outNote");

                DateApproved = db.GetOutParameterDateTime("@outDateOn");
                ValidFrom = db.GetOutParameterDateTime("@outdateFrom");
                ValidTill = db.GetOutParameterDateTime("@outDateUntil");

                Changed = false;
            }
        }

        public void Save()
        {
            if (Changed)
            {
                int countryID;
                int unionID;
                GetTargetCountryOrUnion(out countryID, out unionID);

                if (ID >= 0)
                {
                    DBInterface.StoredProcedure("visa_update");

                    DBInterface.AddParameter("@inVisa", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.AddParameter("@inIdPassport", MySql.Data.MySqlClient.MySqlDbType.Int32, PassportID);
                    DBInterface.AddParameter("@inName", MySql.Data.MySqlClient.MySqlDbType.String, OwnerName);
                    DBInterface.AddParameter("@inVisaNumber", MySql.Data.MySqlClient.MySqlDbType.String, Number);
                    DBInterface.AddParameter("@inDateOn", MySql.Data.MySqlClient.MySqlDbType.DateTime, DateApproved);
                    DBInterface.AddParameter("@indateFrom", MySql.Data.MySqlClient.MySqlDbType.DateTime, ValidFrom);
                    DBInterface.AddParameter("@inDateUntil", MySql.Data.MySqlClient.MySqlDbType.DateTime, ValidTill);

                    if (countryID == -1)
                    {
                        DBInterface.AddParameter("@inIdCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, DBNull.Value);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, countryID);
                    }


                    if (unionID == -1)
                    {
                        DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, DBNull.Value);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, unionID);
                    }

                    DBInterface.AddParameter("@inIdCountryEsquire", MySql.Data.MySqlClient.MySqlDbType.Int32, CountryOfEmmitationID);
                    DBInterface.AddParameter("@inTypeVisa", MySql.Data.MySqlClient.MySqlDbType.String, VizaType);
                    DBInterface.AddParameter("@inEntriesNumber", MySql.Data.MySqlClient.MySqlDbType.Int32, EntriesNumber);
                    DBInterface.AddParameter("@inDaysCount", MySql.Data.MySqlClient.MySqlDbType.Int32, DaysCount);
                    DBInterface.AddParameter("@inUsedDays", MySql.Data.MySqlClient.MySqlDbType.Int32, DaysUsed);
                    DBInterface.AddParameter("@inIssuedIn", MySql.Data.MySqlClient.MySqlDbType.String, Issued);
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@inIdDocument", MySql.Data.MySqlClient.MySqlDbType.Int32, DBNull.Value);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    DBInterface.StoredProcedure("visa_insert");

                    DBInterface.AddOutParameter("@outIdVisa", MySql.Data.MySqlClient.MySqlDbType.Int32);

                    DBInterface.AddParameter("@inIdPassport", MySql.Data.MySqlClient.MySqlDbType.Int32, PassportID);
                    DBInterface.AddParameter("@inName", MySql.Data.MySqlClient.MySqlDbType.String, OwnerName);
                    DBInterface.AddParameter("@inVisaNumber", MySql.Data.MySqlClient.MySqlDbType.String, Number);
                    DBInterface.AddParameter("@inDateOn", MySql.Data.MySqlClient.MySqlDbType.DateTime, DateApproved);
                    DBInterface.AddParameter("@indateFrom", MySql.Data.MySqlClient.MySqlDbType.DateTime, ValidFrom);
                    DBInterface.AddParameter("@inDateUntil", MySql.Data.MySqlClient.MySqlDbType.DateTime, ValidTill);

                    if (countryID == -1)
                    {
                        DBInterface.AddParameter("@inIdCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, DBNull.Value);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, countryID);
                    }
                    

                    if (unionID == -1)
                    {
                        DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, DBNull.Value);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, unionID);
                    }

                    DBInterface.AddParameter("@inIdCountryEsquire", MySql.Data.MySqlClient.MySqlDbType.Int32, CountryOfEmmitationID);
                    DBInterface.AddParameter("@inTypeVisa", MySql.Data.MySqlClient.MySqlDbType.String, VizaType);
                    DBInterface.AddParameter("@inEntriesNumber", MySql.Data.MySqlClient.MySqlDbType.Int32, EntriesNumber);
                    DBInterface.AddParameter("@inDaysCount", MySql.Data.MySqlClient.MySqlDbType.Int32, DaysCount);
                    DBInterface.AddParameter("@inUsedDays", MySql.Data.MySqlClient.MySqlDbType.Int32, DaysUsed);
                    DBInterface.AddParameter("@inIssuedIn", MySql.Data.MySqlClient.MySqlDbType.String, Issued);
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@inIdDocument", MySql.Data.MySqlClient.MySqlDbType.Int32, DBNull.Value);

                    DBInterface.ExecuteTransaction();

                    ID = DBInterface.GetOutParameterInt("@outIdVisa");

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
            DBInterface.StoredProcedure("Visa_delete");
            DBInterface.AddParameter("@inIdVisa", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.ExecuteTransaction();
        }
    }
}