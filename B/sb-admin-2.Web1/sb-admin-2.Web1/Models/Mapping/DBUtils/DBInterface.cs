using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace sb_admin_2.Web1.Models.Mapping.DBUtils
{
    public class DBInterface
    {
        private class DBInterfaceObject
        {
            private MySqlConnection conn { get; set; }

            public DBInterfaceObject()
            {
                //string connStr = "Server=MYSQL5015.SmarterASP.NET;Database=db_a17285_crmtest;Uid=a17285_crmtest;Pwd=000000zz";
                //string connStr = "server=mysql4.gear.host;user=sellcontroller;database=sellcontroller;password=Dc5SWX?j~3bM";
                System.Configuration.Configuration rootWebConfig =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/wwwwroot");

                string connStr = rootWebConfig.ConnectionStrings.ConnectionStrings["DBConnect"].ConnectionString;
                try
                {
                    conn = new MySqlConnection(connStr);
                    conn.Open();
                }
                catch
                {
                    throw new NotImplementedException("Handling of connection to DB exception was not implemented");
                }
            }

            ~DBInterfaceObject()
            {
                conn.Close();
            }

            public MySqlCommand command
            {
                get;
                private set;
            }

            public void Clear()
            {
                command = conn.CreateCommand();
                command.Connection = conn;
            }

            public DataTable ExecuteSelection()
            {
                DataTable tab = new DataTable();
                MySqlDataAdapter adapt = new MySqlDataAdapter(command);
                adapt.Fill(tab);
                return tab;
            }

            public void ExecuteTransaction()
            {
                MySqlTransaction trans = conn.BeginTransaction();
                command.Transaction = trans;
                command.ExecuteNonQuery();
                trans.Commit();
            }
        }

        static DBInterface()
        {
            _obj = null;
        }

        static private DBInterfaceObject _obj;

        static private DBInterfaceObject obj
        {
            get
            {
                if (_obj == null)
                    _obj = new DBInterfaceObject();
                return _obj;
            }
        }

        static public string CommandText
        {
            get
            {
                return obj.command.CommandText;
            }
            set
            {
                obj.Clear();
                obj.command.CommandText = value;
            }
        }

        static public void AddParameter(string parameterName, MySqlDbType DBType, object Value)
        {
            if (obj.command == null)
                throw new NullReferenceException("DB command have not be created. Set command text before add paramaters");
            obj.command.Parameters.Add(parameterName, DBType);
            int cnt = obj.command.Parameters.Count;
            obj.command.Parameters[cnt - 1].Value = Value;
        }

        static public DataTable ExecuteSelection()
        {
            return obj.ExecuteSelection();
        }

        static public void ExecuteTransaction()
        {
            obj.ExecuteTransaction();
        }
    }

    public interface IDBObject
    {
        event EventHandler Updated;

        void Load();

        void Save();

        void Delete();

        bool Changed { get; }
    }

    public abstract class DBObjectList<T> : List<T>
        where T : IDBObject
    {
        protected virtual void OnItemUpdated(object sender, EventArgs args)
        {
            if (args is DBEventArgs)
            {
                DBEventArgs a = args as DBEventArgs;
                if (a.ForceUpdate)
                {
                    Load();
                }
            }
        }

        public abstract void Load();

        public void Save()
        {
            foreach(var item in this)
            {
                if (item.Changed)
                {
                    item.Save();
                }
            }
        }

        public new void Add(T item)
        {
            base.Add(item);
            item.Updated += OnItemUpdated;
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            item.Updated -= OnItemUpdated;
        }

        ~DBObjectList()
        {
            Clear();
        }

        public new void Clear()
        {
            foreach (var item in this)
            {
                item.Updated -= OnItemUpdated;
            }
            base.Clear();
        }

        protected virtual void Init(T item)
        {
            item.Updated += OnItemUpdated;
        }
    }

    public class DBEventArgs : EventArgs
    {
        public bool ForceUpdate { get; set; }
    }

    public class InsertRow
    {
        private Dictionary<string, object> parameterValue;

        private Dictionary<string, MySqlDbType> parameterType;

        private string tableName;

        private string idName;

        public InsertRow(string tableName, string idName)
        {
            this.tableName = tableName;
            this.idName = idName;
            parameterValue = new Dictionary<string, object>();
            parameterType = new Dictionary<string, MySqlDbType>();

            Add(idName, MySqlDbType.Int32, PrimaryID());
        }

        private int PrimaryID()
        {
            DBInterface.CommandText = "SELECT MAX(" + idName + ") FROM " + tableName;
            DataTable tab = DBInterface.ExecuteSelection();
            if (tab.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(tab.Rows[0][0]) + 1;
        }

        public void Add(string Name, MySqlDbType DBType, object Value)
        {
            parameterValue.Add(Name, Value);
            parameterType.Add(Name, DBType);
        }

        public void Execute()
        {
            string com = "INSERT INTO " + tableName + " (";
            bool first = true;
            foreach(var item in parameterValue.Keys)
            {
                if (!first)
                {
                    com += ", ";
                }
                com += item;
                first = false;
            }
            com += ") VALUES (@";

            first = true;
            foreach (var item in parameterValue.Keys)
            {
                if (!first)
                {
                    com += ", @";
                }
                com += item;
                first = false;
            }
            com += ");";

            DBInterface.CommandText = com;

            DBInterface.AddParameter("@tab", MySqlDbType.String, tableName);

            foreach(var item in parameterValue)
            {
                MySqlDbType DBType = MySqlDbType.String;
                if (parameterType.TryGetValue(item.Key, out DBType))
                {
                    DBInterface.AddParameter("@" + item.Key, DBType, item.Value);
                }
            }

            DBInterface.ExecuteTransaction();
        }
    }
}
