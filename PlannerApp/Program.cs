using PlannerApp;

Event testEvent = new Event
{
    Id = 1,
    Name = "First event",
    Category = "test event",
    Description = "this is a test",
    Notes = "first test event",
    Start = DateTime.Now,
    End = DateTime.Now.AddDays(1).AddHours(12),
    IsAllDay = false
};

Console.WriteLine("=== First event detail: ===");
Console.WriteLine($"Id: {testEvent.Id}");
Console.WriteLine($"Name: {testEvent.Name}");
Console.WriteLine($"Category: {testEvent.Category}");
Console.WriteLine($"Description: {testEvent.Description}");
Console.WriteLine($"Notes: {testEvent.Notes}");
Console.WriteLine($"Start: {testEvent.Start}");
Console.WriteLine($"End: {testEvent.End}");
Console.WriteLine($"IsAllDay: {testEvent.IsAllDay}");
