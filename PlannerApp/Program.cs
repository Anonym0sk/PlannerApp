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

            manager.AddEvent(event1);
            manager.AddEvent(event2);
            manager.AddEvent(event3);

            // 2. Test search and retrieval methods (READ)
            Console.WriteLine("--- All Events ---");
            foreach (var e in manager.GetAllEvents())
            {
                Console.WriteLine($"[{e.Id}] {e.Name} ({e.Category})");
            }

            Console.WriteLine("\n--- Search by ID (1) ---");
            Event? foundById = manager.GetEventById(1);
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

            manager.UpdateEvent(updatedEvent2);

            Event? checkUpdate = manager.GetEventById(event2.Id);
            Console.WriteLine($"\n--- Updated Event #{event2.Id} ---");
            Console.WriteLine($"Name: {checkUpdate?.Name}, IsAllDay: {checkUpdate?.IsAllDay}");

            // 4. Test DeleteEvent (DELETE)
            manager.DeleteEvent(1);

            Console.WriteLine("\n--- All Events After Deleting ID 1 ---");
            foreach (var e in manager.GetAllEvents())
            {
                Console.WriteLine($"[{e.Id}] {e.Name}");
            }
        }
    }
}