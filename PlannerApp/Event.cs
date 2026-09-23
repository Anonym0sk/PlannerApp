using System;
using System.Collections.Generic;
using System.Text;

namespace PlannerApp
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
    }
}
