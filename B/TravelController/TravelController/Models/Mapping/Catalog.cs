﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelController.Models.Mapping
{
    public class Catalog
    {
        private CountryList _countryList;

        public CountryList countryList 
        { 
            get
            {
                if (_countryList.Count == 0)
                    _countryList.Update();
                return _countryList;
            }
            private set
            {
                _countryList = value;
            }
        }

        public Catalog()
        {
            countryList = new CountryList();
        }
    }
}