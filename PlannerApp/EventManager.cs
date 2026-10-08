using System;
using System.Collections.Generic;

namespace PlannerApp
{
    public class EventManager
    {
        private List<Event> _events = new List<Event>();

        public void AddEvent(Event newEvent)
        {
            _events.Add(newEvent);
        }

        public List<Event> GetAllEvents()
        {
            return new List<Event>(_events);
        }

        public Event? GetEventById(int id)
        {
            foreach (Event e in _events)
            {
                if (e.Id == id)
                    return e;
            }
            return null;
        }

        public List<Event> GetEventsByName(string name)
        {
            List<Event> matchingEvents = new List<Event>();

            foreach (Event e in _events)
            {
                if (e.Name != null &&
                    e.Name.ToLower().Contains(name.ToLower())
                    )
                    matchingEvents.Add(e);
            }
            return matchingEvents;
        }

        public List<Event> GetEventsByCategory(string category)
        {
            List<Event> matchingEvents = new List<Event>();

            foreach (Event e in _events)
            {
                if (e.Category != null &&
                    e.Category.ToLower().Contains(category.ToLower())
                    )
                    matchingEvents.Add(e);
            }
            return matchingEvents;
        }

        public List<Event> GetEventsByDate(DateTime date)
        {
            return GetEventsbyRange(date, date);
        }

        public List<Event> GetEventsbyRange(DateTime from, DateTime to)
        {
            List<Event> matchingEvents = new List<Event>();

            foreach (Event e in _events)
            {
                DateTime eventEnd = e.End ?? e.Start;

                if (e.Start.Date <= to.Date && from.Date <= eventEnd.Date)
                    matchingEvents.Add(e);
            }
            return matchingEvents;
        }

        public bool UpdateEvent(Event updatedEvent)
        {
            Event? toUpdate = GetEventById(updatedEvent.Id);

            if (toUpdate == null)
                return false;

            toUpdate.Name = updatedEvent.Name;
            toUpdate.Category = updatedEvent.Category;
            toUpdate.Description = updatedEvent.Description;
            toUpdate.Notes = updatedEvent.Notes;
            toUpdate.Start = updatedEvent.Start;
            toUpdate.End = updatedEvent.End;
            toUpdate.IsAllDay = updatedEvent.IsAllDay;

            return true;
        }

        public bool DeleteEvent(int id)
        {
            Event? toDelete= GetEventById(id);

            if (toDelete == null)
                return false;

            _events.Remove(toDelete);

            return true;
        }
    }
}
