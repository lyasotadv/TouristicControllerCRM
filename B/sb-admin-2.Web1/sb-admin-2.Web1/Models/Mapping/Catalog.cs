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

        public Catalog()
        {
            countryList = new CountryList();
            labelList = new LabelList();
        }
    }
}