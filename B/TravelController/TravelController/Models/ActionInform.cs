using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelController.Models
{
    public class ActionInform : IDebugModel
    {
        public string Description { get; set; }

        public string DescriptionWide { get; set; }

        public void CreateTestData()
        {
            Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit";
            DescriptionWide = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        }
    }
}