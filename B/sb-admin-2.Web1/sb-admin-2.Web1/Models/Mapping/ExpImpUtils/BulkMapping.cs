using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models.Mapping.ExpImpUtils
{
    public class BulkMapping
    {
        public RawBulkDataList<RawBulkPassport> rawPassportList;

        public RawBulkDataList<RawBulkPerson> rawPersonList;

        public RawBulkDataList<RawBulkMileCard> rawMileCardList;

        public RawBulkDataList<RawBulkCompany> rawCompanyList;

        public BulkMapping()
        {
            rawPassportList = new RawBulkDataList<RawBulkPassport>();
            rawPassportList.Width = RawBulkPassport.Width;

            rawPersonList = new RawBulkDataList<RawBulkPerson>();
            rawPersonList.Width = RawBulkPerson.Width;

            rawMileCardList = new RawBulkDataList<RawBulkMileCard>();
            rawMileCardList.Width = RawBulkMileCard.Width;

            rawCompanyList = new RawBulkDataList<RawBulkCompany>();
            rawCompanyList.Width = RawBulkCompany.Width;

            rawPassportList.StatusUpdated += OnListUpdated;
            rawPersonList.StatusUpdated += OnListUpdated;
            rawMileCardList.StatusUpdated += OnListUpdated;
            rawCompanyList.StatusUpdated += OnListUpdated;
        }

        private void OnListUpdated(object sender, EventArgs e)
        {
            if (rawCompanyList.IsLoaded & rawPersonList.IsLoaded & rawPassportList.IsLoaded & rawMileCardList.IsLoaded)
            {
                int x = 1;
                x++;
            }
        }
    }

    public class RawBulkDataList<T> : List<T>
        where T : RawBulkData, new()
    {
        public event EventHandler StatusUpdated;

        private enum BulkListStatus { unloaded, loaded };

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
                return _status == BulkListStatus.loaded;
            }
        }

        public int Width { get; set; }

        public RawBulkDataList()
        {
            Width = 0;
            status = BulkListStatus.unloaded;
        }

        public bool Export(string fileName)
        {
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

            status = BulkListStatus.loaded;
            return true;
        }
    }

    public abstract class RawBulkData
    {
        public class RawDataField
        {
            public bool IsValid { get; private set; }

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

        public abstract void Parser(string[] data);

        public abstract bool IsValid { get; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        public RawDataField estimated { get; private set; }

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
            citizen = new RawDataField(DefaultTrueValidator);
            birth = new RawDataField(DefaultTrueValidator);
            gender = new RawDataField(DefaultTrueValidator);
            estimated = new RawDataField(DefaultTrueValidator);
            validtill = new RawDataField(DefaultTrueValidator);
            note = new RawDataField(DefaultTrueValidator);

            dayBirth = new RawDataField(DefaultTrueValidator);
            monthBirth = new RawDataField(DefaultTrueValidator);
            yearBirth = new RawDataField(DefaultTrueValidator);

            dayValid = new RawDataField(DefaultTrueValidator);
            monthValid = new RawDataField(DefaultTrueValidator);
            yearValid = new RawDataField(DefaultTrueValidator);
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
            estimated.Data = data[8];
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
                return owner.IsValid;
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
            company = new RawDataField(DefaultTrueValidator);
            NMS = new RawDataField(DefaultTrueValidator);
            number = new RawDataField(DefaultTrueValidator);
            phone1 = new RawDataField(DefaultTrueValidator);
            phone2 = new RawDataField(DefaultTrueValidator);
            email1 = new RawDataField(DefaultTrueValidator);
            email2 = new RawDataField(DefaultTrueValidator);
            name = new RawDataField(DefaultTrueValidator);
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
            number = new RawDataField(DefaultTrueValidator);
            company = new RawDataField(DefaultTrueValidator);
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
                return owner.IsValid;
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

        public RawDataField oficialname { get; private set; }

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
            oficialname = new RawDataField(DefaultTrueValidator);
            number = new RawDataField(DefaultTrueValidator);
            phone1 = new RawDataField(DefaultTrueValidator);
            phone2 = new RawDataField(DefaultTrueValidator);
            email1 = new RawDataField(DefaultTrueValidator);
            email2 = new RawDataField(DefaultTrueValidator);
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
            oficialname.Data = data[3];
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