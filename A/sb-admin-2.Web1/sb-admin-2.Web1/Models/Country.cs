using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;
using sb_admin_2.Web1.Models.Mapping.ExpImpUtils;

namespace sb_admin_2.Web1.Models
{
    public class CountryList : DBObjectList<Country>
    {
        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.country";
            DataTable tab = DBInterface.ExecuteSelection();
            foreach (DataRow row in tab.Rows)
            {
                Country item = new Country() { ID = Convert.ToInt32(row["idCountry"]), Name = Convert.ToString(row["nameCountry"]) };
                Add(item);
            }
        }

        public Country Create()
        {
            Country country = new Country();
            Init(country);
            return country;
        }

        public bool Export(string fileName)
        {
            ExcelEI comp = null;
            try
            {
                comp = new ExcelEI(fileName);
                comp.startrange = "A2";
                comp.w = 1;
                comp.AutoRange("A2", ExcelEI.Direction.down);
                comp.Read();
            }
            finally
            {
                if (comp != null)
                {
                    comp.Close();
                }
            }
            

            for (int n = 0; n < comp.h; n++ )
            {
                if (this.Find(item => item.Name == comp.Data[n, 0]) == null)
                {
                    Country country = Create();
                    country.Name = comp.Data[n, 0];
                    country.Save();
                }
            }
            return true;
        }
    }

    public class Country : IDBObject
    {
        public int ID { get; set; }

        private string _Name;

        public string Name 
        {
            get { return _Name; }
            set
            {
                if ((value != _Name) & (value != null) & (value != string.Empty))
                {
                    _Name = value;
                    Changed = true;
                }
            }
        }

        public string ISO { get; set; }

        public Country()
        {
            ID = -1;
            Changed = false;
        }


        public event EventHandler Updated;

        public void Load()
        {
            DBInterface.CommandText = "SELECT `country`.`idCountry`, `country`.`nameCountry` FROM `sellcontroller`.`country` WHERE `idCountry` = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DataTable tab = DBInterface.ExecuteSelection();
            
            if (tab.Rows.Count == 1)
            {
                Name = Convert.ToString(tab.Rows[0]["nameCountry"]);
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Country table has rows with same id");
            }

            Changed = false;
        }

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "UPDATE `sellcontroller`.`country` SET `nameCountry` = @name WHERE `idCountry` = @id;";
                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    InsertRow insertRow = new InsertRow("country", "idCountry");
                    insertRow.Add("nameCountry", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    insertRow.Execute();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = true });
                    }
                }

                Changed = false;
            }
        }

        public void Delete()
        {
            if (ID >= 0)
            {
                DBInterface.CommandText = "DELETE FROM `sellcontroller`.`country` WHERE `idCountry` = @id;";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();

                if (Updated != null)
                {
                    Updated(this, new DBEventArgs() { ForceUpdate = true });
                }
            }
        }

        public bool Changed { get; private set; }
    }
}