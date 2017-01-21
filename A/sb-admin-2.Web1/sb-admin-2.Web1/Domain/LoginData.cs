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
        public int ID;

        public string Name { get; set; }

        public string Password { get; set; }

        private string PassHash()
        {
            int h = Password.GetHashCode();
            return h.ToString();
        }

        public bool Check()
        {
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
                if (tab.Rows[0]["hashcode"].ToString() == PassHash())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}