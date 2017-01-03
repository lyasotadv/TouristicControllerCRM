using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models.Mapping;

namespace TravelController.Domain
{
    public abstract class PageData
    {
        public Catalog catalog { get; set; }
    }
}