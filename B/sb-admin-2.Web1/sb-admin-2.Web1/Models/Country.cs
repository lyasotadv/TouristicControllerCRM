using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;
using sb_admin_2.Web1.Models.Mapping.ExpImpUtils;

namespace sb_admin_2.Web1.Models
{
    public abstract class CountryListGeneral : DBObjectList<Country>
    {
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


            for (int n = 0; n < comp.h; n++)
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

        protected void Append(DataTable tab)
        {
            if (tab != null)
            {
                foreach (DataRow row in tab.Rows)
                {
                    Country item = new Country() { ID = Convert.ToInt32(row["idCountry"]), Name = Convert.ToString(row["nameCountry"]) };
                    item.ISO = Convert.ToString(row["codeISO2"]);
                    item.ISO3 = Convert.ToString(row["codeISO3"]);
                    item.Nationality = Convert.ToString(row["codeCitizen"]);
                    Add(item);
                }
            }
            else
            {
                throw new ArgumentNullException("DataTable must be not null");
            }
        }
    }

    public class CountryList : CountryListGeneral
    {
        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.country";
            Append(DBInterface.ExecuteSelection());
        }
    }

    public class Country : IDBObject, VizaFormation
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

        private string _ISO;

        public string ISO 
        { 
            get
            {
                return _ISO;
            }
            set
            {
                if ((value != _ISO) & (value != null) & (value != string.Empty))
                {
                    _ISO = value;
                    Changed = true;
                }
            }
        }

        private string _ISO3;

        public string ISO3
        {
            get
            {
                return _ISO3;
            }
            set
            {
                if ((value != _ISO3) & (value != null) & (value != string.Empty))
                {
                    _ISO3 = value;
                    Changed = true;
                }
            }
        }

        public string ShortName
        {
            get
            {
                return ISO;
            }
        }

        private string _Nationality;

        public string Nationality
        {
            get
            {
                return _Nationality;
            }
            set
            {
                if ((value != _Nationality) & (value != null) & (value != string.Empty))
                {
                    _Nationality = value;
                    Changed = true;
                }
            }
        }

        public CountryUnionList UnionList { get; private set; }

        public Country()
        {
            ID = -1;
            Changed = false;

            UnionList = new CountryUnionList();
        }


        public event EventHandler Updated;

        public void Load()
        {
            DBInterface.CommandText = "SELECT nameCountry, codeISO2, codeISO3, codeCitizen FROM `sellcontroller`.`country` WHERE `idCountry` = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DataTable tab = DBInterface.ExecuteSelection();
            
            if (tab.Rows.Count == 1)
            {
                Name = Convert.ToString(tab.Rows[0]["nameCountry"]);
                ISO = Convert.ToString(tab.Rows[0]["codeISO2"]);
                ISO3 = Convert.ToString(tab.Rows[0]["codeISO3"]);
                Nationality = Convert.ToString(tab.Rows[0]["codeCitizen"]);
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
                    DBInterface.CommandText = "UPDATE `sellcontroller`.`country` SET `nameCountry` = @name, " +
                        "`codeISO2` = @iso2, `codeISO3` = @iso3, `codeCitizen` = @nat  WHERE `idCountry` = @id;";

                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@iso2", MySql.Data.MySqlClient.MySqlDbType.String, ISO);
                    DBInterface.AddParameter("@iso3", MySql.Data.MySqlClient.MySqlDbType.String, ISO3);
                    DBInterface.AddParameter("@nat", MySql.Data.MySqlClient.MySqlDbType.String, Nationality);
                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    InsertRow insertRow = new InsertRow("country");
                    insertRow.Add("nameCountry", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    insertRow.Add("codeISO2", MySql.Data.MySqlClient.MySqlDbType.String, ISO);
                    insertRow.Add("codeISO3", MySql.Data.MySqlClient.MySqlDbType.String, ISO3);
                    insertRow.Add("codeCitizen", MySql.Data.MySqlClient.MySqlDbType.String, Nationality);
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

    public class CountryUnionList : DBObjectList<CountryUnion>
    {
        public override void Load()
        {
            throw new NotImplementedException();
        }

        public CountryUnion Create()
        {
            CountryUnion union = new CountryUnion();
            Init(union);
            return union;
        }

        public override string ToString()
        {
            string str = string.Empty;
            foreach(var union in this)
            {
                if (str != string.Empty)
                {
                    str += ", ";
                }
                str += union.ShortName;
            }
            return str;
        }

        public string StrFormat
        {
            get
            {
                return ToString();
            }
        }
    }

    public class CountryUnion : CountryListGeneral, IDBObject, VizaFormation
    {
        public event EventHandler Updated;

        public int ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public override void Load()
        {
            throw new NotImplementedException("Country union is not implemented");

            DBInterface.CommandText = "";
            DBInterface.AddParameter("idUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            Append(DBInterface.ExecuteSelection());
        }

        public override void Save()
        {
            base.Save();
            throw new NotImplementedException("Country union is not implemented");
        }

        public void Delete()
        {
            throw new NotImplementedException("Country union is not implemented");
        }

        public bool Changed { get; private set; }
    }

    public interface VizaFormation
    {
        int ID { get; set; }

        string Name { get; }

        string ShortName { get; }
    }
}