using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Drawing;

using sb_admin_2.Web1.Models.Mapping.DBUtils;

namespace sb_admin_2.Web1.Models
{
    public class LabelList : DBObjectList<Label>
    {
        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "SELECT * FROM sellcontroller.label";
            DataTable tab = DBInterface.ExecuteSelection();
            foreach (DataRow row in tab.Rows)
            {
                Label item = Create();
                item.ID = Convert.ToInt32(row["idLabel"]);
                item.Name = Convert.ToString(row["name"]);
                item.Comment = Convert.ToString(row["comment"]);
                item.SetColor(row["color"]);
                if (!(row["idParent"] is DBNull))
                    item.ParentID = Convert.ToInt32(row["idParent"]);
                Add(item);
            }
        }

        public Label Create()
        {
            Label label = new Label();
            Init(label);
            return label;
        }
    }

    public class LabelListPerson : DBObjectList<LabelPerson>
    {
        private PersonGeneral person { get; set; }

        public LabelListPerson(PersonGeneral person)
        {
            if (person == null)
                throw new ArgumentNullException("Person cannot be null");
            this.person = person;
        }

        public override void Load()
        {
            Clear();
            DBInterface.CommandText = "select " +
                                        "label.idLabel, " + 
                                        "label.name, " + 
                                        "label.color, " + 
                                        "label.comment, " + 
                                        "personlabel.note, " + 
                                        "label.idParent " + 
                                        "from personlabel " + 
                                        "inner join label " + 
                                        "on label.idLabel = personlabel.idLabel " + 
                                        "where personlabel.idPerson = @idPerson;";

            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);

            DataTable tab = DBInterface.ExecuteSelection();
            foreach (DataRow row in tab.Rows)
            {
                LabelPerson item = Create();
                item.ID = Convert.ToInt32(row["idLabel"]);
                item.Name = Convert.ToString(row["name"]);
                item.Comment = Convert.ToString(row["comment"]);
                item.SetColor(row["color"]);
                if (!(row["idParent"] is DBNull))
                    item.ParentID = Convert.ToInt32(row["idParent"]);
                if (item is LabelPerson)
                {
                    (item as LabelPerson).Note = Convert.ToString(row["note"]);
                }
                Add(item);
            }
        }

        public LabelPerson Create()
        {
            LabelPerson label = new LabelPerson(person);
            Init(label);
            return label;
        }

        public LabelPerson AttachLabelToPerson(Label label, string Note)
        {
            LabelPerson labelPerson = null; 
            if ((label != null) && (Find(item => item.ID == label.ID) == null))
            {
                labelPerson = Create() as LabelPerson;
                labelPerson.ID = label.ID;
                labelPerson.Load();
                labelPerson.Note = Note;
                labelPerson.AttachToPerson();
            }
            return labelPerson;
        }

        public void ReAttachLabelFromPerson(Label label)
        {
            if (label != null)
            {
                Label labelTemp = Find(item => item.ID == label.ID);
                if (labelTemp != null)
                {
                    LabelPerson labelPerson = labelTemp as LabelPerson;
                    labelPerson.ReAttachFromPerson();
                }
            }
        }
    }

    public class Label : IDBObject
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
                if (value != _Name)
                {
                    _Name = value;
                    Changed = true;
                }
            }
        }

        private string _Comment;

        public string Comment 
        { 
            get
            {
                return _Comment;
            }
            set
            {
                if (value != _Comment)
                {
                    _Comment = value;
                    Changed = true;
                }
            }
        }

        public Color color { get; set; }

        public Label()
        {
            ID = -1;
            Changed = false;
            _ParentID = -1;
        }

        public void SetColor(object obj)
        {
            try
            {
                byte[] arr = (byte[])obj;
                color = Color.FromArgb(0, arr[0], arr[1], arr[2]);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("Incorrect color data");
            }   
        }

        public byte[] GetColor()
        {
            if (color == null)
                return null;
            byte[] arr = new byte[3];
            arr[0] = color.R;
            arr[1] = color.G;
            arr[2] = color.B;
            return arr;
        }

        public string ColorHtml
        {
            get
            {
                if (color == null)
                    return "";
                return ColorTranslator.ToHtml(color);
            }
            set
            {
                if (value != ColorHtml)
                {
                    try 
                    {
                        color = ColorTranslator.FromHtml(value);
                    }
                    catch (InvalidCastException e)
                    {
                        throw new InvalidCastException("Incorrect color html data");
                    }   
                    
                }
            }
        }

        public string BlackOrWhite
        {
            get
            {
                if (color == null)
                    return "#000000";
                int p = color.R + color.G + color.B;
                p /= 3;
                if (p < 127)
                    return "#ffffff";
                else
                    return "#000000";
            }
        }

        public void AppendRandomColor()
        {
            Random rand = new Random();
            byte R = (byte)(rand.Next(255));
            byte G = (byte)(rand.Next(255));
            byte B = (byte)(rand.Next(255));
            color = Color.FromArgb(0, R, G, B);
        }


        private int _ParentID;

        public int ParentID
        {
            get
            {
                return _ParentID;
            }
            set
            {
                if (value != _ParentID)
                {
                    _ParentID = value;
                    Changed = true;

                    Label label = new Label();
                    label.ID = ParentID;
                    label.Load();

                    _ParentName = label.Name;
                    ParentColorHTML = label.ColorHtml;
                    ParentBlackOrWhite = label.BlackOrWhite;
                }
            }
        }

        private string _ParentName;

        public string ParentName
        {
            get
            {
                return _ParentName;
            }
            set
            {
                if (value != _ParentName)
                {
                    _ParentName = value;
                    Changed = true;

                    Label label = new Label();
                    label.Load(ParentName);

                    _ParentID = label.ID;
                    ParentColorHTML = label.ColorHtml;
                    ParentBlackOrWhite = label.BlackOrWhite;
                }
            }
        }

        public string ParentColorHTML { get; private set; }

        public string ParentBlackOrWhite { get; private set; }


        public event EventHandler Updated;

        public virtual void Load()
        {
            DBInterface.CommandText = "SELECT * FROM `sellcontroller`.`label` WHERE `idLabel` = @id;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                Name = Convert.ToString(tab.Rows[0]["name"]);
                Comment = Convert.ToString(tab.Rows[0]["comment"]);
                if (!(tab.Rows[0]["idParent"] is DBNull))
                    ParentID = Convert.ToInt32(tab.Rows[0]["idParent"]);
                SetColor(tab.Rows[0]["color"]);
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Label table has rows with same id");
            }

            Changed = false;
        }

        public void Save()
        {
            if (Changed)
            {
                if (ID >= 0)
                {
                    DBInterface.CommandText = "UPDATE `sellcontroller`.`label` SET `name` = @name, " +
                        "`comment` = @comment, `color` = @color, `idParent` = @idParent WHERE `idLabel` = @id;";

                    DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    DBInterface.AddParameter("@comment", MySql.Data.MySqlClient.MySqlDbType.String, Comment);
                    DBInterface.AddParameter("@color", MySql.Data.MySqlClient.MySqlDbType.VarBinary, GetColor());
                    
                    if (ParentID > -1)
                        DBInterface.AddParameter("@idParent", MySql.Data.MySqlClient.MySqlDbType.Int32, ParentID);
                    else
                        DBInterface.AddParameter("@idParent", MySql.Data.MySqlClient.MySqlDbType.Int32, null);

                    DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);

                    DBInterface.ExecuteTransaction();

                    if (Updated != null)
                    {
                        Updated(this, new DBEventArgs() { ForceUpdate = false });
                    }
                }
                else
                {
                    InsertRow insertRow = new InsertRow("label");
                    insertRow.Add("name", MySql.Data.MySqlClient.MySqlDbType.String, Name);
                    insertRow.Add("comment", MySql.Data.MySqlClient.MySqlDbType.String, Comment);
                    insertRow.Add("color", MySql.Data.MySqlClient.MySqlDbType.VarBinary, GetColor());
                    
                    if (ParentID > -1)
                        insertRow.Add("idParent", MySql.Data.MySqlClient.MySqlDbType.Int32, ParentID);
                    
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
                DBInterface.CommandText = "DELETE FROM `sellcontroller`.`label` WHERE `idLabel` = @id;";
                DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                DBInterface.ExecuteTransaction();

                if (Updated != null)
                {
                    Updated(this, new DBEventArgs() { ForceUpdate = true });
                }
            }
        }

        public bool Changed { get; protected set; }


        private void Load(string name)
        {
            DBInterface.CommandText = "SELECT * FROM `sellcontroller`.`label` WHERE `name` = @name;";
            DBInterface.AddParameter("@name", MySql.Data.MySqlClient.MySqlDbType.String, name);
            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                ID = Convert.ToInt32(tab.Rows[0]["idLabel"]);
                Name = Convert.ToString(tab.Rows[0]["name"]);
                Comment = Convert.ToString(tab.Rows[0]["comment"]);
                SetColor(tab.Rows[0]["color"]);
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Label table has rows with same name");
            }

            Changed = false;
        }
    }

    public class LabelPerson : Label
    {
        private PersonGeneral person { get; set; }

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

        public LabelPerson(PersonGeneral person)
        {
            if (person == null)
                throw new ArgumentNullException("Person must be not null");
            this.person = person;
        }

        private bool LoadAttachedData(bool UpdateData)
        {
            DBInterface.CommandText = "SELECT * FROM `sellcontroller`.`personlabel` WHERE `idLabel` = @id and `idPerson` = @idPerson;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);

            DataTable tab = DBInterface.ExecuteSelection();

            if (tab.Rows.Count == 1)
            {
                if (UpdateData)
                {
                    Note = Convert.ToString(tab.Rows[0]["note"]);
                }
            }
            else if (tab.Rows.Count > 1)
            {
                throw new DuplicateNameException("Label to person table has rows with same id");
            }

            return tab.Rows.Count == 1;
        }

        public override void Load()
        {
            base.Load();
            LoadAttachedData(true);
        }

        public void AttachToPerson()
        {
            if (!LoadAttachedData(false))
            {
                InsertRow insertRow = new InsertRow("personlabel");
                insertRow.Add("note", MySql.Data.MySqlClient.MySqlDbType.String, Note);
                insertRow.Add("idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);
                insertRow.Add("idLabel", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
                insertRow.Execute();
            }
            else
            {
                throw new DuplicateNameException("Its impossible to add the same label to person twice");
            }
        }

        public void ReAttachFromPerson()
        {
            DBInterface.CommandText = "DELETE FROM `sellcontroller`.`personlabel` WHERE `idLabel` = @id and `idPerson` = @idPerson;";
            DBInterface.AddParameter("@id", MySql.Data.MySqlClient.MySqlDbType.Int32, ID);
            DBInterface.AddParameter("@idPerson", MySql.Data.MySqlClient.MySqlDbType.Int32, person.ID);
            DBInterface.ExecuteTransaction();
        }
    }
}