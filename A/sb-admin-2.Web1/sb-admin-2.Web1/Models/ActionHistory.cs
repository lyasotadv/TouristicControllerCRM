using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin_2.Web1.Models
{
    public class ActionHistoryList : ModelList<ActionHistory>
    {

    }

    public class ActionHistory : IDebugModel
    {
        static private Random rand;

        static ActionHistory()
        {
            rand = new Random(100);
        }

        public enum ActionHistoryStatus { green, yellow, red };

        public ActionHistoryStatus Status { get; set; }

        public Person Person { get; set; }

        public Order Order { get; set; }

        public DateTime Created { get; set; }

        public string Content { get; set; }

        public void CreateTestData()
        {
            Status = (ActionHistoryStatus)rand.Next(3);

            Person = new Person();
            Person.CreateTestData();
            Order = new Order();
            Order.CreateTestData();
            Created = DateTime.Now;
            Content = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
        }
    }
}