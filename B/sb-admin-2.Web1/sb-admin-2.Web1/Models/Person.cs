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

                    this.Add(person);
                }
                else
                {
                    Company company = new Company();

                    company.ID = Convert.ToInt32(row["idPerson"]);
                    company.CompanyID = Convert.ToInt32(row["idCompany"]);
                    company.FullName = row["officialCompanyName"].ToString();

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

        protected PersonGeneral()
        {
            personType = new PersonType(this);
            ContactList = new ContactList();
            ContactList.person = this;

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

        public virtual void Load()
        {
            
        }

        public virtual void Save()
        {
            
        }

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

        public PassportList PassportList { get; private set; }

        public Country _Citizen;

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
                                                "`birthDate` = @birthDate, " +
                                                "`Note` = @desc, " +
                                                "`gender` = @gender " +
                                                "WHERE `idPeople` = @id;";

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonID);
                    DBInterface.AddParameter("@firstName", MySql.Data.MySqlClient.MySqlDbType.String, FirstName);
                    DBInterface.AddParameter("@middleName", MySql.Data.MySqlClient.MySqlDbType.String, MiddleName);
                    DBInterface.AddParameter("@lastName", MySql.Data.MySqlClient.MySqlDbType.String, SecondName);
                    DBInterface.AddParameter("@birthDate", MySql.Data.MySqlClient.MySqlDbType.DateTime, Birth);
                    DBInterface.AddParameter("@desc", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@gender", MySql.Data.MySqlClient.MySqlDbType.Int32, GenderID());

                    DBInterface.ExecuteTransaction();

                    RaiseUpdated(false);
                }
                else
                {
                    DBInterface.CommandText = "insert into person (isPeople) values (1); " +
                                                "insert into people (idPerson, firstName, middleName, lastName, Note, birthDate, gender) " +
                                                "values (LAST_INSERT_ID(), @firstName, @middleName, @lastName, @desc, @birthDate, @gender);";

                    DBInterface.AddParameter("@firstName", MySql.Data.MySqlClient.MySqlDbType.String, FirstName);
                    DBInterface.AddParameter("@middleName", MySql.Data.MySqlClient.MySqlDbType.String, MiddleName);
                    DBInterface.AddParameter("@lastName", MySql.Data.MySqlClient.MySqlDbType.String, SecondName);
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
    }

    public class Company : PersonGeneral
    {
        public int CompanyID { get; set; }

        public enum CompanyType { air, isurance, provider }

        protected CompanyType companyType;

        public string Kod { get; set; }

        public string OfficialName { get; set; }

        private string _FullName;

        public override string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
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