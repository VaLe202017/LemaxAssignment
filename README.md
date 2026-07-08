# Lemax Hotel Search API

A JSON REST web service built in C# (.NET 8) following Clean Architecture principles.

## Features Implemented

1. **CRUD Interface:** Fully functional endpoints for Create, Read, Update, and Delete operations on Hotel data.
2. **Search Interface:** A smart search endpoint that parses natural language prompts for budget and location.
3. **Multi-Objective Sorting:** Uses the Haversine formula to calculate the exact distance from the user, and normalizes distance and price to return a sorted list (cheaper and closer at the top).
4. **Pagination:** Search results are paginated via `Page` and `PageSize` parameters.
5. **In-Memory Persistence:** Simulates a real database using a thread-safe `ConcurrentDictionary` and seeds real Croatian hotel data on startup.

## How to Run

1. Ensure the .NET 8 SDK is installed.
2. Open a terminal in the root directory.
3. Run the following command:
   ```bash
   dotnet run --project HotelSearch.Api/HotelSearch.Api.csproj
   ```
