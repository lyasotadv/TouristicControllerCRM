using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TravelController.Models;

namespace TravelController.Domain
{
    public class ActionData
    {
        public ActionInform ActionInform { get; private set; }

        public ActionData()
        {
            ActionInform = new ActionInform();
        }
    }
}