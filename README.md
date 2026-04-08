# RSS Feed Reader - MVP

A modern web application for managing RSS feed subscriptions. Built with ASP.NET Core 7 backend (Web API) and Blazor WebAssembly frontend.

## Overview

The MVP (Minimum Viable Product) implements two core user stories:
1. **Add Feed Subscription**: Users can enter an RSS feed URL and add it to their subscription list
2. **View Subscriptions**: Users can view all subscribed feeds in an organized table format

## Project Structure

```
RSSFeedReader/
├── backend/
│   ├── RSSFeedReader.Api/              # ASP.NET Core Web API backend
│   │   ├── Models/
│   │   │   └── Subscription.cs        # Feed subscription entity
│   │   ├── Services/
│   │   │   └── SubscriptionService.cs # Business logic for subscriptions
│   │   ├── Controllers/
│   │   │   └── SubscriptionsController.cs # REST API endpoints
│   │   └── Program.cs                  # Startup configuration, DI, CORS
│   └── RSSFeedReader.Api.Tests/        # Backend unit and integration tests
├── frontend/
│   ├── RSSFeedReader.UI/               # Blazor WebAssembly frontend
│   │   ├── Pages/
│   │   │   └── Subscriptions.razor    # Main UI page - add & list subscriptions
│   │   ├── Models/
│   │   │   └── SubscriptionDto.cs     # Data transfer object
│   │   ├── Services/
│   │   │   └── SubscriptionApiService.cs # HTTP client for backend communication
│   │   ├── wwwroot/
│   │   │   └── appsettings.json       # Configuration (API base URL)
│   │   └── Shared/
│   │       └── NavMenu.razor          # Navigation component
│   └── RSSFeedReader.UI.Tests/        # Frontend unit tests
└── specs/
    └── 001-add-subscriptions/         # Project specifications and documentation
        ├── spec.md                    # Feature requirements
        ├── plan.md                    # Technical architecture and decisions
        ├── data-model.md              # Entity definitions
        ├── contracts/                 # API contracts
        └── tasks.md                   # Detailed task list (T001-T074)
```

## Quick Start

### Prerequisites
- .NET 7.0 SDK or later ([download](https://dotnet.microsoft.com/download))
- Windows, macOS, or Linux

### Build & Run

```bash
# Terminal 1: Backend
cd backend/RSSFeedReader.Api
dotnet run

# Terminal 2: Frontend
cd frontend/RSSFeedReader.UI
dotnet run

# Open browser to http://localhost:5213
```

### Run Tests

```bash
dotnet test
```

**Expected Result**: ✓ All 20 tests pass (15 backend + 5 frontend)

## Usage

1. Open `http://localhost:5213` in your browser
2. Enter an RSS feed URL in the input field
3. Click "Add Subscription"
4. View all subscriptions in the table below

## API Reference

### GET /api/subscriptions
Returns all subscriptions.

### POST /api/subscriptions
Adds a new subscription. Body: `{ "url": "..." }`

## Implementation Status

✅ **MVP COMPLETE** - All 74 tasks implemented

- **Phase 1**: Project setup (7 tasks)
- **Phase 2**: Foundation (14 tasks)
- **Phase 3**: User Story 1 - Add Subscription (10 tasks)
- **Phase 4**: User Story 2 - View Subscriptions (9 tasks)
- **Phase 5**: Integration & Verification (22 tasks)
- **Phase 6-7**: Polish & Documentation (12 tasks)

### Test Results
- Backend Tests: **15/15 ✓**
- Frontend Tests: **5/5 ✓**
- Build Status: **✓ 0 errors, 0 warnings**

## Technology Stack

- **Backend**: ASP.NET Core 7 Web API
- **Frontend**: Blazor WebAssembly
- **Testing**: xUnit
- **Storage**: In-memory (MVP scope)

## Known Limitations

- In-memory storage only (data lost on backend restart)
- No URL validation
- No RSS parsing (future enhancement)
- No authentication
- No database persistence

## Documentation

See `specs/001-add-subscriptions/` for detailed specification, architecture, and task breakdown.

---

**Version**: 1.0.0  
**Status**: MVP Complete ✓  
**Last Updated**: 2024-04-08
