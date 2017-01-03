using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelController.Models
{
    public abstract class Contact
    {
        protected enum ContactType { email, mobile, adress };

        protected ContactType _contactType;

        public string contactType
        {
            get 
            { 
                string str = string.Empty;
                if (DescriptionMapping.TryGetValue(_contactType, out str))
                    return str;
                throw new ArgumentException("Unhandled contact type");
            }
        }

        public abstract string Content { get; set; }

        public string Description { get; set; }

        public int ID { get; set; }


        static private Dictionary<ContactType, string> DescriptionMapping;

        static Contact()
        {
            DescriptionMapping = new Dictionary<ContactType, string>();

            DescriptionMapping.Add(ContactType.adress, "adress");
            DescriptionMapping.Add(ContactType.email, "e-mail");
            DescriptionMapping.Add(ContactType.mobile, "mobile");
        }

        static public Contact Create(string contactType)
        {
            try
            {
                KeyValuePair<ContactType, string> item = DescriptionMapping.First(pair => pair.Value == contactType);
                switch (item.Key)
                {
                    case ContactType.adress:
                        {
                            return new Adress();
                        }
                    case ContactType.email:
                        {
                            return new Email();
                        }
                    case ContactType.mobile:
                        {
                            return new MobilePhone();
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Contact type description has incorrect format");
            }
        }
    }

    public class Email : Contact
    {
        public Email()
        {
            _contactType = ContactType.email;
        }

        public string MainPart { get; set; }

        public string Domen { get; set; }

        public override string Content
        {
            get
            {
                return MainPart + "@" + Domen;
            }
            set
            {
                if (value != string.Empty)
                {
                    string[] str = value.Split('@');
                    if (str.Length != 2)
                        throw new ArgumentException("Email has incorrect format");
                    MainPart = str[0];
                    Domen = str[1];
                }
            }
        }
    }

    public class MobilePhone : Contact
    {
        public MobilePhone()
        {
            _contactType = ContactType.mobile;
        }

        public string CountryCode { get; set; }

        public string OperatorCode { get; set; }

        public string PrivateNumber { get; set; }

        public override string Content
        {
            get
            {
                return "+" + CountryCode + OperatorCode + PrivateNumber;
            }
            set
            {
                if (value != string.Empty)
                {
                    string str = value;
                    if (value.StartsWith("+"))
                        str = str.Remove(0, 1);

                    try
                    {
                        CountryCode = str.Substring(0, 3);
                        OperatorCode = str.Substring(3, 2);
                        PrivateNumber = str.Substring(5);
                    }
                    catch
                    {
                        throw new ArgumentException("Mobile phone has incorrect format");
                    }
                }
            }
        }
    }

    public class Adress : Contact
    {
        public Adress()
        {
            _contactType = ContactType.adress;
            _Content = string.Empty;
        }

        private string _Content;

        public override string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
            }
        }
    }
}