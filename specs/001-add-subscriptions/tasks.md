---
description: "Task list for MVP RSS Feed Reader - Add and Manage Feed Subscriptions"
---

# Tasks: MVP - Add and Manage Feed Subscriptions

**Input**: Design documents from `specs/001-add-subscriptions/`  
**Prerequisites**: plan.md ✓, spec.md ✓, research.md ✓, data-model.md ✓, contracts/ ✓

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

**Format**: `[ID] [P?] [Story] Description with file path`
- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story (US1, US2) or phase
- Paths are workspace-relative

---

## Phase 1: Setup (Project Initialization)

**Purpose**: Create project structure and initialize projects

- [ ] T001 Create ASP.NET Core Web API project in `backend/RSSFeedReader.Api/`
- [ ] T002 Create Blazor WebAssembly project in `frontend/RSSFeedReader.UI/`
- [ ] T003 [P] Create xUnit test project in `backend/RSSFeedReader.Api.Tests/`
- [ ] T004 [P] Create xUnit test project in `frontend/RSSFeedReader.UI.Tests/`
- [ ] T005 Update backend `Properties/launchSettings.json` to set port 5151 (HTTP) and 7151 (HTTPS)
- [ ] T006 Update frontend `Properties/launchSettings.json` to set port 5213 (HTTP) and 7213 (HTTPS)
- [ ] T007 Create `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` with `ApiBaseUrl: http://localhost:5151/api/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before any user story implementation

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Backend Foundation

- [ ] T008 Create backend `Models/Subscription.cs` with properties: `Id` (int), `Url` (string), `DateAdded` (DateTime) per [data-model.md](./data-model.md)
- [ ] T009 Create backend `Services/SubscriptionService.cs` with methods `AddSubscription(string url)` and `GetSubscriptions()` per [data-model.md](./data-model.md)
- [ ] T010 Create backend `Controllers/SubscriptionsController.cs` with dependency injection for `SubscriptionService` per [contracts/subscriptions-api.md](./contracts/subscriptions-api.md)
- [ ] T011 Update backend `Program.cs` to register `SubscriptionService` in dependency injection (line: `builder.Services.AddSingleton<SubscriptionService>();`)
- [ ] T012 Update backend `Program.cs` to configure CORS policy `AllowLocalFrontend` allowing ports 5213 and 7213 per [plan.md](./plan.md)
- [ ] T013 Update backend `Program.cs` to call `app.UseCors("AllowLocalFrontend")` before routing

### Frontend Foundation

- [ ] T014 Delete template demo pages: `frontend/RSSFeedReader.UI/Pages/Home.razor`, `Counter.razor`, `Weather.razor` per [quickstart.md](./quickstart.md)
- [ ] T015 Update `frontend/RSSFeedReader.UI/Layout/NavMenu.razor` to remove demo navigation links and prepare for MVP pages
- [ ] T016 Update frontend `Program.cs` to register `HttpClient` with base address from `ApiBaseUrl` configuration per [quickstart.md](./quickstart.md)
- [ ] T017 Create frontend `Services/SubscriptionApiService.cs` with methods `GetSubscriptionsAsync()` and `AddSubscriptionAsync(string url)` per [contracts/subscriptions-api.md](./contracts/subscriptions-api.md)
- [ ] T018 Create frontend `Models/SubscriptionDto.cs` with properties: `Id` (int), `Url` (string), `DateAdded` (DateTime) matching backend contract

### Verification

- [ ] T019 Verify backend builds without errors: `dotnet build backend/RSSFeedReader.Api/`
- [ ] T020 Verify frontend builds without errors: `dotnet build frontend/RSSFeedReader.UI/`
- [ ] T021 Verify no routing conflicts in frontend (no duplicate `@page "/"` directives)

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Add a Feed Subscription (Priority: P1)

**Goal**: Implement backend API and frontend UI to accept feed URL and add it to subscription list

**Independent Test**: User can enter a feed URL in input field, click "Add", and see it appear in table

### Tests for User Story 1 (Unit + Integration)

- [ ] T022 [P] [US1] Write unit test `backend/RSSFeedReader.Api.Tests/Unit/SubscriptionServiceTests.cs` with test: `AddSubscription_ShouldStoreSubscription_AndReturnInList()`
- [ ] T023 [P] [US1] Write unit test `backend/RSSFeedReader.Api.Tests/Unit/SubscriptionServiceTests.cs` with test: `AddSubscription_MultipleUrls_ShouldStoreAll()`
- [ ] T024 [US1] Write integration test `backend/RSSFeedReader.Api.Tests/Integration/SubscriptionsControllerTests.cs` with test: `PostSubscription_ShouldAddAndReturnUpdatedList()`
- [ ] T025 [US1] Write integration test `backend/RSSFeedReader.Api.Tests/Integration/SubscriptionsControllerTests.cs` with test: `PostSubscription_WithEmptyUrl_ShouldReturnBadRequest()`

### Implementation for User Story 1

- [ ] T026 [US1] Implement `SubscriptionsController.Post([FromBody] AddSubscriptionRequest request)` method in `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs` to:
  - Validate request is not null and URL is not empty
  - Call `_service.AddSubscription(request.Url)`
  - Return 200 OK with updated subscription list
  - Per [contracts/subscriptions-api.md](./contracts/subscriptions-api.md)

- [ ] T027 [US1] Create `AddSubscriptionRequest` DTO in `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs` with property `Url` (string)

- [ ] T028 [US1] Create `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` component with:
  - Input field for feed URL (per [quickstart.md](./quickstart.md))
  - "Add Subscription" button
  - Calls `await ApiService.AddSubscriptionAsync(newUrl)`
  - Clears input field after successful add
  - Refreshes subscription list

- [ ] T029 [US1] Implement `SubscriptionApiService.AddSubscriptionAsync(string url)` in `frontend/RSSFeedReader.UI/Services/SubscriptionApiService.cs`:
  - POST to `/subscriptions` with `{ url }` body
  - Return true on success, false on error
  - Per [contracts/subscriptions-api.md](./contracts/subscriptions-api.md)

- [ ] T030 [P] [US1] Update `@page` directive in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` to `@page "/"` to set as landing page

