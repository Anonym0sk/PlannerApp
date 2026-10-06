using PlannerApp;

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

Console.WriteLine("=== First event detail: ===");
Console.WriteLine($"Id: {event1.Id}");
Console.WriteLine($"Name: {event1.Name}");
Console.WriteLine($"Category: {event1.Category}");
Console.WriteLine($"Description: {event1.Description}");
Console.WriteLine($"Notes: {event1.Notes}");
Console.WriteLine($"Start: {event1.Start}");
Console.WriteLine($"End: {event1.End}");
Console.WriteLine($"IsAllDay: {event1.IsAllDay}");

Console.WriteLine();

Console.WriteLine("=== Second event detail: ===");
Console.WriteLine($"Id: {event2.Id}");
Console.WriteLine($"Name: {event2.Name}");
Console.WriteLine($"Category: {event2.Category}");
Console.WriteLine($"Description: {event2.Description}");
Console.WriteLine($"Notes: {event2.Notes}");
Console.WriteLine($"Start: {event2.Start}");
Console.WriteLine($"End: {event2.End}");
Console.WriteLine($"IsAllDay: {event2.IsAllDay}");