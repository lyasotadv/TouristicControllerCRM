using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace BuisnessLogicModule
{
    class MessageList : List<Message>, IDBDataList
    {
        public MessageList()
        {
            DBRead();
        }

        public void DBRead()
        {
            DBInterface.CommandText = "SELECT person.Name, message.Text FROM person, message WHERE person.idPerson = message.idPerson";
            DataTable tab = DBInterface.ExecuteSelection();

            Clear();
            foreach (DataRow row in tab.Rows)
            {
                string Name = Convert.ToString(row["Name"]);
                string Text = Convert.ToString(row["Text"]);

                Add(new Message { PersonName = Name, TextFreeFlow = Text });
            }
        }
    }
}
