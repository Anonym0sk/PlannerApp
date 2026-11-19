using System;
using System.Collections.Generic;
using PlannerApp;
using Xunit;

namespace PlannerApp.Tests
{
    public class EventManagerTests
    {
        private readonly DateTime _baseDate = new DateTime(2026, 11, 2, 10, 0, 0);

        // ==========================================
        // 1. ADD EVENT, VALIDATION & ID GENERATION
        // ==========================================

        [Fact]
        public void AddEvent_FirstEvent_AssignsIdAndAddsToList()
        {
            EventManager manager = new EventManager();
            Event newEvent = new Event
            {
                Name = "Meeting",
                Category = "Work",
                Description = "Sprint planning",
                Notes = "Bring laptop",
                Start = _baseDate,
                End = _baseDate.AddHours(1),
                IsAllDay = false
            };

            bool result = manager.AddEvent(newEvent);
            List<Event> all = manager.GetAllEvents();

            Assert.True(result);
            Assert.Single(all);
            Assert.True(all[0].Id > 0);
            Assert.Equal("Meeting", all[0].Name);
            Assert.Equal("Work", all[0].Category);
            Assert.Equal("Sprint planning", all[0].Description);
            Assert.Equal("Bring laptop", all[0].Notes);
        }

        [Fact]
        public void AddEvent_MultipleEvents_AssignsUniqueSequentialIds()
        {
            EventManager manager = new EventManager();
            Event e1 = new Event {Name = "E1", Start = _baseDate, End = _baseDate.AddHours(1)};
            Event e2 = new Event {Name = "E2", Start = _baseDate.AddHours(2), End = _baseDate.AddHours(3)};

            manager.AddEvent(e1);
            manager.AddEvent(e2);

            Assert.NotEqual(e1.Id, e2.Id);
            Assert.Equal(2, manager.GetAllEvents().Count);
        }

        [Fact]
        public void AddEvent_AfterDeletingEvent_StillAssignsUniqueId()
        {
            EventManager manager = new EventManager();
            Event e1 = new Event {Name = "E1", Start = _baseDate, End = _baseDate.AddHours(1)};
            Event e2 = new Event {Name = "E2", Start = _baseDate.AddHours(2), End = _baseDate.AddHours(3)};

            manager.AddEvent(e1);
            manager.AddEvent(e2);
            manager.DeleteEvent(e1.Id);

            Event e3 = new Event {Name = "E3", Start = _baseDate.AddHours(4), End = _baseDate.AddHours(5)};
            manager.AddEvent(e3);

            Assert.NotEqual(e2.Id, e3.Id);
        }

        [Fact]
        public void AddEvent_NullEvent_ReturnsFalse()
        {
            EventManager manager = new EventManager();
            Assert.False(manager.AddEvent(null!));
        }

        [Fact]
        public void AddEvent_EndBeforeStartOnTimedEvent_ReturnsFalse()
        {
            EventManager manager = new EventManager();
            Event invalidEvent = new Event
            {
                Name = "Time Paradox",
                Start = _baseDate.AddHours(2),
                End = _baseDate, // End is earlier than Start
                IsAllDay = false
            };

            Assert.False(manager.AddEvent(invalidEvent));
            Assert.Empty(manager.GetAllEvents());
        }

        [Fact]
        public void AddEvent_ZeroDurationEvent_ReturnsTrueAndDoesNotConflict()
        {
            EventManager manager = new EventManager();
            Event zeroDuration = new Event
            {
                Name = "Reminder",
                Start = _baseDate,
                End = _baseDate // Start == End
            };

            Assert.True(manager.AddEvent(zeroDuration));
            Assert.False(manager.HasConflict(zeroDuration));
        }

        // ==========================================
        // 2. CONFLICT DETECTION TESTS
        // ==========================================

