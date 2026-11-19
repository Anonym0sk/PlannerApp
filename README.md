# PlannerApp

A C# / .NET console and core library application for managing events, schedules, and appointments.

## Features

- **Event Management (CRUD):** Create, read, update, and delete events with details such as name, category, description, notes, start/end timestamps, and all-day flags.
- **Smart Search & Filtering:**
  - Find events by unique ID.
  - Case-insensitive partial search by event name or category.
  - Filter events by a specific date or a custom date range.
- **Time Conflict Detection & Validation:**
  - Automatically prevents adding or updating events that overlap with existing scheduled events.
  - Supports back-to-back (seamless) scheduling where one event ends exactly when another begins.
  - Excludes all-day events and open-ended events (`End == null`) from blocking time slots.
  - Built-in defensive validation against null inputs and invalid time ranges.
- **Unit Tested Core:** Core business logic in `EventManager` is covered by 45 automated unit tests using **xUnit**.

## Project Structure

```text
PlannerApp/
├── PlannerApp/           # Core application logic, models (Event), and manager (EventManager)
└—— PlannerApp.Tests/     # xUnit test suite covering CRUD operations, search, and edge cases
```

## Tech Stack

- **Language:** C#
- **Framework:** .NET 10
- **Testing:** xUnit

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or Visual Studio 2022 / Rider with .NET 10 support)

### Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/Anonym0sk/PlannerApp.git
   cd PlannerApp
   ```

2. Run the console demo:
   ```bash
   dotnet run --project PlannerApp
   ```

### Running Unit Tests

Execute the automated test suite via CLI:
```bash
dotnet test
```
Or run them directly in Visual Studio using **Test Explorer** (`Ctrl + R, A`).

## Roadmap

- [x] Core `Event` model & `EventManager` CRUD operations
- [x] Date/range filtering & keyword search
- [x] Time-overlap conflict detection & defensive input validation
- [x] Comprehensive unit test suite with xUnit
- [ ] Interactive Console UI
- [ ] Persistent storage (JSON file export/import)
- [ ] Relational database integration (SQL)
- [ ] Recurring events & reminder notifications
- [ ] Desktop / Web UI migration (WPF / Blazor)

## Git Workflow

This project follows a feature-branch workflow:
- `main` – stable releases
- `development` – active integration branch
- `feature/*` – isolated branches for new features and fixes merged via Pull Requests