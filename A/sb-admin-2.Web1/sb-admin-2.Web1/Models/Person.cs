using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class PersonList : ModelList<Person>
    {
        
    }

    public class Person : IDebugModel
    {
        private enum Sex { male, female };

        private Sex _sex;

        public string FirstName { get; set; }
        
        public string SecondName { get; set; }

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

        public string FullName
        {
            get { return FirstName + " " + SecondName; }   
        }

        public void CreateTestData()
        {
            FirstName = "Lorem";
            SecondName = "Ipsum";
            Gender = "male";
            Organization = "Default";
            Birth = DateTime.Now.Date;
            Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
        }
    }
}