        // Existing event is always scheduled from 10:00 to 12:00
        [Theory]
        [InlineData(10, 0, 12, 0)]  // Exact same time slot
        [InlineData(9, 30, 10, 30)] // Overlaps start (from the left)
        [InlineData(11, 30, 12, 30)]// Overlaps end (from the right)
        [InlineData(10, 30, 11, 30)]// New event is completely inside existing event
        [InlineData(9, 0, 13, 0)]   // New event completely surrounds existing event
        [InlineData(10, 0, 10, 15)] // Same start time, shorter duration
        [InlineData(11, 45, 12, 0)] // Same end time, shorter duration
        public void AddEvent_OverlappingTimeSlots_ReturnsFalse(int startHour, int startMin, int endHour, int endMin)
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Name = "Existing",
                Start = new DateTime(2026, 11, 2, 10, 0, 0),
                End = new DateTime(2026, 11, 2, 12, 0, 0)
            });

            Event candidate = new Event
            {
                Name = "Conflicting",
                Start = new DateTime(2026, 11, 2, startHour, startMin, 0),
                End = new DateTime(2026, 11, 2, endHour, endMin, 0)
            };

            bool result = manager.AddEvent(candidate);

            Assert.False(result);
            Assert.Single(manager.GetAllEvents());
        }

        [Theory]
        [InlineData(8, 0, 10, 0)]   // Ends exactly when existing starts (seamless before)
        [InlineData(12, 0, 14, 0)]  // Starts exactly when existing ends (seamless after)
        [InlineData(7, 0, 8, 0)]    // Completely before
        [InlineData(15, 0, 16, 0)]  // Completely after
        public void AddEvent_NonOverlappingAndBoundaryTimeSlots_ReturnsTrue(int startHour, int startMin, int endHour, int endMin)
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Name = "Existing",
                Start = new DateTime(2026, 11, 2, 10, 0, 0),
                End = new DateTime(2026, 11, 2, 12, 0, 0)
            });

            Event candidate = new Event
            {
                Name = "Valid",
                Start = new DateTime(2026, 11, 2, startHour, startMin, 0),
                End = new DateTime(2026, 11, 2, endHour, endMin, 0)
            };

            bool result = manager.AddEvent(candidate);

            Assert.True(result);
            Assert.Equal(2, manager.GetAllEvents().Count);
        }

        [Fact]
        public void AddEvent_NewEventIsAllDay_IgnoresConflictWithTimedEvent()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Start = _baseDate,
                End = _baseDate.AddHours(2),
                IsAllDay = false
            });

            Event allDay = new Event
            {
                Start = _baseDate.Date,
                End = _baseDate.Date.AddDays(1),
                IsAllDay = true
            };

            Assert.True(manager.AddEvent(allDay));
            Assert.Equal(2, manager.GetAllEvents().Count);
        }

        [Fact]
        public void AddEvent_ExistingEventIsAllDay_DoesNotBlockNewTimedEvent()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Start = _baseDate.Date,
                End = _baseDate.Date.AddDays(1),
                IsAllDay = true
            });

            Event timed = new Event
            {
                Start = _baseDate,
                End = _baseDate.AddHours(2),
                IsAllDay = false
            };

            Assert.True(manager.AddEvent(timed));
            Assert.Equal(2, manager.GetAllEvents().Count);
        }

        [Fact]
        public void AddEvent_NewEventHasNullEnd_IgnoresConflict()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Start = _baseDate,
                End = _baseDate.AddHours(2)
            });

            Event openEnded = new Event
            {
                Start = _baseDate.AddMinutes(30),
                End = null
            };

            Assert.True(manager.AddEvent(openEnded));
            Assert.Equal(2, manager.GetAllEvents().Count);
        }

        [Fact]
        public void AddEvent_ExistingEventHasNullEnd_DoesNotBlockNewEvent()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Start = _baseDate,
                End = null
            });

            Event timed = new Event
            {
                Start = _baseDate.AddMinutes(30),
                End = _baseDate.AddHours(1)
            };

            Assert.True(manager.AddEvent(timed));
            Assert.Equal(2, manager.GetAllEvents().Count);
        }

        [Fact]
        public void HasConflict_NullEvent_ReturnsFalse()
        {
            EventManager manager = new EventManager();
            Assert.False(manager.HasConflict(null!));
        }

        // ==========================================
        // 3. READ, SEARCH & STATE INTEGRITY TESTS
        // ==========================================

        [Fact]
        public void GetAllEvents_EmptyManager_ReturnsEmptyList()
        {
            EventManager manager = new EventManager();
            List<Event> result = manager.GetAllEvents();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllEvents_ModifyingReturnedList_DoesNotAffectInternalState()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Protected Event"});

            List<Event> externalList = manager.GetAllEvents();
            externalList.Clear();

            Assert.Single(manager.GetAllEvents());
        }

        [Fact]
        public void GetEventById_ExistingId_ReturnsCorrectEvent()
        {
            EventManager manager = new EventManager();
            Event ev = new Event {Name = "Target"};
            manager.AddEvent(ev);

            Event? found = manager.GetEventById(ev.Id);

            Assert.NotNull(found);
            Assert.Equal("Target", found.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void GetEventById_InvalidOrNonExistingId_ReturnsNull(int invalidId)
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Existing"});

            Assert.Null(manager.GetEventById(invalidId));
        }

        [Fact]
        public void GetEventsByName_PartialAndCaseInsensitive_ReturnsMatches()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Annual Dentist Checkup"});
            manager.AddEvent(new Event {Name = "Team Meeting"});
            manager.AddEvent(new Event {Name = null!}); // Edge case: null Name must not throw NullReferenceException

            List<Event> results = manager.GetEventsByName("dentist");

            Assert.Single(results);
            Assert.Equal("Annual Dentist Checkup", results[0].Name);
        }

        [Fact]
        public void GetEventsByName_NoMatch_ReturnsEmptyList()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Team Meeting"});

            List<Event> results = manager.GetEventsByName("Vacation");

            Assert.Empty(results);
        }

        [Fact]
        public void GetEventsByCategory_PartialAndCaseInsensitive_ReturnsMatches()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "E1", Category = "Health & Medical"});
            manager.AddEvent(new Event {Name = "E2", Category = "Work"});
            manager.AddEvent(new Event {Name = "E3", Category = null!}); // Edge case: null Category must not throw NullReferenceException

            List<Event> results = manager.GetEventsByCategory("MEDICAL");

            Assert.Single(results);
            Assert.Equal("E1", results[0].Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SearchMethods_NullOrWhitespaceQuery_ReturnsEmptyList(string? query)
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Meeting", Category = "Work"});

            Assert.Empty(manager.GetEventsByName(query!));
            Assert.Empty(manager.GetEventsByCategory(query!));
        }

        [Fact]
        public void GetEventsByDate_MatchesRegardlessOfTimeOfDay()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Early", Start = new DateTime(2026, 11, 2, 0, 1, 0)});
            manager.AddEvent(new Event {Name = "Late", Start = new DateTime(2026, 11, 2, 23, 59, 0)});
            manager.AddEvent(new Event {Name = "NextDay", Start = new DateTime(2026, 11, 3, 0, 0, 0)});

            List<Event> results = manager.GetEventsByDate(new DateTime(2026, 11, 2, 15, 30, 0));

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void GetEventsByDate_MultiDayEvent_MatchesMiddleDay()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event
            {
                Name = "3-Day Conference",
                Start = new DateTime(2026, 11, 1, 9, 0, 0),
                End = new DateTime(2026, 11, 3, 17, 0, 0)
            });

            List<Event> middleDayResults = manager.GetEventsByDate(new DateTime(2026, 11, 2));

            Assert.Single(middleDayResults);
            Assert.Equal("3-Day Conference", middleDayResults[0].Name);
        }

        [Fact]
        public void GetEventsbyRange_IncludesBoundariesAndExcludesOutside()
        {
            EventManager manager = new EventManager();
            DateTime rangeStart = new DateTime(2026, 11, 10, 0, 0, 0);
            DateTime rangeEnd = new DateTime(2026, 11, 20, 23, 59, 59);

            manager.AddEvent(new Event {Name = "Before", Start = rangeStart.AddSeconds(-1)});
            manager.AddEvent(new Event {Name = "OnStart", Start = rangeStart});
            manager.AddEvent(new Event {Name = "Inside", Start = new DateTime(2026, 11, 15)});
            manager.AddEvent(new Event {Name = "OnEnd", Start = rangeEnd});
            manager.AddEvent(new Event {Name = "After", Start = rangeEnd.AddSeconds(1)});

            List<Event> results = manager.GetEventsbyRange(rangeStart, rangeEnd);

            Assert.Equal(3, results.Count);
            Assert.Contains(results, e => e.Name == "OnStart");
            Assert.Contains(results, e => e.Name == "Inside");
            Assert.Contains(results, e => e.Name == "OnEnd");
        }

        [Fact]
        public void GetEventsbyRange_StartAfterEnd_ReturnsEmptyList()
        {
            EventManager manager = new EventManager();
            manager.AddEvent(new Event {Name = "Event", Start = _baseDate});

            List<Event> results = manager.GetEventsbyRange(_baseDate.AddDays(5), _baseDate.AddDays(-5));

            Assert.Empty(results);
        }

        // ==========================================
        // 4. UPDATE EVENT TESTS
        // ==========================================

        [Fact]
        public void UpdateEvent_ValidChangesAndSameTime_UpdatesAllPropertiesAndReturnsTrue()
        {
            EventManager manager = new EventManager();
            Event original = new Event
            {
                Name = "Old Name",
                Category = "Old Cat",
                Description = "Old Desc",
                Notes = "Old Notes",
                Start = _baseDate,
                End = _baseDate.AddHours(1),
                IsAllDay = false
            };
            manager.AddEvent(original);

            Event updated = new Event
            {
                Id = original.Id,
                Name = "New Name",
                Category = "New Cat",
                Description = "New Desc",
                Notes = "New Notes",
                Start = _baseDate,
                End = _baseDate.AddHours(1),
                IsAllDay = true
            };

            bool result = manager.UpdateEvent(updated);
            Event? saved = manager.GetEventById(original.Id);

            Assert.True(result);
            Assert.NotNull(saved);
            Assert.Equal("New Name", saved.Name);
            Assert.Equal("New Cat", saved.Category);
            Assert.Equal("New Desc", saved.Description);
            Assert.Equal("New Notes", saved.Notes);
            Assert.True(saved.IsAllDay);
        }

        [Fact]
        public void UpdateEvent_MovedToNewFreeSlot_FreesOldSlotForNewEvents()
        {
            EventManager manager = new EventManager();
            Event e1 = new Event
            {
                Name = "Movable",
                Start = new DateTime(2026, 11, 2, 10, 0, 0),
                End = new DateTime(2026, 11, 2, 11, 0, 0)
            };
            manager.AddEvent(e1);

            // Move e1 from 10:00-11:00 to 14:00-15:00
            Event moved = new Event
            {
                Id = e1.Id,
                Name = "Movable",
                Start = new DateTime(2026, 11, 2, 14, 0, 0),
                End = new DateTime(2026, 11, 2, 15, 0, 0)
            };
            Assert.True(manager.UpdateEvent(moved));

            // Original 10:00-11:00 slot must now be available
            Event replacement = new Event
            {
                Name = "Replacement",
                Start = new DateTime(2026, 11, 2, 10, 0, 0),
                End = new DateTime(2026, 11, 2, 11, 0, 0)
            };
            Assert.True(manager.AddEvent(replacement));
        }

        [Fact]
        public void UpdateEvent_MovedToConflictingSlot_ReturnsFalseAndPreservesOriginalData()
        {
            EventManager manager = new EventManager();
            Event event1 = new Event
            {
                Name = "Event 1",
                Start = new DateTime(2026, 11, 2, 10, 0, 0),
                End = new DateTime(2026, 11, 2, 11, 0, 0)
            };
            Event event2 = new Event
            {
                Name = "Event 2",
                Start = new DateTime(2026, 11, 2, 12, 0, 0),
                End = new DateTime(2026, 11, 2, 13, 0, 0)
            };
            manager.AddEvent(event1);
            manager.AddEvent(event2);

            Event conflictingUpdate = new Event
            {
                Id = event2.Id,
                Name = "Changed Name That Should Not Save",
                Start = new DateTime(2026, 11, 2, 10, 30, 0), // Conflicts with event1
                End = new DateTime(2026, 11, 2, 11, 30, 0)
            };

            bool result = manager.UpdateEvent(conflictingUpdate);
            Event? event2AfterFailedUpdate = manager.GetEventById(event2.Id);

            Assert.False(result);
            Assert.Equal("Event 2", event2AfterFailedUpdate?.Name);
            Assert.Equal(new DateTime(2026, 11, 2, 12, 0, 0), event2AfterFailedUpdate?.Start);
        }

        [Fact]
        public void UpdateEvent_EndBeforeStart_ReturnsFalseAndPreservesOriginal()
        {
            EventManager manager = new EventManager();
            Event original = new Event
            {
                Name = "Valid Event",
                Start = _baseDate,
                End = _baseDate.AddHours(1)
            };
            manager.AddEvent(original);

            Event invalidUpdate = new Event
            {
                Id = original.Id,
                Name = "Corrupted",
                Start = _baseDate.AddHours(3),
                End = _baseDate.AddHours(1) // Invalid range
            };

            Assert.False(manager.UpdateEvent(invalidUpdate));
            Assert.Equal("Valid Event", manager.GetEventById(original.Id)?.Name);
        }

        [Fact]
        public void UpdateEvent_NullEvent_ReturnsFalse()
        {
            EventManager manager = new EventManager();
            Assert.False(manager.UpdateEvent(null!));
        }

        [Fact]
        public void UpdateEvent_NonExistingId_ReturnsFalse()
        {
            EventManager manager = new EventManager();
            Event nonExisting = new Event {Id = 999, Name = "Ghost"};

            Assert.False(manager.UpdateEvent(nonExisting));
        }

        // ==========================================
        // 5. DELETE EVENT TESTS
        // ==========================================

        [Fact]
        public void DeleteEvent_ExistingId_RemovesOnlyTargetEventAndReturnsTrue()
        {
            EventManager manager = new EventManager();
            Event e1 = new Event {Name = "Keep", Start = _baseDate, End = _baseDate.AddHours(1)};
            Event e2 = new Event {Name = "Delete", Start = _baseDate.AddHours(2), End = _baseDate.AddHours(3)};
            manager.AddEvent(e1);
            manager.AddEvent(e2);

            bool result = manager.DeleteEvent(e2.Id);

            Assert.True(result);
            Assert.Single(manager.GetAllEvents());
            Assert.Null(manager.GetEventById(e2.Id));
            Assert.NotNull(manager.GetEventById(e1.Id));
        }

        [Fact]
        public void DeleteEvent_SameIdTwice_SecondCallReturnsFalse()
        {
            EventManager manager = new EventManager();
            Event e1 = new Event {Name = "Once"};
            manager.AddEvent(e1);

            Assert.True(manager.DeleteEvent(e1.Id));
            Assert.False(manager.DeleteEvent(e1.Id));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(999)]
        public void DeleteEvent_InvalidOrNonExistingId_ReturnsFalse(int invalidId)
        {
            EventManager manager = new EventManager();
            Assert.False(manager.DeleteEvent(invalidId));
        }
    }
}