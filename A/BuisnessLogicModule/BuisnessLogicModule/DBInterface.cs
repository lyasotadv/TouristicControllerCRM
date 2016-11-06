using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace BuisnessLogicModule
{
    public class DBInterface
    {
        private class DBInterfaceObject
        {
            private MySqlConnection conn { get; set; }

            public DBInterfaceObject()
            {
                string connStr = "server=localhost;user=root;database=test;port=3306;password=000000";
                conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
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

    
}
