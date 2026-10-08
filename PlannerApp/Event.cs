using System;

namespace PlannerApp
{
    public class Event
    {
        private static int _nextId = 1;

        public int Id { get; internal set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime? End { get; set; } = null;
        public bool IsAllDay { get; set; }

        public Event()
        {
            Id = _nextId;
            _nextId++;
        }
    }
}
