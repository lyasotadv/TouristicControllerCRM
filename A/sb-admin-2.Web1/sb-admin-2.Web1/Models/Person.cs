using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models.Mapping;

namespace sb_admin_2.Web1.Models
{
    public class PersonList : List<PersonGeneral>
    {
        
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

    public abstract class PersonGeneral
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
            ContactList = new List<Contact>();
        }

        public List<Contact> ContactList { get; private set; }
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