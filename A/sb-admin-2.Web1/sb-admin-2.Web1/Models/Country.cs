using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class CountryList : List<Country>
    {
        public void Update()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.country";
            DataTable tab = DBInterface.ExecuteSelection();
            foreach (DataRow row in tab.Rows)
            {
                Country item = new Country() { Name = Convert.ToString(row["nameCountry"]) };
                Add(item);
            }
        }

        public void AddData(string Name)
        {
            InsertRow insertRow = new InsertRow("country", "idCountry");
            insertRow.Add("nameCountry", MySql.Data.MySqlClient.MySqlDbType.String, Name);
            insertRow.Execute();

            Update();
        }
    }

    public class Country
    {
        public string Name { get; set; }

        public string ISO { get; set; }
    }
}