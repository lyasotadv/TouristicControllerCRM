using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class ContactList : DBObjectList<Contact>
    {
        public PersonGeneral person { get; set; }

        public override void Load()
        {
            Clear();

            DBInterface.CommandText = "select " +
                                        "contact.idContact, " +
                                        "typecontact.nameContact, " +
                                        "contact.value, " +
                                        "contact.description " +
                                        "from contact " +
                                        "left join person " +
                                        "on contact.idPerson = person.idPerson " +
                                        "left join typecontact " +
                                        "on contact.idTypeContact = typecontact.idTypeContact " +
                                        "where " +
                                        "person.idPerson = @id;";

            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);

            DataTable tab = DBInterface.ExecuteSelection();
            foreach (DataRow row in tab.Rows)
            {
                Contact item = Contact.Create(Convert.ToString(row["nameContact"]));
                item.ID = Convert.ToInt32(row["idContact"]);
                item.Content = Convert.ToString(row["value"]);
                item.Description = Convert.ToString(row["description"]);
                Add(item);
            }
        }

        public Contact Create(string contactType)
        {
            Contact contact = Contact.Create(contactType);
            Init(contact);
            contact.person = person;
            return contact;
        }
    }

    public abstract class Contact : IDBObject
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

        protected abstract string GetContent();

        protected abstract void SetContent(string val);

        public string Content 
        { 
            get
            {
                return GetContent();
            }
            set
            {
                if (value != GetContent())
                {
                    SetContent(value);
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

        public PersonGeneral person { get; set; }

        protected Contact()
        {
            Changed = false;
            ID = -1;
        }


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

        public event EventHandler Updated;

        public void Load()
        {
            DBInterface.CommandText = "SELECT `contact`.`idContact`, `contact`.`value` FROM `sellcontroller`.`contact` WHERE `idContact` = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                Content = Convert.ToString(tab.Rows[0]["value"]);
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Contact table has rows with same id");
            }

            Changed = false;
        }

        public void Delete()
        {
            if (ID >= 0)
            {
                DBInterface.CommandText = "DELETE FROM `sellcontroller`.`contact` WHERE `idContact` = @id;";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();

                if (Updated != null)
                {
                    Updated(this, new DBEventArgs() { ForceUpdate = true });
                }
            }
        }

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "UPDATE sellcontroller.contact SET value = @content, description = @desc WHERE idContact = @id;";
                    DBInterface.AddParameter("@content", MySql.Data.MySqlClient.MySqlDbType.String, Content);
                    DBInterface.AddParameter("@desc", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    InsertRow insertRow = new InsertRow("contact");
                    insertRow.Add("idTypeContact", MySql.Data.MySqlClient.MySqlDbType.Int32, ContactTypeID());
                    insertRow.Add("idPerson", MySql.Data.MySqlClient.MySqlDbType.String, person.ID);
                    insertRow.Add("value", MySql.Data.MySqlClient.MySqlDbType.String, Content);
                    insertRow.Add("description", MySql.Data.MySqlClient.MySqlDbType.String, Description);
                    insertRow.Execute();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = true });
                    }
                }

                Changed = false;
            }
        }

        private int ContactTypeID()
        {
            DBInterface.CommandText = "select idTypeContact from typecontact where nameContact = @name";
            DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, contactType);
            DataTable tab = DBInterface.ExecuteSelection();
            if ((tab != null)||(tab.Rows.Count == 1))
            {
                return Convert.ToInt32(tab.Rows[0]["idTypeContact"]);
            }
            else
            {
                throw new ArgumentException("Unhandled contacy type");
            }
        }

        public bool Changed { get; private set; }
    }

    public class Email : Contact
    {
        public Email()
        {
            _contactType = ContactType.email;
        }

        public string MainPart { get; set; }

        public string Domen { get; set; }

        protected override string GetContent()
        {
            return MainPart + "@" + Domen;
        }

        protected override void SetContent(string val)
        {
            if (val != string.Empty)
            {
                string[] str = val.Split('@');
                if (str.Length != 2)
                    throw new ArgumentException("Email has incorrect format");
                MainPart = str[0];
                Domen = str[1];
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

        protected override string GetContent()
        {
            return "+" + CountryCode + OperatorCode + PrivateNumber;
        }

        protected override void SetContent(string val)
        {
            if (val != string.Empty)
            {
                string str = val;
                if (val.StartsWith("+"))
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

    public class Adress : Contact
    {
        public Adress()
        {
            _contactType = ContactType.adress;
            _Content = string.Empty;
        }

        private string _Content;

        protected override string GetContent()
        {
            return _Content;
        }

        protected override void SetContent(string val)
        {
            _Content = val;
        }
    }
}