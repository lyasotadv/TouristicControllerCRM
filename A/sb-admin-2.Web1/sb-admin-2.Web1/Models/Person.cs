using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models.Mapping;

namespace sb_admin_2.Web1.Models
{
    public class PersonList : List<Person>
    {
        
    }

    public class Person
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

        public string Organization { get; set; }

        public string Description { get; set; }

        public DateTime Birth { get; set; }

        public string BirthStr
        {
            get { return Birth.ToString("ddMMMyy", Preferences.cultureInfo); }
            set { throw new NotImplementedException("Setter for BirthStr havnt been implemented"); }
        }

        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + SecondName; }   
        }

        public List<Passport> PassportList { get; private set; }

        public Country Citizen { get; set; }

        public List<Contact> ContactList { get; private set; }

        public Person()
        {
            PassportList = new List<Passport>();
            ContactList = new List<Contact>();
        }
    }
}