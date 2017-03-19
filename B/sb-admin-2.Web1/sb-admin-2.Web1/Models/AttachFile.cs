using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class AttachFilePersonList : DBObjectList<AttachFilePerson>
    {
        private PersonGeneral person { get; set; }

        public AttachFilePersonList(PersonGeneral person)
        {
            if (person == null)
                throw new ArgumentNullException("Attached file list must have not null person");
            this.person = person;
        }

        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "select " +
                                        "document.name, " +
                                        "document.GUID, " +
                                        "document.extension, " +
                                        "document.idDocument, " +
                                        "persondocument.description " +
                                        "from persondocument " +
                                        "inner join document " +
                                        "on document.idDocument = persondocument.idDocument " +
                                        "where persondocument.idPerson = @idPerson;";

            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);

            DataTable tab = DBInterface.ExecuteSelection();

            foreach (DataRow row in tab.Rows)
            {
                AttachFilePerson atp = Create();

                atp.ID = Convert.ToInt32(row["idDocument"]);
                atp.Name = Convert.ToString(row["name"]);
                atp.guid = Convert.ToString(row["GUID"]);
                atp.Extension = Convert.ToString(row["extension"]);
                atp.Description = Convert.ToString(row["description"]);

                Add(atp);
            }
        }

        public AttachFilePerson Create()
        {
            AttachFilePerson atp = new AttachFilePerson(person);
            Init(atp);
            return atp;
        }
    }

    public abstract class AttachFile : IDBObject
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
                if (_Name != value)
                {
                    _Name = value;
                    Changed = true;
                }
            }
        }

        private string _Extension;

        public string Extension
        {
            get
            {
                return _Extension;
            }
            set
            {
                if (_Extension != value)
                {
                    _Extension = value;
                    Changed = true;
                }
            }
        }

        private string _guid;

        public string guid
        {
            get
            {
                return _guid;
            }
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    Changed = true;
                }
            }
        }


        protected AttachFile()
        {
            ID = -1;
            Changed = false;
        }

        protected abstract void SaveNew();

        protected abstract void SaveExisted();


        public event EventHandler Updated;

        public abstract void Load();

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "UPDATE `sellcontroller`.`document` SET `name` = @name, " +
                        "`GUID` = @guid, `extension` = @extension WHERE `idDocument` = @id;";

                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@guid", MySql.Data.MySqlClient.MySqlDbType.String, guid);
                    DBInterface.AddParameter("@extension", MySql.Data.MySqlClient.MySqlDbType.String, Extension);

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

                    DBInterface.ExecuteTransaction();

                    SaveExisted();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    SaveNew();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = true });
                    }
                }

                Changed = false;
            }
        }

        public virtual void Delete()
        {
            if (ID >= 0)
            {
                DBInterface.CommandText = "DELETE FROM `sellcontroller`.`document` WHERE `idDocument` = @id;";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();

                if (Updated != null)
                {
                    Updated(this, new DBEventArgs() { ForceUpdate = true });
                }
            }
        }

        public bool Changed { get; protected set; }
    }

    public class AttachFilePerson : AttachFile
    {
        private PersonGeneral person { get; set; }

        private string _Description;

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    Changed = true;
                }
            }
        }


        public AttachFilePerson(PersonGeneral person)
        {
            if (person == null)
                throw new ArgumentNullException("Attached file must have not null person");
            this.person = person;
        }

        protected override void SaveNew()
        {
            DBInterface.CommandText = "insert into document (GUID, name, extension) values (@guid, @name, @extension); " +
                                        "insert into persondocument (idDocument, idPerson, description) " +
                                        "values (LAST_INSERT_ID(), @idPerson, @description);";

            DBInterface.AddParameter("@guid", MySql.Data.MySqlClient.MySqlDbType.String, guid);
            DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
            DBInterface.AddParameter("@extension", MySql.Data.MySqlClient.MySqlDbType.String, Extension);
            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.String, person.ID);
            DBInterface.AddParameter("@description", MySql.Data.MySqlClient.MySqlDbType.String, Description);

            ID = Convert.ToInt32(DBInterface.ExecuteTransaction());
        }

        protected override void SaveExisted()
        {
            DBInterface.CommandText = "UPDATE `sellcontroller`.`persondocument` SET `description` = @description " +
                        "WHERE `idDocument` = @id and `idPerson` = @idPerson;";

            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.String, person.ID);
            DBInterface.AddParameter("@description", MySql.Data.MySqlClient.MySqlDbType.String, Description);
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

            DBInterface.ExecuteTransaction();
        }

        public override void Load()
        {
            DBInterface.CommandText = "select " +
                                        "document.name, " +
                                        "document.GUID, " +
                                        "document.extension, " +
                                        "persondocument.description, " +
                                        "from persondocument " +
                                        "inner join document " +
                                        "on document.idDocument = persondocument.idDocument " +
                                        "where document.idDocument = @id and persondocument.idPerson = @idPerson;";

            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);

            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                Name = Convert.ToString(tab.Rows[0]["name"]);
                guid = Convert.ToString(tab.Rows[0]["GUID"]);
                Extension = Convert.ToString(tab.Rows[0]["extension"]);
                Description = Convert.ToString(tab.Rows[0]["description"]);
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Attached file to person table has rows with same id");
            }

            Changed = false;
        }

        public override void Delete()
        {
            if (ID >= 0)
            {
                DBInterface.CommandText = "DELETE FROM `sellcontroller`.`persondocument` WHERE `idDocument` = @id;";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();
            }
            base.Delete();
        }
    }

    public class AttachFileTransferData : AttachFile
    {
        public AttachFileTransferData()
        {

        }

        protected override void SaveNew()
        {
            throw new NotImplementedException("Transfer data file can not be used for storage");
        }

        public override void Load()
        {
            throw new NotImplementedException("Transfer data file can not be used for storage");
        }

        protected override void SaveExisted()
        {
            throw new NotImplementedException("Transfer data file can not be used for storage");
        }

        public void FillData(AttachFile data)
        {
            if (data != null)
            {
                data.ID = ID;
                data.Name = Name;
                data.Extension = Extension;
                data.guid = guid;
            }
        }
    }
}