- [ ] T031 [US1] Verify User Story 1 tests pass: `dotnet test backend/RSSFeedReader.Api.Tests/`

**Checkpoint**: User Story 1 is complete and independently testable - users can add subscriptions via API

---

## Phase 4: User Story 2 - View All Subscriptions (Priority: P1)

**Goal**: Implement backend API and frontend UI to display list of all subscriptions

**Independent Test**: Application displays table of all subscriptions in order added; empty state shown when no subscriptions exist

### Tests for User Story 2 (Unit + Integration)

- [ ] T032 [P] [US2] Write unit test `backend/RSSFeedReader.Api.Tests/Unit/SubscriptionServiceTests.cs` with test: `GetSubscriptions_Empty_ShouldReturnEmptyList()`
- [ ] T033 [P] [US2] Write unit test `backend/RSSFeedReader.Api.Tests/Unit/SubscriptionServiceTests.cs` with test: `GetSubscriptions_WithSubscriptions_ShouldReturnOrderedByDateAdded()`
- [ ] T034 [US2] Write integration test `backend/RSSFeedReader.Api.Tests/Integration/SubscriptionsControllerTests.cs` with test: `GetSubscriptions_ShouldReturnAllSubscriptions()`
- [ ] T035 [US2] Write integration test `backend/RSSFeedReader.Api.Tests/Integration/SubscriptionsControllerTests.cs` with test: `GetSubscriptions_Empty_ShouldReturnEmptyArray()`

### Implementation for User Story 2

- [ ] T036 [US2] Implement `SubscriptionsController.Get()` method in `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs` to:
  - Call `_service.GetSubscriptions()`
  - Return 200 OK with list of subscriptions (or empty array if none)
  - Per [contracts/subscriptions-api.md](./contracts/subscriptions-api.md)

- [ ] T037 [US2] Implement `SubscriptionApiService.GetSubscriptionsAsync()` in `frontend/RSSFeedReader.UI/Services/SubscriptionApiService.cs`:
  - GET from `/subscriptions`
  - Parse JSON response to List<SubscriptionDto>
  - Return empty list on error
  - Per [contracts/subscriptions-api.md](./contracts/subscriptions-api.md)

