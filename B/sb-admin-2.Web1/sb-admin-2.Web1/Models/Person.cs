using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping;
using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class PersonList : DBObjectList<PersonGeneral>
    {
        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "select " +
                                        "person.idPerson, " +
                                        "people.idPeople, " +
                                        "company.idCompany, " +
                                        "people.firstName, " +
                                        "people.lastName, " +
                                        "people.middleName, " +
                                        "company.officialCompanyName, " +
                                        "person.isPeople " +
                                        "from person " +
                                        "left join people " +
                                        "on people.idPerson = person.idPerson " +
                                        "left join company " +
                                        "on company.idPerson = person.idPerson;";

            DataTable tab = DBInterface.ExecuteSelection();

            foreach(DataRow row in tab.Rows)
            {
                if (row["isPeople"].ToString() == "1")
                {
                    Person person = new Person();

                    person.ID = Convert.ToInt32(row["idPerson"]);
                    person.PersonID = Convert.ToInt32(row["idPeople"]);
                    person.FirstName = row["firstName"].ToString();
                    person.SecondName = row["lastName"].ToString();
                    person.MiddleName = row["middleName"].ToString();
                    person.labelList.Load();

                    this.Add(person);
                }
                else
                {
                    Company company = new Company();

                    company.ID = Convert.ToInt32(row["idPerson"]);
                    company.CompanyID = Convert.ToInt32(row["idCompany"]);
                    company.FullName = row["officialCompanyName"].ToString();
                    company.labelList.Load();

                    this.Add(company);
                }
            }
        }

        public PersonGeneral Create(string companyType)
        {
            PersonGeneral person = null;
            switch (companyType)
            {
                case "people":
                {
                    person = new Person();
                    break;
                }
            }

            if (person == null)
                throw new NotImplementedException("Unhandled company type description");
            Init(person);
            return person;
        }
    }

    public class PersonType
    {
        private PersonGeneral _Owner;

        public PersonGeneral Owner 
        { 
            get
            {
                return _Owner;
            }
            private set
            {
                if (value != _Owner)
                {
                    _Owner = value;
                    Update();
                }
            }
        }

        public PersonType(PersonGeneral Owner)
        {
            this.Owner = Owner;
        }

        private void Update()
        {
            if (Owner != null)
            {
                if (Owner is Person)
                {
                    Desciption = "Person";
                }
                else if (Owner is AirCompany)
                {
                    Desciption = "Air company";
                }
                else if (Owner is Insurance)
                {
                    Desciption = "Insurance company";
                }
                else if (Owner is Provider)
                {
                    Desciption = "Provider";
                }
                else if (Owner is Company)
                {
                    Desciption = "Company";
                }
                else
                {
                    throw new InvalidOperationException("Unknown person type");
                }
            }
            else
            {
                Desciption = string.Empty;
            }
        }
        
        public string Desciption { get; private set; }
    }

    public abstract class PersonGeneral : IDBObject
    {
        private PersonType personType;

        public string PersonTypeStr
        {
            get
            {
                return personType.Desciption;
            }
        }

        public int ID { get; set; }

        public abstract string FullName { get; set; }

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

        private PersonGeneral _Parent;

        public PersonGeneral Parent 
        { 
            get
            {
                return _Parent;
            }
            set
            {
                if ((value != null) && ((_Parent == null) || (value.ID != _Parent.ID)))
                {
                    _Parent = value;
                    Changed = true;
                }
            }
        }

        public LabelListPerson labelList { get; private set; }

        protected PersonGeneral()
        {
            personType = new PersonType(this);
            ContactList = new ContactList();
            ContactList.person = this;
            labelList = new LabelListPerson(this);

            Changed = false;
            ID = -1;

            Updated += OnUpdated;
        }

        private void OnUpdated(object sender, EventArgs arg)
        {
            if ((arg is DBEventArgs) && ((arg as DBEventArgs).ForceUpdate))
            {
                Load();
            }
        }

        public ContactList ContactList { get; private set; }


        public event EventHandler Updated;

        public abstract void Load();

        public abstract void Save();

        public virtual void Delete()
        {
            DBInterface.CommandText = "delete from person where idPerson = @id";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.ExecuteTransaction();
        }

        public bool Changed { get; protected set; }


        protected void RaiseUpdated(bool ForceUpdate)
        {
            if (Updated != null)
            {
                Updated(this, new DBEventArgs() { ForceUpdate = ForceUpdate });
            }
        }

        protected string IntToStr(long numb, int Length)
        {
            if (numb == 0)
                return string.Empty;

            string str = numb.ToString();

            if ((str.Length > Length) | (numb < 0))
            {
                throw new FormatException("Number is too long");
            }

            while (str.Length < Length)
            {
                str = " " + str;
            }
            return str;
        }
    }

    public class Person : PersonGeneral
    {
        public int PersonID { get; set; }

        private enum Sex { male, female };

        private Sex _sex;

        private int GenderID()
        {
            if (_sex == Sex.male)
                return 0;
            else
                return 1;
        }

        private string _FirstName;

        public string FirstName 
        { 
            get
            {
                return _FirstName;
            }
            set
            {
                if (value != _FirstName)
                {
                    _FirstName = value;
                    Changed = true;
                }
            }
        }

        private string _SecondName;

        public string SecondName
        {
            get
            {
                return _SecondName;
            }
            set
            {
                if (value != _SecondName)
                {
                    _SecondName = value;
                    Changed = true;
                }
            }
        }

        private string _MiddleName;

        public string MiddleName
        {
            get
            {
                return _MiddleName;
            }
            set
            {
                if (value != _MiddleName)
                {
                    _MiddleName = value;
                    Changed = true;
                }
            }
        }

        private string _FirstNameUA;

        public string FirstNameUA
        {
            get
            {
                return _FirstNameUA;
            }
            set
            {
                if (value != _FirstNameUA)
                {
                    _FirstNameUA = value;
                    Changed = true;
                }
            }
        }

        private string _SecondNameUA;

        public string SecondNameUA
        {
            get
            {
                return _SecondNameUA;
            }
            set
            {
                if (value != _SecondNameUA)
                {
                    _SecondNameUA = value;
                    Changed = true;
                }
            }
        }

        private string _MiddleNameUA;

        public string MiddleNameUA
        {
            get
            {
                return _MiddleNameUA;
            }
            set
            {
                if (value != _MiddleNameUA)
                {
                    _MiddleNameUA = value;
                    Changed = true;
                }
            }
        }

        public string Gender 
        { 
            get
            {
                if (_sex == Person.Sex.male)
                    return "male";
                else
                    return "female";
            }
            set
            {
                if (value != Gender)
                {
                    switch (value)
                    {
                        case "male":
                            {
                                _sex = Person.Sex.male;
                                break;
                            };
                        case "female":
                            {
                                _sex = Person.Sex.female;
                                break;
                            }
                        default:
                            {
                                throw new ArgumentException("Input gender in incorrect");
                            }
                    }
                    Changed = true;
                }
            }
        }

        public DateTime Birth { get; set; }

        public string BirthStr
        {
            get { return Birth.ToString("ddMMMyy", Preferences.cultureInfo); }
            set 
            {
                if (value != BirthStr)
                {
                    try
                    {
                        Birth = DateTime.ParseExact(value, "ddMMMyy", Preferences.cultureInfo);
                    }
                    catch
                    {
                        throw new FormatException("Incorrect input date format. Please use: ddMMMyy. Example: 13Jun31");
                    }
                    Changed = true;
                }
            }
        }

        public override string FullName
        {
            get { return FirstName + " " + MiddleName + " " + SecondName; }
            set { throw new NotImplementedException("There is no parser of full name for person"); }
        }

        public string FullNameUA
        {
            get
            {
                return SecondNameUA + " " + FirstNameUA + " " + MiddleNameUA;
            }
            set
            {
                if (value != null)
                {
                    string[] str = value.Split(' ');
                    int iter = 0;
                    foreach(var s in str)
                    {
                        if ((s == null) || (s == string.Empty))
                            continue;

                        switch (iter)
                        {
                            case 0: SecondNameUA = s; break;
                            case 1: FirstNameUA = s; break;
                            case 2: MiddleNameUA = s; break;
                        }
                        iter++;
                    }
                }
            }
        }

        public PassportList PassportList { get; private set; }

        private Country _Citizen;

        public Country Citizen 
        { 
            get
            {
                return _Citizen;
            }
            set
            {
                if ((value != null) && ((_Citizen == null) || (value.ID != _Citizen.ID)))
                {
                    _Citizen = value;
                    Changed = true;
                }
            }
        }

        private int _itn;

        public string itn
        {
            get
            {
                return IntToStr(_itn, 10);
            }
            set
            {
                if ((value != itn) && (value != null) && (value != string.Empty))
                {
                    try
                    {
                        if (value.Length != 10)
                            throw new FormatException("ITN must contains 10 digits");
                        _itn = Convert.ToInt32(value);
                    }
                    catch
                    {
                        throw new FormatException("ITN must contains 10 digits");
                    }
                    Changed = true;
                }
            }
        }

        public Person()
        {
            PassportList = new PassportList();
            PassportList.person = this;
        }

        static public Person CreatePerson()
        {
            Person person = new Person();
            person.Changed = true;
            return person;
        }

        public override void Load()
        {
            DBInterface.CommandText = "SELECT `people`.`idPeople`, " + 
                                        "`people`.`idPerson`, " +
                                        "`people`.`firstName`, " + 
                                        "`people`.`middleName`, " + 
                                        "`people`.`lastName`, " +
                                        "`people`.`firstNameUA`, " +
                                        "`people`.`middleNameUA`, " +
                                        "`people`.`lastNameUA`, " + 
                                        "`people`.`birthDate`, " +
                                        "`people`.`Note`, " +
                                        "`people`.`itn`, " +
                                        "`people`.`gender` " +
                                        "FROM `sellcontroller`.`people` WHERE `idPeople` = @id;";

            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonID);
            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                FirstName = Convert.ToString(tab.Rows[0]["firstName"]);
                SecondName = Convert.ToString(tab.Rows[0]["lastName"]);
                MiddleName = Convert.ToString(tab.Rows[0]["middleName"]);
                FirstNameUA = Convert.ToString(tab.Rows[0]["firstNameUA"]);
                SecondNameUA = Convert.ToString(tab.Rows[0]["lastNameUA"]);
                MiddleNameUA = Convert.ToString(tab.Rows[0]["middleNameUA"]);
                itn = Convert.ToString(tab.Rows[0]["itn"]);
                Birth = Convert.ToDateTime(tab.Rows[0]["birthDate"]);
                Description = Convert.ToString(tab.Rows[0]["Note"]);

                if (Convert.ToInt32(tab.Rows[0]["gender"]) == 0)
                    _sex = Sex.male;
                else
                    _sex = Sex.female;
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("People table has rows with same id");
            }

            ContactList.Load();
            PassportList.Load();
            labelList.Load();

            Changed = false;
        }

        public override void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "UPDATE `sellcontroller`.`people` " +
                                                "SET " +
                                                "`firstName` = @firstName, " +
                                                "`middleName` = @middleName, " +
                                                "`lastName` = @lastName, " +
                                                "`firstNameUA` = @firstNameUA, " +
                                                "`middleNameUA` = @middleNameUA, " +
                                                "`lastNameUA` = @lastNameUA, " +
                                                "`birthDate` = @birthDate, " +
                                                "`Note` = @desc, " +
                                                "`itn` = @itn, " +
                                                "`gender` = @gender " +
                                                "WHERE `idPeople` = @id;";

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonID);
                    DBInterface.AddParameter("@firstName", MySql.Data.MySqlClient.MySqlDbType.String, FirstName);
                    DBInterface.AddParameter("@middleName", MySql.Data.MySqlClient.MySqlDbType.String, MiddleName);
                    DBInterface.AddParameter("@lastName", MySql.Data.MySqlClient.MySqlDbType.String, SecondName);
                    DBInterface.AddParameter("@firstNameUA", MySql.Data.MySqlClient.MySqlDbType.String, FirstNameUA);
                    DBInterface.AddParameter("@middleNameUA", MySql.Data.MySqlClient.MySqlDbType.String, MiddleNameUA);
                    DBInterface.AddParameter("@lastNameUA", MySql.Data.MySqlClient.MySqlDbType.String, SecondNameUA);
                    DBInterface.AddParameter("@itn", MySql.Data.MySqlClient.MySqlDbType.String, itn);
                    DBInterface.AddParameter("@birthDate", MySql.Data.MySqlClient.MySqlDbType.DateTime, Birth);
                    DBInterface.AddParameter("@desc", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@gender", MySql.Data.MySqlClient.MySqlDbType.Int32, GenderID());

                    DBInterface.ExecuteTransaction();

                    RaiseUpdated(false);
                }
                else
                {
                    DBInterface.CommandText = "insert into person (isPeople) values (1); " +
                                                "insert into people (idPerson, firstName, middleName, lastName, firstNameUA, middleNameUA, lastNameUA, Note, birthDate, gender, itn) " +
                                                "values (LAST_INSERT_ID(), @firstName, @middleName, @lastName, @firstNameUA, @middleNameUA, @lastNameUA, @desc, @birthDate, @gender, @itn);";

                    DBInterface.AddParameter("@firstName", MySql.Data.MySqlClient.MySqlDbType.String, FirstName);
                    DBInterface.AddParameter("@middleName", MySql.Data.MySqlClient.MySqlDbType.String, MiddleName);
                    DBInterface.AddParameter("@lastName", MySql.Data.MySqlClient.MySqlDbType.String, SecondName);
                    DBInterface.AddParameter("@firstNameUA", MySql.Data.MySqlClient.MySqlDbType.String, FirstNameUA);
                    DBInterface.AddParameter("@middleNameUA", MySql.Data.MySqlClient.MySqlDbType.String, MiddleNameUA);
                    DBInterface.AddParameter("@lastNameUA", MySql.Data.MySqlClient.MySqlDbType.String, SecondNameUA);
                    DBInterface.AddParameter("@itn", MySql.Data.MySqlClient.MySqlDbType.String, itn);
                    DBInterface.AddParameter("@birthDate", MySql.Data.MySqlClient.MySqlDbType.DateTime, Birth);
                    DBInterface.AddParameter("@desc", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@gender", MySql.Data.MySqlClient.MySqlDbType.Int32, GenderID());

                    PersonID = Convert.ToInt32(DBInterface.ExecuteTransaction());

                    DBInterface.CommandText = "select idPerson from people where idPeople = @id";
                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonID);

                    DataTable tab = DBInterface.ExecuteSelection();
                    ID = Convert.ToInt32(tab.Rows[0]["idPerson"]);

                    RaiseUpdated(true);
                }

                Changed = false;
            }
        }

        public override void Delete()
        {
            DBInterface.CommandText = "delete from people where idPeople = @id";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonID);
            DBInterface.ExecuteTransaction();

            base.Delete();
        }

        public string AviaStatus()
        {
            string str = string.Empty;
            if (_sex == Sex.male)
                str += "M";
            else
                str += "F";

            TimeSpan age = DateTime.Now - Birth;
            if (age < TimeSpan.FromDays(365.0 * 2.0))
                str += "I";

            return str;
        }
    }

    public class Company : PersonGeneral
    {
        public int CompanyID { get; set; }

        public enum CompanyType { air, isurance, provider }

        protected CompanyType companyType;

        private string _FullName;

        public override string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }

        private int _MFO;

        public string MFO
        {
            get
            {
                return IntToStr(_MFO, 6);
            }
            set
            {
                if ((value != MFO) && (value != null) && (value != string.Empty) && (value != "0"))
                {
                    try
                    {
                        if ((value == null) || (value.Length != 6))
                        {
                            throw new FormatException("MFO must contains 6 digits");
                        }
                        _MFO = Convert.ToInt32(value.Replace(" ", ""));
                    }
                    catch
                    {
                        throw new FormatException("MFO must contains 6 digits");
                    }
                    Changed = true;
                }
            }
        }

        private int _EDRPOU;

        // 32855961
        private bool CheckEDRPOU(string val)
        {
            if (val.Length == 8)
            {
                int[] x = new int[8];
                for (int n = 0; n<8; n++)
                {
                    x[n] = Convert.ToInt32(val.Substring(n, 1));
                }
                int valInt = Convert.ToInt32(val);

                int[] m = new int[7];

                if ((valInt > 30000000) & (valInt < 60000000))
                {
                    m[0] = 7;
                    m[1] = 1;
                    m[2] = 2;
                    m[3] = 3;
                    m[4] = 4;
                    m[5] = 5;
                    m[6] = 6;
                }
                else
                {
                    m[0] = 1;
                    m[1] = 2;
                    m[2] = 3;
                    m[3] = 4;
                    m[4] = 5;
                    m[5] = 6;
                    m[6] = 7;
                }

                int S = 0;
                for (int n = 0; n<7; n++)
                {
                    S += m[n] * x[n];
                }
                int p = S % 11;

                if (p >= 10)
                {
                    for (int n = 0; n<7; n++)
                    {
                        m[n] += 2;
                    }
                    S = 0;
                    for (int n = 0; n < 7; n++)
                    {
                        S += m[n] * x[n];
                    }
                    p = S % 11;
                }
                return p == x[7];
            }
            else
            {
                return false;
            }
        }

        public string EDRPOU
        {
            get
            {
                return IntToStr(_EDRPOU, 8);
            }
            set
            {
                if ((value != EDRPOU) && (value != null) && (value != string.Empty))
                {
                    try
                    {
                        if ((value == null) || (value.Length != 8) || (!CheckEDRPOU(value)))
                        {
                            throw new FormatException("EDRPOU must contains 8 digits");
                        }
                        _EDRPOU = Convert.ToInt32(value.Replace(" ", ""));
                    }
                    catch
                    {
                        throw new FormatException("EDRPOU must contains 8 digits");
                    }
                    Changed = true;
                }
            }
        }

        private long _Account;

        public string Account
        {
            get
            {
                return IntToStr(_Account, 14);
            }
            set
            {
                if ((value != Account) && (value != null) && (value != string.Empty))
                {
                    try
                    {
                        if ((value == null) || (value.Length != 14))
                        {
                            throw new FormatException("Account must contains 14 digits");
                        }
                        _Account = Convert.ToInt64(value.Replace(" ", ""));
                    }
                    catch
                    {
                        throw new FormatException("Account must contains 14 digits");
                    }
                    Changed = true;
                }
            }
        }

        private string _BankName;

        public string BankName
        {
            get
            {
                return _BankName;
            }
            set
            {
                if (value != _BankName)
                {
                    _BankName = value;
                    Changed = true;
                }
            }
        }

        static public Company Create()
        {
            Company company = new Company();
            company.Changed = true;
            return company;
        }

        static public Company Create(CompanyType companyType)
        {
            switch (companyType)
            {
                case CompanyType.air:
                    {
                        return new AirCompany();
                    }
                case CompanyType.isurance:
                    {
                        return new Insurance();
                    }
                case CompanyType.provider:
                    {
                        return new Provider();
                    }
                default:
                    {
                        throw new ArgumentException("Unhandled company type");
                    }
            }
        }

        public override void Load()
        {
            DBInterface.CommandText = "select officialCompanyName, note, edrpou, MFO, account, bankName from company where idCompany = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, CompanyID);
            DataTable tab = DBInterface.ExecuteSelection();

            if ((tab != null) && (tab.Rows.Count == 1))
            {
                FullName = tab.Rows[0]["officialCompanyName"].ToString();
                MFO = tab.Rows[0]["MFO"].ToString();
                EDRPOU = tab.Rows[0]["edrpou"].ToString();
                Account = tab.Rows[0]["account"].ToString();
                BankName = tab.Rows[0]["bankName"].ToString();
                Description = tab.Rows[0]["note"].ToString();
            } 
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Company table has rows with same id");
            }

            ContactList.Load();
            labelList.Load();

            Changed = false;
        }

        public override void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "UPDATE `sellcontroller`.`company` " +
                                                "SET " +
                                                "`officialCompanyName` = @name, " +
                                                "`MFO` = @mfo, " +
                                                "`edrpou` = @edrpou, " +
                                                "`account` = @account, " +
                                                "`bankName` = @bankName, " +
                                                "`note` = @desc " +
                                                "WHERE `idCompany` = @id;";

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, CompanyID);
                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, FullName);
                    DBInterface.AddParameter("@desc", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@mfo", MySql.Data.MySqlClient.MySqlDbType.String, MFO);
                    DBInterface.AddParameter("@edrpou", MySql.Data.MySqlClient.MySqlDbType.String, EDRPOU);
                    DBInterface.AddParameter("@account", MySql.Data.MySqlClient.MySqlDbType.String, Account);
                    DBInterface.AddParameter("@bankName", MySql.Data.MySqlClient.MySqlDbType.String, BankName);

                    DBInterface.ExecuteTransaction();

                    RaiseUpdated(false);
                }
                else
                {
                    DBInterface.CommandText = "insert into person (isPeople) values (0); " +
                                                "insert into company (idPerson, officialCompanyName, note, MFO, edrpou, account, bankName) " +
                                                "values (LAST_INSERT_ID(), @name, @desc, @mfo, @edrpou, @account, @bankName);";

                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, FullName);
                    DBInterface.AddParameter("@desc", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@mfo", MySql.Data.MySqlClient.MySqlDbType.String, MFO);
                    DBInterface.AddParameter("@edrpou", MySql.Data.MySqlClient.MySqlDbType.String, EDRPOU);
                    DBInterface.AddParameter("@account", MySql.Data.MySqlClient.MySqlDbType.String, Account);
                    DBInterface.AddParameter("@bankName", MySql.Data.MySqlClient.MySqlDbType.String, BankName);

                    CompanyID = Convert.ToInt32(DBInterface.ExecuteTransaction());

                    DBInterface.CommandText = "select idPerson from company where idCompany = @id";
                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, CompanyID);

                    DataTable tab = DBInterface.ExecuteSelection();
                    ID = Convert.ToInt32(tab.Rows[0]["idPerson"]);

                    RaiseUpdated(true);
                }

                Changed = false;
            }
        }

        public override void Delete()
        {
            DBInterface.CommandText = "delete from company where idCompany = @id";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, CompanyID);
            DBInterface.ExecuteTransaction();

            base.Delete();
        }
    }

    public class AirCompany : Company
    {
        public AirCompany()
        {
            companyType = CompanyType.air;
        }
    }

    public class Insurance : Company
    {
        public Insurance()
        {
            companyType = CompanyType.isurance;
        }
    }

    public class Provider : Company
    {
        public Provider()
        {
            companyType = CompanyType.provider;
        }
    }
}