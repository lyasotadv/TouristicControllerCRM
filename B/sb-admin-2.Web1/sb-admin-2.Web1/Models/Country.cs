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
                    item.Load();
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

    public class Country : IDBObject, IVizaFormation
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

        public CountryUnionList mirror { get; private set; }

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

            UnionList.Load(this);
            mirror = UnionList.mirror;

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
        public CountryUnionList mirror { get; private set; }

        private void UpdateMirror()
        {
            if (mirror == null)
                mirror = new CountryUnionList();
            mirror.Clear();
            mirror.Load();
            foreach (var cu in this)
            {
                mirror.RemoveAll(item => item.ID == cu.ID);
            }
        }

        public CountryUnionList()
        {

        }

        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * from sellcontroller.countryunion";

            DataTable tab = DBInterface.ExecuteSelection();

            foreach (DataRow row in tab.Rows)
            {
                CountryUnion item = Create();

                item.ID = Convert.ToInt32(row["idCountryUnion"]);
                item.Name = row["UnionName"].ToString();
                item.ShortName = row["shortUnionName"].ToString();
                item.Note = row["note"].ToString();
            }
        }

        public void Load(Country country)
        {
            Clear();

            if (country == null)
                return;

            DBInterface.CommandText = "select " +
                                        "countryunion.idCountryUnion, " +
                                        "countryunion.UnionName, " +
                                        "countryunion.shortUnionName, " +
                                        "countryunion.note " +
                                        "from joincountryunion " +
                                        "left join countryunion " +
                                        "on joincountryunion.idCountryUnion = countryunion.idCountryUnion " +
                                        "where joincountryunion.idCountry = @idCountry";

            DBInterface.AddParameter("@idCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, country.ID);

            DataTable tab = DBInterface.ExecuteSelection();

            foreach (DataRow row in tab.Rows)
            {
                CountryUnion item = Create();
                item.ID = Convert.ToInt32(row["idCountryUnion"]);
                item.Name = row["UnionName"].ToString();
                item.ShortName = row["shortUnionName"].ToString();
                item.Note = row["note"].ToString();
            }

            UpdateMirror();
        }

        public void AddElement(Country country, CountryUnion countryUnion)
        {
            DBInterface.CommandText = "select * " +
                                        "from joincountryunion " +
                                        "where joincountryunion.idCountry = @idCountry " +
                                        "and joincountryunion.idCountryUnion = @idCountryUnion;";

            DBInterface.AddParameter("@idCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, country.ID);
            DBInterface.AddParameter("@idCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, countryUnion.ID);

            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 0)
            {
                DBInterface.StoredProcedure("join_Country_union_insert");

                DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, countryUnion.ID);
                DBInterface.AddParameter("@inIdCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, country.ID);
                DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, "");

                DBInterface.AddOutParameter("@outIdJoinCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32);

                DBInterface.ExecuteTransaction();
            }
        }

        public void RemoveElement(Country country, CountryUnion countryUnion)
        {
            DBInterface.CommandText = "select * " +
                                        "from joincountryunion " +
                                        "where joincountryunion.idCountry = @idCountry " +
                                        "and joincountryunion.idCountryUnion = @idCountryUnion;";

            DBInterface.AddParameter("@idCountry", MySql.Data.MySqlClient.MySqlDbType.Int32, country.ID);
            DBInterface.AddParameter("@idCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, countryUnion.ID);

            DataTable tab = DBInterface.ExecuteSelection();

            foreach (DataRow row in tab.Rows)
            {
                DBInterface.StoredProcedure("join_Country_union_delete");
                DBInterface.AddParameter("@inIdJoinCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, Convert.ToInt32(row["idJoinCountryUnion"]));
                DBInterface.ExecuteTransaction();
            }
        }

        public CountryUnion Create()
        {
            CountryUnion union = new CountryUnion();
            Init(union);
            Add(union);
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

    public class CountryUnion : IDBObject, IVizaFormation
    {
        public event EventHandler Updated;

        public int ID { get; set; }

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    Changed = true;
                }
            }
        }

        private string _ShortName;

        public string ShortName
        {
            get
            {
                return _ShortName;
            }
            set
            {
                if (value != _ShortName)
                {
                    _ShortName = value;
                    Changed = true;
                }
            }
        }

        private string _Note;

        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                if (value != _Note)
                {
                    _Note = value;
                    Changed = true;
                }
            }
        }

        public CountryUnion()
        {
            Changed = false;
            ID = -1;
        }

        public void Load()
        {
            if (ID != -1)
            {
                IDBInterface db = DBInterface.CreatePointer();

                db.StoredProcedure("Country_union_select_by_id");

                db.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

                db.AddOutParameter("@outUnionName", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outShortUnionName", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outNote", MySql.Data.MySqlClient.MySqlDbType.String);

                db.Execute();


                Name = db.GetOutParameterStr("@outUnionName");
                ShortName = db.GetOutParameterStr("@outShortUnionName");
                Note = db.GetOutParameterStr("@outNote");

                Changed = false;
            }
        }

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.StoredProcedure("country_union_update");

                    DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.AddParameter("@inUnionName", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@inShortUnionName", MySql.Data.MySqlClient.MySqlDbType.String, ShortName);
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Note);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    DBInterface.StoredProcedure("country_union_insert");

                    DBInterface.AddParameter("@inUnionName", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@inShortUnionName", MySql.Data.MySqlClient.MySqlDbType.String, ShortName);
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Note);

                    DBInterface.AddOutParameter("@outIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32);

                    DBInterface.ExecuteTransaction();

                    ID = Convert.ToInt32(DBInterface.GetOutParameter("@outIdCountryUnion"));

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
            DBInterface.StoredProcedure("Country_union_delete");
            DBInterface.AddParameter("@inIdCountryUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.ExecuteTransaction();
        }

        public bool Changed { get; private set; }
    }

    public interface IVizaFormation
    {
        int ID { get; set; }

        string Name { get; }

        string ShortName { get; }

        void Load();
    }
}