- [ ] T038 [US2] Add to `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`:
  - `OnInitializedAsync()` lifecycle method to load subscriptions on page load
  - Call `await ApiService.GetSubscriptionsAsync()` and store in local state
  - Per [quickstart.md](./quickstart.md)

- [ ] T039 [US2] Add table display to `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`:
  - Display columns: URL (as link), DateAdded (formatted as YYYY-MM-DD HH:mm:ss)
  - Show empty state message if no subscriptions
  - Update table after each add (reload from GetSubscriptionsAsync)
  - Per [quickstart.md](./quickstart.md)

- [ ] T040 [US2] Verify User Story 2 tests pass: `dotnet test backend/RSSFeedReader.Api.Tests/`

**Checkpoint**: Both user stories complete - users can add and view subscriptions independently

---

## Phase 5: Integration & Verification

**Purpose**: Verify end-to-end MVP workflow and all requirements met

### End-to-End Testing

- [ ] T041 Start backend: `dotnet run --project backend/RSSFeedReader.Api/` and verify listens on `http://localhost:5151` (manual test)

- [ ] T042 Start frontend: `dotnet run --project frontend/RSSFeedReader.UI/` and verify loads at `http://localhost:5213` (manual test)

- [ ] T043 Verify no console errors in browser DevTools (F12 → Console tab) - no CORS, network, or JavaScript errors

- [ ] T044 Manual workflow test: Add subscription `https://devblogs.microsoft.com/dotnet/feed/` and verify appears in table with current timestamp

- [ ] T045 Manual workflow test: Add second subscription `https://feeds.arstechnica.com/arstechnica/index` and verify both appear in table in order added

- [ ] T046 Manual workflow test: Refresh frontend page (Ctrl+R) and verify list persists (data is in-memory on backend, survives page reload)

- [ ] T047 Manual workflow test: Restart backend, refresh frontend, and verify list is empty (confirms in-memory storage, data lost on restart)

### Requirements Verification

- [ ] T048 Verify FR-001: Users can enter feed URL in input field on landing page (manual inspection of Subscriptions.razor)

- [ ] T049 Verify FR-002: Users can click "Add" button to submit URL (manual test: click button, verify network request)

- [ ] T050 Verify FR-003: Backend API accepts POST to `/api/subscriptions` with `{ url }` body (test with curl or browser Network tab)

- [ ] T051 Verify FR-004: Backend API returns updated subscription list after add (test with curl: `curl -X POST http://localhost:5151/api/subscriptions -H "Content-Type: application/json" -d "{\"url\":\"https://example.com/feed\"}"`)

- [ ] T052 Verify FR-005: Frontend displays subscription list in table format (manual inspection)

- [ ] T053 Verify FR-006: Input field clears after successful add (manual test)

- [ ] T054 Verify FR-007: Only "Add" button shown, no edit/delete/refresh buttons (manual inspection of Subscriptions.razor)

- [ ] T055 Verify FR-008: CORS configured and frontend can reach backend (no CORS errors in browser console)

- [ ] T056 Verify FR-009: Application runs on Windows, macOS, or Linux (test on available platforms or verify .NET cross-platform support)

- [ ] T057 Verify FR-010: Subscriptions stored in memory only (verify disappear on backend restart)

### Success Criteria Verification

- [ ] T058 Verify SC-001: Add subscription and see in list within 1 second (manual stopwatch test or DevTools Network timing)

- [ ] T059 Verify SC-002: Add 50+ subscriptions and verify table remains responsive (add via loop or manual testing)

- [ ] T060 Verify SC-003: Build and run on Windows, macOS, Linux without errors (manual verification or CI/CD pipeline test)

- [ ] T061 Verify SC-004: Complete workflow (add 3 subscriptions, view all 3) in under 2 minutes (manual timing)

- [ ] T062 Verify SC-005: All 10 functional requirements (FR-001 through FR-010) implemented and working (cross-check with T048-T057)

---

## Phase 6: Polish & Code Quality

