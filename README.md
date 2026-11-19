# \# PlannerApp

# 

# A C# / .NET console and core library application for managing events, schedules, and appointments.

# 

# \## Features

# 

# \- \*\*Event Management (CRUD):\*\* Create, read, update, and delete events with details such as name, category, description, notes, start/end timestamps, and all-day flags.

# \- \*\*Smart Search \& Filtering:\*\*

# &#x20; - Find events by unique ID.

# &#x20; - Case-insensitive partial search by event name or category.

# &#x20; - Filter events by a specific date or a custom date range.

# \- \*\*Time Conflict Detection \& Validation:\*\*

# &#x20; - Automatically prevents adding or updating events that overlap with existing scheduled events.

# &#x20; - Supports back-to-back (seamless) scheduling where one event ends exactly when another begins.

# &#x20; - Excludes all-day events and open-ended events (`End == null`) from blocking time slots.

# &#x20; - Built-in defensive validation against null inputs and invalid time ranges.

# \- \*\*Unit Tested Core:\*\* Core business logic in `EventManager` is covered by 45 automated unit tests using \*\*xUnit\*\*.

# 

# \## Project Structure

# 

# ```text

# PlannerApp/

# ├── PlannerApp/           # Core application logic, models (Event), and manager (EventManager)

# └── PlannerApp.Tests/     # xUnit test suite covering CRUD operations, search, and edge cases

# ```

# 

# \## Tech Stack

# 

# \- \*\*Language:\*\* C#

# \- \*\*Framework:\*\* .NET 10

# \- \*\*Testing:\*\* xUnit

# 

# \## Getting Started

# 

# \### Prerequisites

# 

# \- \[.NET 10 SDK](https://dotnet.microsoft.com/download) (or Visual Studio 2022 / Rider with .NET 10 support)

# 

# \### Running the Application

# 

# 1\. Clone the repository:

# &#x20;  ```bash

# &#x20;  git clone \[https://github.com/Anonym0sk/PlannerApp.git](https://github.com/Anonym0sk/PlannerApp.git)

# &#x20;  cd PlannerApp

# &#x20;  ```

# 

# 2\. Run the console demo:

# &#x20;  ```bash

# &#x20;  dotnet run --project PlannerApp

# &#x20;  ```

# 

# \### Running Unit Tests

# 

# Execute the automated test suite via CLI:

# ```bash

# dotnet test

# ```

# Or run them directly in Visual Studio using \*\*Test Explorer\*\* (`Ctrl + R, A`).

# 

# \## Roadmap

# 

# \- \[x] Core `Event` model \& `EventManager` CRUD operations

# \- \[x] Date/range filtering \& keyword search

# \- \[x] Time-overlap conflict detection \& defensive input validation

# \- \[x] Comprehensive unit test suite with xUnit

# \- \[ ] Interactive Console UI

# \- \[ ] Persistent storage (JSON file export/import)

# \- \[ ] Relational database integration (SQL)

# \- \[ ] Recurring events \& reminder notifications

# \- \[ ] Desktop / Web UI migration (WPF / Blazor)

# 

# \## Git Workflow

# 

# This project follows a feature-branch workflow:

# \- `main` – stable releases

# \- `development` – active integration branch

# \- `feature/\*` – isolated branches for new features and fixes merged via Pull Requests

