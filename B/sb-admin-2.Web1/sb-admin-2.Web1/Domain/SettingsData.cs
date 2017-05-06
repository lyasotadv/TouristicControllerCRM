using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class SettingsData : PageData
    {
        public SettingsData()
        {
            aviaCompanyList = new AviaCompanyList();
            aviaCompanyUnionList = new AviaCompanyUnionList();
        }

        public AviaCompanyList aviaCompanyList { get; private set; }

        public AviaCompanyUnionList aviaCompanyUnionList { get; private set; }
    }
}