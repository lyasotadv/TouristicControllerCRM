using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Domain
{
    public class LoginData
    {
        public string TestConnection
        {
            get
            {
                return DBInterface.DBTestConnection;
            }
        }

        public class UserRole
        {
            private enum RoleEnum { guest, admin, god };

            private RoleEnum role { get; set; }

            private Dictionary<RoleEnum, string> dict;

            public UserRole()
            {
                role = RoleEnum.guest;

                dict = new Dictionary<RoleEnum, string>();
                dict.Add(RoleEnum.guest, "guest");
                dict.Add(RoleEnum.admin, "admin");
                dict.Add(RoleEnum.god, "god");
            }

            public string RoleString
            {
                get
                {
                    string str = string.Empty;
                    if (dict.TryGetValue(role, out str))
                        return str;
                    return null;
                }
                set
                {
                    try
                    {
                        KeyValuePair<RoleEnum, string> item = dict.First(a => a.Value == value);
                        role = item.Key;
                    }
                    catch
                    {
                        throw new ArgumentNullException("Incorrect user role");
                    }
                }
            }

            public void SetByInt(int val)
            {
                switch (val)
                {
                    case 0: { role = RoleEnum.guest; break; }
                    case 1: { role = RoleEnum.admin; break; }
                    case 2: { role = RoleEnum.god; break; }
                }
            }
        }

        public UserRole role { get; set; }

        public int ID { get; set; }

        public string FullName { get; set; }

        public string Name { get; set; }

        public string NameNew { get; set; }

        public string Password { get; set; }

        public string PasswordNew { get; set; }

        public string PasswordConfirm { get; set; }

        public bool ChangePassword { get; set; }

        public LoginData()
        {
            ChangePassword = false;

            FullName = null;
            Name = null;
            NameNew = null;
            Password = null;
            PasswordNew = null;
            PasswordConfirm = null;

            role = new UserRole();
        }

        private string PassHash()
        {
            return PassHash(Password);
        }

        private string PassHash(string pass)
        {
            int h = pass.GetHashCode();
            return h.ToString();
        }

        private bool IsUserEmpty()
        {
            DBInterface.CommandText = "SELECT * FROM sellcontroller.user;";
            DataTable tab = DBInterface.ExecuteSelection();
            return tab.Rows.Count == 0;
        }

        public bool Check(out string hash)
        {
            hash = PassHash();
            try
            {
                DBInterface.CommandText = "SELECT * FROM sellcontroller.user WHERE login = @name;";
                DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                DataTable tab = DBInterface.ExecuteSelection();

                if (tab.Rows.Count == 0)
                    return false;

                if (tab.Rows.Count > 1)
                    throw new ArgumentException("User with current id is not unique.");

                ID = Convert.ToInt32(tab.Rows[0]["idUser"]);
                return tab.Rows[0]["hashcode"].ToString() == hash;
            }
            catch
            {
                return false;
            }
        }

        public void Load()
        {
            DBInterface.CommandText = "SELECT * FROM sellcontroller.user WHERE login = @name;";
            DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count != 1)
                throw new ArgumentException("User with current id is not unique.");

            FullName = Convert.ToString(tab.Rows[0]["Name"]);
            role.SetByInt(Convert.ToInt32(tab.Rows[0]["isAdminRole"]));
        }

        public void Save()
        {
            DBInterface.CommandText = "update sellcontroller.user set isAdminRole = @role, Name = @name where idUser = @id";
            DBInterface.AddParameter("@role", MySql.Data.MySqlClient.MySqlDbType.Int32, 1);
            DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, FullName);
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.ExecuteTransaction();

            string hash;
            if (ChangePassword && (PasswordNew != null) && (PasswordNew != string.Empty) && (PasswordNew == PasswordConfirm) 
                && (NameNew != null) && (NameNew != string.Empty) && Check(out hash))
            {
                DBInterface.CommandText = "update sellcontroller.user set login = @login, hashcode = @hash where idUser = @id";
                DBInterface.AddParameter("@login", MySql.Data.MySqlClient.MySqlDbType.String, NameNew);
                DBInterface.AddParameter("@hash", MySql.Data.MySqlClient.MySqlDbType.String, PassHash(PasswordNew));
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();
            }
        }

        public void AddNewUser(string FullName, string Name, bool IsAdmin)
        {
            if ((role.RoleString == "admin") && (FullName != null) && (FullName != string.Empty) && (Name != null) && (Name != string.Empty))
            {
                string pass = "123321";

                int IsAdminInt = 0;
                if (IsAdmin)
                    IsAdminInt = 1;

                InsertRow insertRow = new InsertRow("user");
                insertRow.Add("login", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                insertRow.Add("Name", MySql.Data.MySqlClient.MySqlDbType.String, FullName);
                insertRow.Add("isAdminRole", MySql.Data.MySqlClient.MySqlDbType.Int32, IsAdminInt);
                insertRow.Add("hashcode", MySql.Data.MySqlClient.MySqlDbType.String, PassHash(pass));

                insertRow.Execute();
            }
        }
    }
}