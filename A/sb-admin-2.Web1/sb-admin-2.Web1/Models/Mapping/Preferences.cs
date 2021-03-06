﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Globalization;

namespace sb_admin_2.Web1.Models.Mapping
{
    static public class Preferences
    {
        static private PreferencesData _pref;

        static private PreferencesData pref
        {
            get
            {
                if (_pref == null)
                {
                    _pref = new PreferencesData();
                }
                return _pref;
            }
        }

        static public CultureInfo cultureInfo
        {
            get { return pref.cultureInfo; }
        }
    }

    public class PreferencesData
    {
        public CultureInfo cultureInfo { get; private set; }

        public PreferencesData()
        {
            cultureInfo = CultureInfo.CreateSpecificCulture("en-UK");
        }
    }
}