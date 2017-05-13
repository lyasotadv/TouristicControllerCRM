using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class MileCardList : DBObjectList<MileCard>
    {
        public PersonGeneral personOwner { get; private set; }
        
        public MileCardList(PersonGeneral personOwner)
        {
            this.personOwner = personOwner;
        }

        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.milecard where idOwnerPerson = @idPerson;";
            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, personOwner.ID);
            DataTable tab = DBInterface.ExecuteSelection();

            foreach (DataRow row in tab.Rows)
            {
                MileCard mc = Create();
                mc.ID = Convert.ToInt32(row["idMileCard"]);
                mc.Load();
                this.Add(mc);
            }
        }

        public MileCard Create()
        {
            MileCard mc = new MileCard(personOwner);
            Init(mc);
            return mc;
        }
    }

    public class MileCard : IDBObject
    {
        public int ID { get; set; }

        private string _Number;

        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                if (value != Number)
                {
                    _Number = value;
                    Changed = true;
                }
            }
        }

        private string _Password;

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (value != Password)
                {
                    _Password = value;
                    Changed = true;
                }
            }
        }

        private int _MilesCount;

        public int MilesCount
        {
            get
            {
                return _MilesCount;
            }
            set
            {
                if (value != MilesCount)
                {
                    _MilesCount = value;
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

        public string AviaUnionAndCompanyStr
        {
            get
            {
                string acu = string.Empty;
                if (AviaCompanyUnionID >= 0)
                    acu = aviacompanyunion.Name;

                string ac = string.Empty;
                if (AviaCompanyID >= 0)
                    ac = aviacompany.ICAO;

                string divider = string.Empty;
                if ((acu != string.Empty) & (ac != string.Empty))
                    divider = " / ";

                return acu + divider + ac;
            }
        }

        private PersonGeneral personOwner { get; set; }

        private PersonGeneral personResponded { get; set; }

        public int PersonRespondedID
        {
            get
            {
                if (personResponded == null)
                    return -1;
                return personResponded.ID;
            }
            set
            {
                if (value != PersonRespondedID)
                {
                    personResponded = PersonGeneral.Create(value);
                    personResponded.Load();
                    Changed = true;
                }
            }
        }

        public string PersonRespondedName
        {
            get
            {
                if (personResponded == null)
                    return string.Empty;
                return personResponded.FullName;
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

        public MileCardStatus mileCardStatus { get; private set; }

        public MileCard(PersonGeneral personOwner)
        {
            ID = -1;
            Changed = false;
            Silent = false;

            this.personOwner = personOwner;
            personResponded = null;

            aviacompany = new AviaCompany();
            aviacompanyunion = new AviaCompanyUnion();
            mileCardStatus = new MileCardStatus();
        }


        public event EventHandler Updated;

        public void Load()
        {
            IDBInterface db = DBInterface.CreatePointer();

            db.StoredProcedure("mile_card_select_by_id");

            db.AddParameter("@inIdMileCard", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

            db.AddOutParameter("@outIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32);
            db.AddOutParameter("@outNumber", MySql.Data.MySqlClient.MySqlDbType.String);
            db.AddOutParameter("@outPassword", MySql.Data.MySqlClient.MySqlDbType.String);
            db.AddOutParameter("@outMilesCount", MySql.Data.MySqlClient.MySqlDbType.Int32);
            db.AddOutParameter("@outIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32);
            db.AddOutParameter("@outIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32);
            db.AddOutParameter("@outIdRespondedPerson", MySql.Data.MySqlClient.MySqlDbType.Int32);
            db.AddOutParameter("@outIdOwnerPerson", MySql.Data.MySqlClient.MySqlDbType.Int32);
            db.AddOutParameter("@outNote", MySql.Data.MySqlClient.MySqlDbType.String);

            db.Execute();

            mileCardStatus.ID = db.GetOutParameterInt("@outIdMileCardStatus");
            mileCardStatus.Load();

            Silent = true;

            Number = db.GetOutParameterStr("@outNumber");
            Password = db.GetOutParameterStr("@outPassword");
            MilesCount = db.GetOutParameterInt("@outMilesCount");
            AviaCompanyUnionID = db.GetOutParameterInt("@outIdAviaCompanyUnion");
            AviaCompanyID = db.GetOutParameterInt("@outIdAviaCompany");
            PersonRespondedID = db.GetOutParameterInt("@outIdRespondedPerson");
            Note = db.GetOutParameterStr("@outNote");

            Silent = false;
            Changed = false;

            if (personOwner.ID != db.GetOutParameterInt("@outIdOwnerPerson"))
                throw new DataException("Mile card doesnt relates to current person");
        }

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.StoredProcedure("mile_card_update");

                    DBInterface.AddParameter("@inIdMileCard", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    
                    if (mileCardStatus.ID >= 0)
                    {
                        DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, mileCardStatus.ID);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }

                    DBInterface.AddParameter("@inNumber", MySql.Data.MySqlClient.MySqlDbType.String, Number);
                    DBInterface.AddParameter("@inPassword", MySql.Data.MySqlClient.MySqlDbType.String, Password);
                    DBInterface.AddParameter("@inMilesCount", MySql.Data.MySqlClient.MySqlDbType.Int32, MilesCount);

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

                    if (PersonRespondedID < 0)
                    {
                        DBInterface.AddParameter("@inIdRespondedPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdRespondedPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonRespondedID);
                    }

                    DBInterface.AddParameter("@inIdOwnerPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, personOwner.ID);
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Note);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {

                    DBInterface.StoredProcedure("mile_card_insert");

                    if (mileCardStatus.ID >= 0)
                    {
                        DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }

                    DBInterface.AddParameter("@inNumber", MySql.Data.MySqlClient.MySqlDbType.String, Number);
                    DBInterface.AddParameter("@inPassword", MySql.Data.MySqlClient.MySqlDbType.String, Password);
                    DBInterface.AddParameter("@inMilesCount", MySql.Data.MySqlClient.MySqlDbType.Int32, MilesCount);

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

                    if (PersonRespondedID < 0)
                    {
                        DBInterface.AddParameter("@inIdRespondedPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, null);
                    }
                    else
                    {
                        DBInterface.AddParameter("@inIdRespondedPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, PersonRespondedID);
                    }

                    DBInterface.AddParameter("@inIdOwnerPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, personOwner.ID);
                    DBInterface.AddParameter("@inNote", MySql.Data.MySqlClient.MySqlDbType.String, Note);

                    DBInterface.AddOutParameter("@outIdMileCard", MySql.Data.MySqlClient.MySqlDbType.Int32);

                    DBInterface.ExecuteTransaction();

                    ID = DBInterface.GetOutParameterInt("@outIdMileCard");

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
            DBInterface.StoredProcedure("mile_card_delete");
            DBInterface.AddParameter("@inIdMileCard", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.ExecuteTransaction();
        }

        private bool _Changed;

        public bool Changed 
        { 
            get
            {
                return _Changed;
            }
            private set
            {
                _Changed = value;
                UpdateStatus();
            }
        }

        private bool _Silent;

        private bool Silent
        {
            get
            {
                return _Silent;
            }
            set
            {
                if (value != Silent)
                {
                    _Silent = value;
                    UpdateStatus();
                }
            }
        }

        private void UpdateStatus()
        {
            if ((Changed) & (!Silent))
            {
                MileCardStatusList mcsList = new MileCardStatusList();
                mcsList.Load();

                Predicate<MileCardStatus> pred = mcs =>
                {
                    return (mcs.AviaCompanyID == AviaCompanyID)
                        & (mcs.AviaCompanyUnionID == AviaCompanyUnionID)
                        & (mcs.MinVal < MilesCount)
                        & ((mcs.MaxVal == 0) | (mcs.MaxVal > MilesCount));
                };

                MileCardStatus mcs0 = mcsList.Find(pred); 

                if (mcs0 != null)
                {
                    mileCardStatus.ID = mcs0.ID;
                }
                else
                {
                    mileCardStatus.ID = -1;
                }
                mileCardStatus.Load();
            }
        }
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
            Changed = false;

            aviacompany = new AviaCompany();
            aviacompanyunion = new AviaCompanyUnion();
        }

        public event EventHandler Updated;

        public void Load()
        {
            if (ID != -1)
            {
                IDBInterface db = DBInterface.CreatePointer();

                db.StoredProcedure("mile_card_status_select_by_id");
                db.AddParameter("@inIdMileCardStatus", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                db.AddOutParameter("@outStatus", MySql.Data.MySqlClient.MySqlDbType.String);
                db.AddOutParameter("@outMinMiles", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outMaxMiles", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outIdAviaCompanyUnion", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outIdAviaCompany", MySql.Data.MySqlClient.MySqlDbType.Int32);
                db.AddOutParameter("@outNote", MySql.Data.MySqlClient.MySqlDbType.String);

                db.Execute();

                Name = db.GetOutParameterStr("@outStatus");
                MinVal = db.GetOutParameterInt("@outMinMiles");
                MaxVal = db.GetOutParameterInt("@outMaxMiles");

                Note = db.GetOutParameterStr("@outNote");

                AviaCompanyID = db.GetOutParameterInt("@outIdAviaCompany");
                AviaCompanyUnionID = db.GetOutParameterInt("@outIdAviaCompanyUnion");

                Changed = false;
            }
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