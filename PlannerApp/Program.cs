using System;

namespace PlannerApp
{
    internal class Program
    {
        static void Main()
        {
            EventManager manager = new EventManager();

            // 1. Prepare sample data and test AddEvent (CREATE)
            Event event1 = new Event
            {
                Name = "First event",
                Category = "test event",
                Description = "this is a test",
                Notes = "first test event",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1).AddHours(12),
                IsAllDay = false
            };

            Event event2 = new Event();

            DateTime testDate = new DateTime(2026, 11, 02, 10, 30, 0);

            Event event3 = new Event
            {
                Name = "Dentist appointment",
                Category = "Doctor appointment",
                Description = "regular check up",
                Notes = "should take less than an hour",
                Start = testDate,
                End = testDate.AddHours(1),
                IsAllDay = false
            };

            // Test conflict detection on add
            Event event3Conflict = new Event
            {
                Name = "Conflicting meeting",
                Start = testDate.AddMinutes(30), // 11:00 (overlaps 10:30 - 11:30)
                End = testDate.AddHours(1).AddMinutes(30)
            };

            Event event3Seamless = new Event
            {
                Name = "Seamless next meeting",
                Start = testDate.AddHours(1), // 11:30 (starts right when event3 ends)
                End = testDate.AddHours(2)
            };

            Console.WriteLine($"Add event1: {manager.AddEvent(event1)}");
            Console.WriteLine($"Add event2 (End == null, should be True): {manager.AddEvent(event2)}");
            Console.WriteLine($"Add event3: {manager.AddEvent(event3)}");
            Console.WriteLine($"Add event3Conflict (should be False): {manager.AddEvent(event3Conflict)}");
            Console.WriteLine($"Add event3Seamless (should be True): {manager.AddEvent(event3Seamless)}");

            // 2. Test search and retrieval methods (READ)
            Console.WriteLine("\n--- All Events ---");
            foreach (var e in manager.GetAllEvents())
            {
                Console.WriteLine($"[{e.Id}] {e.Name} ({e.Category})");
            }

            Console.WriteLine($"\n--- Search by ID ({event1.Id}) ---");
            Event? foundById = manager.GetEventById(event1.Id);
            Console.WriteLine(foundById != null ? $"Found: {foundById.Name}" : "Event not found.");

            Console.WriteLine("\n--- Search by Category ('Doctor appointment') ---");
            foreach (var e in manager.GetEventsByCategory("Doctor appointment"))
            {
                Console.WriteLine($"Found: {e.Name}");
            }

            Console.WriteLine("\n--- Search by Date ---");
            foreach (var e in manager.GetEventsByDate(DateTime.Now))
            {
                Console.WriteLine($"Todays event: {e.Name} ({e.Start})");
            }

            Console.WriteLine("\n--- Search by Date Range ---");
            foreach (var e in manager.GetEventsbyRange(new DateTime(2025, 11, 02), testDate.AddDays(12)))
            {
                Console.WriteLine($"Range Event: {e.Name} ({e.Start})");
            }

            // 3. Test UpdateEvent (UPDATE)
            Event updatedEvent2 = new Event
            {
                Id = event2.Id,
                Name = "blank event",
                IsAllDay = true
            };

            bool isUpdated2 = manager.UpdateEvent(updatedEvent2);
            Console.WriteLine($"\nUpdate event2 result: {isUpdated2}");

            Event? checkUpdate = manager.GetEventById(event2.Id);
            Console.WriteLine($"--- Updated Event #{event2.Id} ---");
            Console.WriteLine($"Name: {checkUpdate?.Name}, IsAllDay: {checkUpdate?.IsAllDay}");

            // Test conflict detection on update
            Event updatedSeamlessToConflict = new Event
            {
                Id = event3Seamless.Id,
                Name = "Moved into conflict",
                Start = testDate.AddMinutes(15), // 10:45 (overlaps event3)
                End = testDate.AddHours(1)
            };
            Console.WriteLine($"Update event3Seamless to conflicting time (should be False): {manager.UpdateEvent(updatedSeamlessToConflict)}");

            // 4. Test DeleteEvent (DELETE)
            bool isDeleted = manager.DeleteEvent(event1.Id);
            Console.WriteLine($"\nDelete event1 result: {isDeleted}");

            Console.WriteLine($"--- All Events After Deleting ID {event1.Id} ---");
            foreach (var e in manager.GetAllEvents())
            {
                Console.WriteLine($"[{e.Id}] {e.Name}");
            }
        }
    }
}