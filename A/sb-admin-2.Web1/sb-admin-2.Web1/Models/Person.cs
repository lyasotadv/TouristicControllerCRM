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
            throw new NotImplementedException("Waiting for stored procedure");
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

        public string Description { get; set; }

        public PersonGeneral Parent { get; set; }

        protected PersonGeneral()
        {
            personType = new PersonType(this);
            ContactList = new ContactList();
            ContactList.person = this;

            Changed = false;
            ID = -1;
        }

        public ContactList ContactList { get; private set; }


        public event EventHandler Updated;

        public virtual void Load()
        {
            
        }

        public virtual void Save()
        {
            
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
        private enum Sex { male, female };

        private Sex _sex;

        public string FirstName { get; set; }
        
        public string SecondName { get; set; }

        public string MiddleName { get; set; }

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
            }
        }

        public DateTime Birth { get; set; }

        public string BirthStr
        {
            get { return Birth.ToString("ddMMMyy", Preferences.cultureInfo); }
            set { throw new NotImplementedException("Setter for BirthStr havnt been implemented"); }
        }

        public override string FullName
        {
            get { return FirstName + " " + MiddleName + " " + SecondName; }
            set { throw new NotImplementedException("There is no parser of full name for person"); }
        }

        public List<Passport> PassportList { get; private set; }

        public Country Citizen { get; set; }

        public Person()
        {
            PassportList = new List<Passport>();
        }


        public override void Load()
        {
            DBInterface.CommandText = "SELECT `people`.`idPeople`," + 
                                        "`people`.`idPerson`," +
                                        "`people`.`firstName`," + 
                                        "`people`.`middleName`," + 
                                        "`people`.`lastName`," + 
                                        "`people`.`birthDate`," +
                                        "`people`.`Note`," +
                                        "`people`.`itn`" +
                                        "FROM `sellcontroller`.`people` WHERE `idPeople` = @id;";

            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                FirstName = Convert.ToString(tab.Rows[0]["firstName"]);
                SecondName = Convert.ToString(tab.Rows[0]["lastName"]);
                MiddleName = Convert.ToString(tab.Rows[0]["middleName"]);
                Birth = Convert.ToDateTime(tab.Rows[0]["birthDate"]);
                Description = Convert.ToString(tab.Rows[0]["Note"]);
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("People table has rows with same id");
            }

            ContactList.Load();

            Changed = false;
        }

        public override void Save()
        {
            throw new NotImplementedException("Waiting for new person store stucture");

            if (Changed)
            {
                if (ID >= 0)
                {
                    //To Do
                    RaiseUpdated(false);
                }
                else
                {
                    //To Do
                    RaiseUpdated(true);
                }

                Changed = false;
            }
        }
    }

    public class Company : PersonGeneral
    {
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