**Purpose**: Finalize implementation with code review and minor improvements

- [ ] T063 Code review: Verify all class and method names are meaningful and follow C# conventions (PascalCase for classes/methods, camelCase for locals)

- [ ] T064 Code review: Verify no hardcoded credentials, API URLs, or secrets in code (check Program.cs, appsettings.json, services)

- [ ] T065 Code review: Verify CORS policy is explicit (allow specific ports, not AllowAnyOrigin) per Constitution Principle I

- [ ] T066 Code review: Verify no business logic duplication between frontend and backend (business logic in services, not controllers or components)

- [ ] T067 Verify all tests pass: `dotnet test backend/RSSFeedReader.Api.Tests/ && dotnet test frontend/RSSFeedReader.UI.Tests/`

- [ ] T068 Create integration test scenario in `frontend/RSSFeedReader.UI.Tests/Integration/SubscriptionWorkflowTests.cs` to test full add→list flow

- [ ] T069 Document any test gaps or deferred testing (e.g., error handling, edge cases) in project notes for Extended-MVP

- [ ] T070 Verify solution builds clean: `dotnet clean && dotnet build`

---

## Phase 7: Documentation & Handoff

**Purpose**: Ensure implementation is documented for future development

- [ ] T071 Verify `backend/RSSFeedReader.Api/` has XML doc comments on public classes and methods

- [ ] T072 Verify `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` has comments explaining key logic (AddSubscription, LoadSubscriptions)

- [ ] T073 Update or create `README.md` with:
  - How to build and run both backend and frontend
  - Expected ports and how to verify connectivity
  - Manual testing steps for MVP workflow
  - Known limitations (in-memory only, no validation, etc.)

- [ ] T074 Verify all design artifacts in `specs/001-add-subscriptions/` are accurate and reflect final implementation

---

## Dependency Graph & Parallel Execution

### Critical Path (Sequential, Cannot Parallelize)

T001 → T005 → Backend Foundation (T008-T013) → User Story 1 & 2 (can parallelize)  
T002 → T006 → T014 → Frontend Foundation (T015-T018) → User Story 1 & 2 (can parallelize)

### Parallelizable Sections

**After Phase 2 Foundation is complete (T019-T021)**:

- **User Story 1 (T022-T031)** and **User Story 2 (T032-T040)** can be implemented in parallel
  - Different files: Controllers, Services, Blazor pages
  - Two developers can work simultaneously without conflicts
  - Both stories independently testable

**Within each story**:
- Tests (T022-T025 for US1, T032-T035 for US2) can be written in parallel while implementation continues
- Backend implementation and frontend implementation can proceed in parallel

### Suggested Team Execution

- **Developer 1**: Backend setup (T001-T013), then User Story 1 backend (T026-T027)
- **Developer 2**: Frontend setup (T002-T018), then User Story 1 frontend (T028-T030)
- **Developer 1**: User Story 2 backend (T036) in parallel with Developer 2 working on US2 frontend (T038-T039)
- **Both**: Verification phase (T041-T062) together to validate MVP

---

## Implementation Strategy

### MVP First Delivery

**Phase 1-2** (Estimated: 2-3 hours):  
Set up projects, create models, services, controllers, and basic frontend bootstrap

**Phase 3-4** (Estimated: 3-4 hours):  
Implement both user stories with tests; total of ~23 development tasks

**Phase 5** (Estimated: 1-2 hours):  
Manual testing and verification of all acceptance criteria

**Total MVP Delivery Time**: 6-9 hours (one developer or two in parallel)

### Success Criteria for MVP Completion

✅ All Phase 1-2 foundation tasks complete  
✅ User Story 1 tests pass (add subscription)  
✅ User Story 2 tests pass (list subscriptions)  
✅ End-to-end manual workflow succeeds (add 3 subscriptions, view all in table)  
✅ No build errors or CORS errors  
✅ All 10 functional requirements verified working

---

**Version**: 0.1.0 Draft | **Last Updated**: 2026-04-08  
**Total Tasks**: 74 | **Parallelizable Tasks**: 15 (marked with [P]) | **Test Tasks**: 8
