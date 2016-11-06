using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BuisnessLogicModule;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BuisnessLogicModuleTest
{
    [TestClass]
    public class DBInterfaceTest
    {
        [TestMethod]
        public void TableRowsCounterTest1()
        {
            DBInterface.CommandText = "SELECT Name FROM person";
            DataTable tab = DBInterface.ExecuteSelection();
            int numberBefore = tab.Rows.Count;

            DBInterface.CommandText = "INSERT INTO person (Name) VALUES (@name)";
            DBInterface.AddParameter("@name", MySqlDbType.String, "Bob");
            DBInterface.ExecuteTransaction();

            DBInterface.CommandText = "SELECT Name FROM person";
            tab = DBInterface.ExecuteSelection();
            int numberAfter = tab.Rows.Count;

            DBInterface.CommandText = "DELETE FROM person where name = @name";
            DBInterface.AddParameter("@name", MySqlDbType.String, "Bob");
            DBInterface.ExecuteTransaction();

            Assert.AreEqual(numberAfter - numberBefore, 1, "Number of rows in People table has changed");
        }

        [TestMethod]
        public void TableRowsCounterTest2()
        {
            DBInterface.CommandText = "SELECT Name FROM person";
            DataTable tab = DBInterface.ExecuteSelection();
            int numberBefore = tab.Rows.Count;

            DBInterface.CommandText = "INSERT INTO person (Name) VALUES (@name)";
            DBInterface.AddParameter("@name", MySqlDbType.String, "Bob");
            DBInterface.ExecuteTransaction();

            DBInterface.CommandText = "INSERT INTO person (Name) VALUES (@name)";
            DBInterface.AddParameter("@name", MySqlDbType.String, "Alisa");
            DBInterface.ExecuteTransaction();

            DBInterface.CommandText = "SELECT Name FROM person";
            tab = DBInterface.ExecuteSelection();
            int numberAfter = tab.Rows.Count;

            DBInterface.CommandText = "DELETE FROM person where (name = @nameA) OR (name = @nameB)";
            DBInterface.AddParameter("@nameA", MySqlDbType.String, "Bob");
            DBInterface.AddParameter("@nameB", MySqlDbType.String, "Alisa");
            DBInterface.ExecuteTransaction();

            Assert.AreEqual(numberAfter - numberBefore, 2, "Number of rows in People table has changed");
        }
    }
}
