using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models;

namespace sb_admin_2.Web1.Domain
{
    public class CompanyData : PageData
    {
        public CompanyData(Company company)
        {
            this.company = company;
        }

        public Company company { get; private set; }
    }
}