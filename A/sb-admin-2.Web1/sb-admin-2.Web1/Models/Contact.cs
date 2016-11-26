using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class ContactList : ModelList<Contact>
    {

    }

    public class Contact : IDebugModel
    {
        private enum ContactType { email, mobile };

        private ContactType _contactType;

        public string contactType
        {
            get 
            { 
                switch(_contactType)
                {
                    case ContactType.email:
                        {
                            return "e-mail";
                        }
                    case ContactType.mobile:
                        {
                            return "mobile";
                        }
                    default:
                        {
                            throw new ArgumentException("Unhandled contact type");
                        }
                }
            }
            set
            {
                switch(value)
                {
                    case "e-mail":
                        {
                            _contactType = ContactType.email;
                            break;
                        }
                    case "mobile":
                        {
                            _contactType = ContactType.mobile;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Incorrect contact type");
                        }
                }
            }
        }

        public string Content { get; set; }

        public string Description { get; set; }

        public void CreateTestData()
        {
            _contactType = ContactType.email;
            Content = @"blabla@gmail.com";
            Description = "Lorem ipsum";
        }
    }
}