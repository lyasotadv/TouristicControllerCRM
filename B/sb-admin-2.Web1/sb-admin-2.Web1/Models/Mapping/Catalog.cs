using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models.Mapping
{
    public class Catalog
    {
        private CountryList _countryList;

        public CountryList countryList 
        { 
            get
            {
                if (_countryList.Count == 0)
                    _countryList.Load();
                return _countryList;
            }
            private set
            {
                _countryList = value;
            }
        }

        private LabelList _labelList;

        public LabelList labelList
        {
            get
            {
                if (_labelList.Count == 0)
                    _labelList.Load();
                return _labelList;
            }
            private set
            {
                _labelList = value;
            }
        }

        private AviaCompanyList _aviaCompanyList;

        public AviaCompanyList aviaCompanyList
        {
            get
            {
                if (_aviaCompanyList.Count == 0)
                    _aviaCompanyList.Load();
                return _aviaCompanyList;
            }
            set
            {
                _aviaCompanyList = value;
            }
        }

        private AviaCompanyUnionList _aviaCompanyUnionList;

        public AviaCompanyUnionList aviaCompanyUnionList
        {
            get
            {
                if (_aviaCompanyUnionList.Count == 0)
                    _aviaCompanyUnionList.Load();
                return _aviaCompanyUnionList;
            }
            set
            {
                _aviaCompanyUnionList = value;
            }
        }

        private PersonList _personList;

        public PersonList personList
        {
            get
            {
                if (_personList.Count == 0)
                    _personList.Load();
                return _personList;
            }
            set
            {
                _personList = value;
            }
        }

        private CountryUnionList _countryUnionList;

        public CountryUnionList countryUnionList
        {
            get
            {
                if (_countryUnionList.Count == 0)
                    _countryUnionList.Load();
                return _countryUnionList;
            }
            set
            {
                _countryUnionList = value;
            }
        }

        public Catalog()
        {
            countryList = new CountryList();
            labelList = new LabelList();
            aviaCompanyList = new AviaCompanyList();
            aviaCompanyUnionList = new AviaCompanyUnionList();
            personList = new PersonList();
            countryUnionList = new CountryUnionList();
        }
    }
}