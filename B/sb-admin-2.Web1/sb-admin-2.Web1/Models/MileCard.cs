using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class MileCardList
    {

    }

    public class MileCard
    {

    }

    public class MileCardStatusList : DBObjectList<MileCardStatus>
    {
        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.milecardstatus;";

            DataTable tab = DBInterface.ExecuteSelection();

            foreach (DataRow row in tab.Rows)
            {
                MileCardStatus mcs = new MileCardStatus();

                mcs.ID = Convert.ToInt32(row["idMileCardStatus"]);
                mcs.Name = row["status"].ToString();
                mcs.MinVal = Convert.ToInt32(row["minMiles"]);
                mcs.MaxVal = Convert.ToInt32(row["maxMiles"]);
                
                if (row["idAviaCompanyUnion"] != DBNull.Value)
                    mcs.AviaCompanyUnionID = Convert.ToInt32(row["idAviaCompanyUnion"]);

                if (row["idAviaCompany"] != DBNull.Value)
                    mcs.AviaCompanyID = Convert.ToInt32(row["idAviaCompany"]);
                
                mcs.Note = row["note"].ToString();

                this.Add(mcs);
            }
        }

        public MileCardStatus Create()
        {
            MileCardStatus mcs = new MileCardStatus();
            Init(mcs);
            return mcs;
        }
    }

    public class MileCardStatus : IDBObject
    {
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
                if (value != Name)
                {
                    _Name = value;
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
                if (value != Note)
                {
                    _Note = value;
                    Changed = true;
                }
            }
        }

        private int _MinVal;

        public int MinVal
        {
            get
            {
                return _MinVal;
            }
            set
            {
                if (value != MinVal)
                {
                    if (value < 0)
                    {
                        _MinVal = 0;
                    }
                    else
                    {
                        _MinVal = value;
                    }
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// 0 is equal to infinity
        /// </summary>
        private int _MaxVal;

        public int MaxVal
        {
            get
            {
                return _MaxVal;
            }
            set
            {
                if (value != MaxVal)
                {
                    if (value < 0)
                    {
                        _MaxVal = 0;
                    }
                    else
                    {
                        _MaxVal = value;
                    }
                    Changed = true;
                }
            }
        }

        public AviaCompany aviacompany { get; private set; }

        public AviaCompanyUnion aviacompanyunion { get; private set; }

        public int AviaCompanyID
        {
            get
            {
                return aviacompany.ID;
            }
            set
            {
                if (value != AviaCompanyID)
                {
                    aviacompany.ID = value;
                    aviacompany.Load();
                    Changed = true;
                }
            }
        }

        public int AviaCompanyUnionID
        {
            get
            {
                return aviacompanyunion.ID;
            }
            set
            {
                if (value != AviaCompanyUnionID)
                {
                    aviacompanyunion.ID = value;
                    aviacompanyunion.Load();
                    Changed = true;
                }
            }
        }

        public string RangeStr
        {
            get
            {
                string str = string.Empty;

                if ((MinVal > 0) & (MaxVal == 0))
                {
                    str = "> " + MinVal.ToString();
                }

                if ((MinVal == 0) & (MaxVal > 0))
                {
                    str = "< " + MaxVal.ToString();
                }

                if ((MinVal > 0) & (MaxVal > 0))
                {
                    str = MinVal.ToString() + " - " + MaxVal.ToString();
                }

                return str;
            }
        }

        public MileCardStatus()
        {
            ID = -1;

            aviacompany = new AviaCompany();
            aviacompanyunion = new AviaCompanyUnion();
        }

        public event EventHandler Updated;

        public void Load()
        {
            DBInterface.StoredProcedure("mile_card_status_select_by_id");
            DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.AddOutParameter("@outStatus", MySql.Data.MySqlClient.MySqlDbType.String);
            DBInterface.AddOutParameter("@outMinMiles", MySql.Data.MySqlClient.MySqlDbType.Int32);
            DBInterface.AddOutParameter("@outMaxMiles", MySql.Data.MySqlClient.MySqlDbType.Int32);
            DBInterface.AddOutParameter("@outIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32);
            DBInterface.AddOutParameter("@outIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32);
            DBInterface.AddOutParameter("@outNote", MySql.Data.MySqlClient.MySqlDbType.String);

            DBInterface.ExecuteTransaction();

            Name = Convert.ToString(DBInterface.GetOutParameter("@outStatus"));
            MinVal = Convert.ToInt32(DBInterface.GetOutParameter("@outMinMiles"));
            MaxVal = Convert.ToInt32(DBInterface.GetOutParameter("@outMaxMiles"));

            if (DBInterface.GetOutParameter("@outIdAviaCompany") != DBNull.Value)
                AviaCompanyID = Convert.ToInt32(DBInterface.GetOutParameter("@outIdAviaCompany"));

            if (DBInterface.GetOutParameter("@outIdAviaCompanyUnion") != DBNull.Value)
                AviaCompanyUnionID = Convert.ToInt32(DBInterface.GetOutParameter("@outIdAviaCompanyUnion"));
            
            Note = Convert.ToString(DBInterface.GetOutParameter("@outNote"));

            Changed = false;
        }

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.StoredProcedure("mile_card_status_update");

                    DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    DBInterface.AddParameter("@inStatus", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@inMinMiles", MySql.Data.MySqlClient.MySqlDbType.Int32, MinVal);
                    DBInterface.AddParameter("@inMaxMiles", MySql.Data.MySqlClient.MySqlDbType.Int32, MaxVal);
                    
                    if (AviaCompanyID < 0)
                    {
                        DBInterface.AddParameter("@inIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32, AviaCompanyID);
                    }

                    if (AviaCompanyUnionID < 0)
                    {
                        DBInterface.AddParameter("@inIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, AviaCompanyUnionID);
                    }
                    
                    
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Note);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    DBInterface.StoredProcedure("mile_card_status_insert");
                    
                    DBInterface.AddParameter("@inStatus", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@inMinMiles", MySql.Data.MySqlClient.MySqlDbType.Int32, MinVal);
                    DBInterface.AddParameter("@inMaxMiles", MySql.Data.MySqlClient.MySqlDbType.Int32, MaxVal);
                    
                    if (AviaCompanyID < 0)
                    {
                        DBInterface.AddParameter("@inIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32, AviaCompanyID);
                    }

                    if (AviaCompanyUnionID < 0)
                    {
                        DBInterface.AddParameter("@inIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32, AviaCompanyUnionID);
                    }
                    
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Note);

                    DBInterface.AddOutParameter("@outIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32);

                    DBInterface.ExecuteTransaction();

                    ID = Convert.ToInt32(DBInterface.GetOutParameter("@outIdMileCardStatus"));

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
            DBInterface.StoredProcedure("mile_card_status_delete");
            DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.ExecuteTransaction();
        }

        public bool Changed { get; private set; }
    }
}