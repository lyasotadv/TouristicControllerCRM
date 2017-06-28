using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Models.Mapping.ExpImpUtils
{
    public class BulkMapping
    {
        public event EventHandler DataExported;

        public class EventArgsRawData : EventArgs
        {
            public List<string> FileName { get; private set; }

            public EventArgsRawData()
            {
                FileName = new List<string>();
            }
        }

        public enum RawFileType { passport, person, milecard, company }

        private Dictionary<RawFileType, IRawBulkDataList> dict;

        private RawBulkDataList<RawBulkPassport> rawPassportList;

        private RawBulkDataList<RawBulkPerson> rawPersonList;

        private RawBulkDataList<RawBulkMileCard> rawMileCardList;

        private RawBulkDataList<RawBulkCompany> rawCompanyList;

        private List<RawBulkMileCard> failedMileCard { get; set; }
        private List<RawBulkPassport> failedPassport { get; set; }
        private List<RawBulkPerson> failedPerson { get; set; }
        private List<RawBulkCompany> failedCompany { get; set; }

        private CountryList countryList { get; set; }
        private AviaCompanyList aviaCompanyList { get; set; }

        private Label labelUnverified;

        public string GetStoredFileName(RawFileType filetype)
        {
            IRawBulkDataList list = null;
            if (dict.TryGetValue(filetype, out list))
            {
                return list.StoredFileName;
            }   
            return null;
        }

        public string[] GetStoredFileName()
        {
            string[] str = new string[dict.Count];
            int n = 0;
            foreach(var pair in dict)
            {
                str[n] = pair.Value.StoredFileName;
                n++;
            }
            return str;
        }

        public void CheckIfLoaded(string fileName, string fullFileName)
        {
            foreach(var pair in dict)
            {
                if (pair.Value.StoredFileName == fileName)
                {
                    pair.Value.StoredFileNameFull = fullFileName;
                    pair.Value.IsLoaded = true;
                    break;
                }
            }
        }

        public BulkMapping()
        {
            rawPassportList = new RawBulkDataList<RawBulkPassport>();
            rawPassportList.Width = RawBulkPassport.Width;
            rawPassportList.StoredFileName = "Passport";

            rawPersonList = new RawBulkDataList<RawBulkPerson>();
            rawPersonList.Width = RawBulkPerson.Width;
            rawPersonList.StoredFileName = "Person";
            rawPersonList.StartCell = "H2";

            rawMileCardList = new RawBulkDataList<RawBulkMileCard>();
            rawMileCardList.Width = RawBulkMileCard.Width;
            rawMileCardList.StoredFileName = "MileCard";

            rawCompanyList = new RawBulkDataList<RawBulkCompany>();
            rawCompanyList.Width = RawBulkCompany.Width;
            rawCompanyList.StoredFileName = "Company";

            rawPassportList.StatusUpdated += OnListUpdated;
            rawPersonList.StatusUpdated += OnListUpdated;
            rawMileCardList.StatusUpdated += OnListUpdated;
            rawCompanyList.StatusUpdated += OnListUpdated;

            dict = new Dictionary<RawFileType, IRawBulkDataList>();

            dict.Add(RawFileType.passport, rawPassportList);
            dict.Add(RawFileType.person, rawPersonList);
            dict.Add(RawFileType.milecard, rawMileCardList);
            dict.Add(RawFileType.company, rawCompanyList);

            string labelName = "Unverified";
            LabelList labelList = new LabelList();
            labelList.Load();
            labelUnverified = labelList.Find(item => item.Name == labelName);
            if (labelUnverified == null)
            {
                labelUnverified = labelList.Create();
                labelUnverified.Name = labelName;
                labelUnverified.Save();
            }
        }

        private void ContactHandler(PersonGeneral personGen, 
            RawBulkData.RawDataField rawPhone1, RawBulkData.RawDataField rawPhone2, 
            RawBulkData.RawDataField rawEmail1, RawBulkData.RawDataField rawEmail2)
        {
            if (rawPhone1.IsValid)
            {
                Contact phone1 = personGen.ContactList.Create("mobile");
                phone1.Description = rawPhone1.Data;
            }

            if (rawPhone2.IsValid)
            {
                Contact phone2 = personGen.ContactList.Create("mobile");
                phone2.Description = rawPhone2.Data;
            }

            if (!rawEmail1.IsEmpty)
            {
                Contact email1 = personGen.ContactList.Create("e-mail");
                if (rawEmail1.IsValid)
                {
                    email1.Content = rawEmail1.Data;
                }
                else
                {
                    email1.Description = rawEmail1.Data;
                }
            }

            if (!rawEmail2.IsEmpty)
            {
                Contact email2 = personGen.ContactList.Create("e-mail");
                if (rawEmail2.IsValid)
                {
                    email2.Content = rawEmail2.Data;
                }
                else
                {
                    email2.Description = rawEmail2.Data;
                }
            }
        }


        private void AssemblyCountryList()
        {
            countryList = new CountryList();
            countryList.Load();
            foreach (RawBulkPassport rawPassport in rawPassportList)
            {
                try
                {
                    if (rawPassport.citizen.IsValid)
                    {
                        string name = rawPassport.citizen.Data;
                        if (countryList.Find(item => item.Name == name) == null)
                        {
                            Country country = countryList.Create();
                            country.Name = name;
                            country.Save();
                        }
                    }

                    if (rawPassport.emmitated.IsValid)
                    {
                        string name = rawPassport.citizen.Data;
                        if (countryList.Find(item => item.Name == name) == null)
                        {
                            Country country = countryList.Create();
                            country.Name = name;
                            country.Save();
                        }
                    }
                }
                catch
                {
                    failedPassport.Add(rawPassport);
                }
            }
        }

        private void AssemblyAviaCompanyList()
        {
            aviaCompanyList = new AviaCompanyList();
            aviaCompanyList.Load();
            foreach (RawBulkCompany rawCompany in rawCompanyList)
            {
                try
                {
                    if (rawCompany.isaviacompany.Data == "1")
                    {
                        AviaCompany aviacompany = aviaCompanyList.Create();
                        aviacompany.FullName = rawCompany.name.Data;

                        if (!rawCompany.phone1.IsEmpty)
                        {
                            aviacompany.Description = " Phone1: " + rawCompany.phone1.Data;
                        }

                        if (!rawCompany.phone2.IsEmpty)
                        {
                            aviacompany.Description = " Phone2: " + rawCompany.phone2.Data;
                        }

                        if (!rawCompany.email1.IsEmpty)
                        {
                            aviacompany.Description = " Email1: " + rawCompany.email1.Data;
                        }

                        if (!rawCompany.email2.IsEmpty)
                        {
                            aviacompany.Description = " Email2: " + rawCompany.email2.Data;
                        }

                        if (!rawCompany.number.IsEmpty)
                        {
                            aviacompany.Description = " Number: " + rawCompany.number.Data;
                        }

                        if (!rawCompany.officialname.IsEmpty)
                        {
                            aviacompany.Description = " Official name: " + rawCompany.officialname.Data;
                        }

                        aviacompany.Save();
                    }

                    rawCompany.IsHandled = true;
                }
                catch
                {
                    failedCompany.Add(rawCompany);
                }
            }
        }
        
        private void ActionPersonByPassport(RawBulkPerson rawPerson, RawBulkPassport rawPassport, Person person)
        {
            if (rawPassport.IsValid && (rawPassport.owner.Data == rawPerson.name.Data))
            {
                person.FirstName = rawPassport.FirstName.Data;
                person.SecondName = rawPassport.SecondName.Data;
                person.MiddleName = rawPassport.Middle.Data;

                int year = Convert.ToInt32(rawPassport.yearBirth.Data);
                int month = Convert.ToInt32(rawPassport.monthBirth.Data);
                int day = Convert.ToInt32(rawPassport.dayBirth.Data);
                person.Birth = new DateTime(year, month, day);

                if (rawPassport.gender.IsValid)
                {
                    person.Gender = rawPassport.gender.Data.ToLower();
                }

                Passport passport = person.PassportList.Create();
                passport.SerialNumber = rawPassport.number.Data;

                year = Convert.ToInt32(rawPassport.yearValid.Data);
                month = Convert.ToInt32(rawPassport.monthValid.Data);
                day = Convert.ToInt32(rawPassport.dayValid.Data);
                passport.ValidTill = new DateTime(year, month, day);

                passport.Description = rawPassport.note.Data;

                passport.Citizen = countryList.Find(item => item.Name == rawPassport.citizen.Data);
                passport.CountryOfEmmitation = countryList.Find(item => item.Name == rawPassport.emmitated.Data);

                passport.Save();
            }

            rawPassport.IsHandled = true;
        }

        private void ActionPersonByMileCard(RawBulkPerson rawPerson, RawBulkMileCard rawMileCard, Person person)
        {
            if (rawMileCard.IsValid && (rawMileCard.owner.Data == rawPerson.name.Data))
            {
                AviaCompany aviaCompany = aviaCompanyList.Find((item) => item.FullName == rawMileCard.company.Data);
                if (aviaCompany != null)
                {
                    MileCard mileCard = person.mileCardList.Create();

                    mileCard.AviaCompanyID = aviaCompany.ID;
                    mileCard.Number = rawMileCard.number.Data;

                    mileCard.Save();
                }
                else
                {
                    person.Description += " Mile card: " + rawMileCard.company.Data + " " + rawMileCard.number.Data;
                }
            }
            rawMileCard.IsHandled = true;
        }

        private void ActionPerson(RawBulkPerson rawPerson)
        {
            try
            {
                if (rawPerson.IsValid)
                {
                    string personRawName = rawPerson.name.Data;

                    Person person = Person.CreatePerson();

                    ContactHandler(person, rawPerson.phone1, rawPerson.phone2, rawPerson.email1, rawPerson.email2);

                    person.Description = "Name: " + personRawName;
                    if (rawPerson.company.IsValid)
                    {
                        person.Description += " Company: " + rawPerson.company.Data;
                    }
                    if (rawPerson.NMS.IsValid)
                    {
                        person.Description += " FIO: " + rawPerson.NMS.Data;
                    }

                    rawPassportList.ForEach((rawPassport) => { ActionPersonByPassport(rawPerson, rawPassport, person); });
                    rawMileCardList.ForEach((rawMileCard) => { ActionPersonByMileCard(rawPerson, rawMileCard, person); });

                    person.Save();
                }
                rawPerson.IsHandled = true;
            }
            catch
            {
                failedPerson.Add(rawPerson);
            }
        }

        private void ActionCompany(RawBulkCompany rawCompany)
        {
            try
            {
                if (rawCompany.IsValid && (rawCompany.isaviacompany.Data == "0"))
                {
                    Company company = Company.Create();

                    company.FullName = rawCompany.name.Data;

                    ContactHandler(company, rawCompany.phone1, rawCompany.phone2, rawCompany.email1, rawCompany.email2);

                    company.Description = "";

                    if (rawCompany.officialname.IsValid)
                    {
                        company.Description += " Official name: " + rawCompany.officialname.Data;
                    }

                    if (rawCompany.number.IsValid)
                    {
                        company.Description += " Number: " + rawCompany.number.Data;
                    }

                    if (rawCompany.isinsurance.Data == "1")
                    {
                        company.Description += " Insurance";
                    }

                    if (rawCompany.isprovider.Data == "1")
                    {
                        company.Description += " Provider";
                    }

                    company.Save();
                }
                rawCompany.IsHandled = true;
            }
            catch
            {
                failedCompany.Add(rawCompany);
            }
        }

        private void DataHandling()
        {
            failedMileCard = new List<RawBulkMileCard>();
            failedPassport = new List<RawBulkPassport>();
            failedPerson = new List<RawBulkPerson>();
            failedCompany = new List<RawBulkCompany>();

            AssemblyCountryList();
            AssemblyAviaCompanyList();

            rawPersonList.ForEach(rawPerson => ActionPerson(rawPerson) );
            rawCompanyList.ForEach(rawCompany => ActionCompany(rawCompany) );
        }

        private void OnListUpdated(object sender, EventArgs e)
        {
            if (rawCompanyList.IsLoaded & rawPersonList.IsLoaded & rawPassportList.IsLoaded & rawMileCardList.IsLoaded)
            {
                foreach(var pair in dict)
                {
                    pair.Value.Export(pair.Value.StoredFileNameFull);
                    Thread.Sleep(1000);
                }

                DataHandling();

                if (DataExported != null)
                {
                    EventArgsRawData arg = new EventArgsRawData();
                    foreach(var pair in dict)
                    {
                        arg.FileName.Add(pair.Value.StoredFileName);
                    }
                    DataExported(this, arg);
                }
            }
        }
    }

    public interface IRawBulkDataList
    {
        string StoredFileName { get; }

        string StoredFileNameFull { get; set; }

        bool IsLoaded { get; set; }

        bool Export(string fileName);
    }

    public class RawBulkDataList<T> : List<T>, IRawBulkDataList
        where T : RawBulkData, new()
    {
        public string StoredFileName { get; set; }

        public string StoredFileNameFull { get; set; }

        public event EventHandler StatusUpdated;

        private enum BulkListStatus { unloaded, loaded, exported };

        private BulkListStatus _status;

        private BulkListStatus status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    if (StatusUpdated != null)
                        StatusUpdated(this, EventArgs.Empty);
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                return status == BulkListStatus.loaded;
            }
            set
            {
                if (value != IsLoaded)
                {
                    status = BulkListStatus.loaded;
                }
            }
        }

        public int Width { get; set; }

        public string StartCell { get; set; }

        public RawBulkDataList()
        {
            Width = 0;
            status = BulkListStatus.unloaded;
            StartCell = "A2";
        }

        public bool Export(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                return false;

            ExcelEI comp = null;
            try
            {
                comp = new ExcelEI(fileName);
                comp.startrange = "A2";
                comp.w = Width;
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
                string[] data = new string[Width];
                for(int m = 0; m< Width; m++)
                {
                    data[m] = comp.Data[n, m];
                }

                T item = new T();
                item.Parser(data);

                if (item.IsValid && (Find(p => item.Equals(p)) == null))
                {
                    Add(item);
                }
            }

            status = BulkListStatus.exported;
            return true;
        }
    }

    public abstract class RawBulkData
    {
        public class RawDataField
        {
            public bool IsValid { get; private set; }

            public bool IsEmpty 
            { 
                get
                {
                    return Data == string.Empty;
                }
            }

            private Func<string, bool> Validator { get; set; }

            private string _Data;

            public string Data
            {
                get
                {
                    return _Data;
                }
                set
                {
                    if (_Data != value)
                    {
                        _Data = value;
                        IsValid = Validator(_Data);
                    }
                }
            }

            public RawDataField(Func<string,bool> Validator)
            {
                IsValid = false;
                this.Validator = Validator;
            }

            public override bool Equals(object obj)
            {
                if ((obj != null) && (obj is RawDataField))
                {
                    return (Data != null) && (Data != string.Empty) && (Data == (obj as RawDataField).Data);
                }
                else
                {
                    return false;
                }
            }

            public override string ToString()
            {
                return Data;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        protected bool DefaultTrueValidator(string str)
        {
            return true;
        }

        protected bool NotEmptyValidator(string str)
        {
            return (str != null) && (str != string.Empty);
        }

        protected bool EmailValidator(string str)
        {
            if (str.Contains(' '))
                return false;

            return str.Split('@').Length == 2;
        }

        public abstract void Parser(string[] data);

        public abstract bool IsValid { get; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsHandled { get; set; }

        public RawBulkData()
        {
            IsHandled = false;
        }
    }

    public class RawBulkPassport : RawBulkData
    {
        public RawDataField owner { get; private set; }

        public RawDataField number { get; private set; }

        public RawDataField FirstName { get; private set; }

        public RawDataField Middle { get; private set; }

        public RawDataField SecondName { get; private set; }

        public RawDataField citizen { get; private set; }

        public RawDataField birth { get; private set; }

        public RawDataField gender { get; private set; }

        public RawDataField emmitated { get; private set; }

        public RawDataField validtill { get; private set; }

        public RawDataField note { get; private set; }

        public RawDataField dayBirth { get; private set; }

        public RawDataField monthBirth { get; private set; }

        public RawDataField yearBirth { get; private set; }

        public RawDataField dayValid { get; private set; }

        public RawDataField monthValid { get; private set; }

        public RawDataField yearValid { get; private set; }


        static public int Width { get; protected set; }

        static RawBulkPassport()
        {
            Width = 17;
        }

        public RawBulkPassport()
        {
            owner = new RawDataField(DefaultTrueValidator);
            number = new RawDataField(DefaultTrueValidator);
            FirstName = new RawDataField(DefaultTrueValidator);
            Middle = new RawDataField(DefaultTrueValidator);
            SecondName = new RawDataField(DefaultTrueValidator);
            citizen = new RawDataField(NotEmptyValidator);
            birth = new RawDataField(DefaultTrueValidator);
            gender = new RawDataField(DefaultTrueValidator);
            emmitated = new RawDataField(NotEmptyValidator);
            validtill = new RawDataField(DefaultTrueValidator);
            note = new RawDataField(DefaultTrueValidator);

            dayBirth = new RawDataField(DefaultTrueValidator);
            monthBirth = new RawDataField(DefaultTrueValidator);
            yearBirth = new RawDataField(DefaultTrueValidator);

            dayValid = new RawDataField(DefaultTrueValidator);
            monthValid = new RawDataField(DefaultTrueValidator);
            yearValid = new RawDataField(DefaultTrueValidator);
        }

        private bool GenderValidator(string str)
        {
            return (str == "Male") | (str == "Female");
        }

        public override void Parser(string[] data)
        {
            if ((data == null) || (data.Length != Width))
            {
                throw new ArgumentException("Input data is incorrect");
            }

            owner.Data = data[0];
            number.Data = data[1];
            FirstName.Data = data[2];
            Middle.Data = data[3];
            SecondName.Data = data[4];
            citizen.Data = data[5];
            birth.Data = data[6];
            gender.Data = data[7];
            emmitated.Data = data[8];
            validtill.Data = data[9];
            note.Data = data[10];

            dayBirth.Data = data[11];
            monthBirth.Data = data[12];
            yearBirth.Data = data[13];

            dayValid.Data = data[14];
            monthValid.Data = data[15];
            yearValid.Data = data[16];
        }

        public override bool IsValid
        {
            get 
            {
                return owner.IsValid && citizen.IsValid  && emmitated.IsValid;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is RawBulkPassport))
            {
                return number.Equals((obj as RawBulkPassport).number);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class RawBulkPerson : RawBulkData
    {
        public RawDataField company { get; private set; }

        public RawDataField NMS { get; private set; }

        public RawDataField number { get; private set; }

        public RawDataField phone1 { get; private set; }

        public RawDataField phone2 { get; private set; }

        public RawDataField email1 { get; private set; }

        public RawDataField email2 { get; private set; }

        public RawDataField name { get; private set; }


        static public int Width { get; protected set; }

        static RawBulkPerson()
        {
            Width = 8;
        }

        public RawBulkPerson()
        {
            company = new RawDataField(NotEmptyValidator);
            NMS = new RawDataField(NotEmptyValidator);
            number = new RawDataField(DefaultTrueValidator);
            phone1 = new RawDataField(NotEmptyValidator);
            phone2 = new RawDataField(NotEmptyValidator);
            email1 = new RawDataField(EmailValidator);
            email2 = new RawDataField(EmailValidator);
            name = new RawDataField(NotEmptyValidator);
        }

        public override void Parser(string[] data)
        {
            if ((data == null) || (data.Length != Width))
            {
                throw new ArgumentException("Input data is incorrect");
            }

            company.Data = data[0];
            NMS.Data = data[1];
            number.Data = data[2];
            phone1.Data = data[3];
            phone2.Data = data[4];
            email1.Data = data[5];
            email2.Data = data[6];
            name.Data = data[7];
        }

        public override bool IsValid
        {
            get
            {
                return name.IsValid;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is RawBulkPerson))
            {
                return name.Equals((obj as RawBulkPerson).name);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class RawBulkMileCard : RawBulkData
    {
        public RawDataField owner { get; private set; }

        public RawDataField number { get; private set; }

        public RawDataField company { get; private set; }


        static public int Width { get; protected set; }

        static RawBulkMileCard()
        {
            Width = 3;
        }

        public RawBulkMileCard()
        {
            owner = new RawDataField(DefaultTrueValidator);
            number = new RawDataField(NotEmptyValidator);
            company = new RawDataField(NotEmptyValidator);
        }

        public override void Parser(string[] data)
        {
            if ((data == null) || (data.Length != Width))
            {
                throw new ArgumentException("Input data is incorrect");
            }

            owner.Data = data[0];
            number.Data = data[1];
            company.Data = data[2];
        }

        public override bool IsValid
        {
            get
            {
                return owner.IsValid & company.IsValid;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is RawBulkMileCard))
            {
                return owner.Equals((obj as RawBulkMileCard).owner);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class RawBulkCompany : RawBulkData
    {
        public RawDataField isaviacompany { get; private set; }

        public RawDataField isprovider { get; private set; }

        public RawDataField isinsurance { get; private set; }

        public RawDataField officialname { get; private set; }

        public RawDataField number { get; private set; }

        public RawDataField phone1 { get; private set; }

        public RawDataField phone2 { get; private set; }

        public RawDataField email1 { get; private set; }

        public RawDataField email2 { get; private set; }

        public RawDataField name { get; private set; }


        static public int Width { get; protected set; }

        static RawBulkCompany()
        {
            Width = 10;
        }

        public RawBulkCompany()
        {
            isaviacompany = new RawDataField(DefaultTrueValidator);
            isprovider = new RawDataField(DefaultTrueValidator);
            isinsurance = new RawDataField(DefaultTrueValidator);
            officialname = new RawDataField(NotEmptyValidator);
            number = new RawDataField(NotEmptyValidator);
            phone1 = new RawDataField(NotEmptyValidator);
            phone2 = new RawDataField(NotEmptyValidator);
            email1 = new RawDataField(EmailValidator);
            email2 = new RawDataField(EmailValidator);
            name = new RawDataField(DefaultTrueValidator);
        }

        public override void Parser(string[] data)
        {
            if ((data == null) || (data.Length != Width))
            {
                throw new ArgumentException("Input data is incorrect");
            }

            isaviacompany.Data = data[0];
            isprovider.Data = data[1];
            isinsurance.Data = data[2];
            officialname.Data = data[3];
            number.Data = data[4];
            phone1.Data = data[5];
            phone2.Data = data[6];
            email1.Data = data[7];
            email2.Data = data[8];
            name.Data = data[9];
        }

        public override bool IsValid
        {
            get
            {
                return name.IsValid;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is RawBulkCompany))
            {
                return name.Equals((obj as RawBulkCompany).name);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}