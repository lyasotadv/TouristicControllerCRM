using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sb_admin_2.Web1.Models.Mapping;

namespace sb_admin_2.Web1.Domain
{
    public abstract class PageData
    {
        public Catalog catalog { get; set; }
    }
}