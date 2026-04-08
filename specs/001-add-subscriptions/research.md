# Research: MVP - Add and Manage Feed Subscriptions

**Generated**: 2026-04-08  
**Phase**: 0 (Research & Clarification Resolution)  
**Status**: Complete - No NEEDS CLARIFICATION markers remain

## Overview

This document resolves all technical unknowns and clarifies decisions for MVP subscription management feature. Specification had no ambiguities; research consolidates architecture and technology decisions from stakeholder documents into actionable guidance.

---

## Decision 1: Architecture Pattern - Frontend + Backend Separation

**Context**: Feature requires web UI (add subscription, view list) and data management (store subscriptions).

**Decision**: ASP.NET Core Web API (backend) + Blazor WebAssembly (frontend)  
**Rationale**:
- **Separation of concerns**: Clean boundary enables independent testing and future scaling
- **Rapid MVP development**: Both technologies minimize boilerplate; Blazor eliminates JavaScript learning curve
- **Cross-platform**: .NET Core runtime supports Windows, macOS, Linux
- **Shared language**: C# front and back enables potential code sharing (entities, validation)
- **Future-ready**: Architecture supports adding feed fetching, persistence, background services without redesign

**Alternatives considered**:
- **ASP.NET Core MVC (monolithic)**: Slower to separate later when Adding Extended-MVP feed service; tighter coupling between UI and data logic
- **Node.js + React**: Requires learning JavaScript ecosystem; added complexity for POC

**Confidence**: ✅ HIGH - Explicitly mandated in TechStack.md; stakeholders validated this choice

---

## Decision 2: Data Storage - In-Memory Only (MVP Phase)

**Context**: Specification requires storing subscriptions; no persistence requirement stated for MVP.

**Decision**: In-memory `List<Subscription>` in backend service; data lost on application restart  
**Rationale**:
- **MVP scope**: Single-user, local development; persistence not required for demonstration
- **Fastest implementation**: No database setup, migrations, ORM learning curve
- **Testing simplicity**: No database state to reset between tests
- **Architecture prepared**: Service abstraction allows swapping in database layer in Extended-MVP without UI/controller changes

**Implementation**:
```csharp
public class SubscriptionService 
{
    private readonly List<Subscription> _subscriptions = new();
    
    public void AddSubscription(string url) => _subscriptions.Add(new Subscription { Url = url });
    public List<Subscription> GetSubscriptions() => _subscriptions.ToList();
}
```

**Alternatives considered**:
- **SQLite**: Adds complexity; unnecessary for MVP scope
- **Cosmos DB/Azure**: Cloud dependency; defeats local-only goal

**Transition to Extended-MVP**: Service interface (`ISubscriptionRepository`) allows simple swap to EF Core + SQLite without changing controllers or Blazor components.

**Confidence**: ✅ HIGH - Clearly documented in ProjectGoals.md and TechStack.md

---

## Decision 3: Input Validation - No URL Validation (MVP), Prepared for Extended-MVP

**Context**: Specification states "No URL validation" for MVP. Security principle mandates safe input handling.

**Decision**: 
- **MVP**: Accept any URL string without validation (no HTTP verification)
- **Input sanitization**: Quote or escape URL if stored in HTML (Blazor handles this by default)
- **Extended-MVP**: Add basic URL format validation and feed accessibility check when fetching begins

**Rationale**:
- **MVP speed**: Feed fetching not required; validation adds no value until HTTP requests occur
- **Security principle honored**: Safe handling (no SQL injection, HTML escaping) without unnecessary complexity
- **Future-ready**: When Extended-MVP fetches feeds, validation logic isolates to feed service, not subscription add API

**Implementation**:
```csharp
// MVP: Accept any URL string
public void AddSubscription(string url) 
{
    if (string.IsNullOrWhiteSpace(url)) return; // Basic safety
    _subscriptions.Add(new Subscription { Url = url });
}

// Extended-MVP: Validate URL format and fetch-ability before adding
public async Task<ValidationResult> ValidateAndAddSubscriptionAsync(string url)
{
    if (!Uri.TryCreate(url, UriKind.Absolute, out _)) 
        return ValidationResult.Invalid("Invalid URL format");
    
    var feed = await _feedService.FetchAsync(url);
    if (feed == null) return ValidationResult.Invalid("Feed not found");
    
    _subscriptions.Add(new Subscription { Url = url });
    return ValidationResult.Success();
}
```

**Constitution alignment**: Security principle (I) satisfied by safe coding practices; URL validation precision deferred to when network security becomes relevant.

**Confidence**: ✅ HIGH - Specification and stakeholders explicitly defer validation

---

## Decision 4: CORS Configuration - Explicit Allow for Local Ports

**Context**: Frontend and backend run on different ports (5213 vs 5151). CORS must be configured in backend.

**Decision**: Explicitly configure CORS to allow frontend port in local development; restrict in production  
**Implementation**:
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5213", "https://localhost:7213")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowLocalFrontend");
```

**Rationale**:
- **Security principle (I) honored**: Explicitly configured CORS prevents accidental exposure
- **Future**: Production environment configuration documented separately
- **Testing**: Easy to verify CORS working (check browser console for CORS errors)

**Alternatives considered**:
- **AllowAnyOrigin()**: Dangerous for any environment; violates security principle
- **No CORS configuration**: API requests fail silently with cryptic browser errors

**Confidence**: ✅ HIGH - Standard ASP.NET Core practice; TechStack.md explicitly mentions CORS requirement

---

## Decision 5: Testing Strategy - Unit + Integration Focus

**Context**: Constitution principle (IV) mandates testing; specification defines clear workflows.

**Decision**:
- **Unit tests** (`SubscriptionServiceTests`): Test AddSubscription and GetSubscriptions logic in isolation
- **Integration tests** (`SubscriptionsControllerTests`): Test API endpoints (POST /subscriptions, GET /subscriptions) with real service
- **Manual workflow test**: User adds 3 subscriptions via UI, verifies they appear in list

**Implementation**:
```csharp
// Unit test
[Fact]
public void AddSubscription_Should_Store_And_Return()
{
    var service = new SubscriptionService();
    service.AddSubscription("https://example.com/feed");
    
    var result = service.GetSubscriptions();
    Assert.Single(result);
    Assert.Equal("https://example.com/feed", result[0].Url);
}

// Integration test
[Fact]
public async Task PostSubscription_Should_Add_And_Return_Updated_List()
{
    var client = new HttpClient { BaseAddress = new Uri("http://localhost:5151") };
    var response = await client.PostAsJsonAsync("/api/subscriptions", 
        new { url = "https://example.com/feed" });
    
    Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
    // Verify subscription appears in response
}
```

**Rationale**:
- **Unit tests** verify business logic independent of HTTP
- **Integration tests** verify API contract (gateway between frontend and backend)
- **Manual testing** validates end-to-end user workflow
- **Test gaps documented**: Feed parsing, error handling deferred to Extended-MVP

**Confidence**: ✅ HIGH - Clear separation enables efficient testing; manual workflow covers primary path

---

## Decision 6: State Management in Blazor Frontend

**Context**: Frontend must add subscriptions and display current list; no persistence required.

**Decision**: Blazor component state (local List<SubscriptionDto>) + API service for backend communication  
**Implementation**:
```csharp
// SubscriptionApiService.cs - HTTP client to backend
public class SubscriptionApiService 
{
    private readonly HttpClient _http;
    public async Task<List<SubscriptionDto>> GetSubscriptionsAsync() 
        => await _http.GetFromJsonAsync<List<SubscriptionDto>>("/api/subscriptions");
    
    public async Task AddSubscriptionAsync(string url)
        => await _http.PostAsJsonAsync("/api/subscriptions", new { url });
}

// Subscriptions.razor - Component state
@page "/"
@inject SubscriptionApiService ApiService

@code {
    private List<SubscriptionDto> subscriptions = new();
    private string newUrl = "";
    
    protected override async Task OnInitializedAsync()
    {
        subscriptions = await ApiService.GetSubscriptionsAsync();
    }
    
    private async Task AddSubscription()
    {
        if (string.IsNullOrWhiteSpace(newUrl)) return;
        
        await ApiService.AddSubscriptionAsync(newUrl);
        subscriptions = await ApiService.GetSubscriptionsAsync();
        newUrl = "";
    }
}
```

**Rationale**:
- **Simple**: Component state sufficient for MVP (no Redux, Mobx, or complex management)
- **Responsive**: Blazor re-renders on state change; list updates immediately after add
- **Testable**: Service is injectable; easy to mock for component tests

**Confidence**: ✅ HIGH - Blazor standard pattern; matches MVP scope

---

## Decision 7: Project Structure and Folder Organization

**Context**: Need clear separation of concerns while keeping MVP simple.

**Decision**: 
- **Backend**: `RSSFeedReader.Api` (controllers, models, services), `RSSFeedReader.Api.Tests`
- **Frontend**: `RSSFeedReader.UI` (pages, services, layout), `RSSFeedReader.UI.Tests`
- **Configuration**: launchSettings.json for port coordination, appsettings.json for API URL

**Rationale**:
- **Scalability**: Folder structure supports adding more services (feed service) and pages (future features) without refactoring
- **Testing proximity**: Tests near source code for discoverability
- **Clear responsibilities**: API folder → data/logic; UI folder → presentation

**Confidence**: ✅ HIGH - Matches ASP.NET Core conventions and supports future growth

---

## Summary of Resolutions

| Item | Status | Outcome |
|------|--------|---------|
| Architecture | Resolved | ASP.NET Core + Blazor WebAssembly |
| Data Storage | Resolved | In-memory List<Subscription> |
| Input Handling | Resolved | Accept any URL; no validation for MVP |
| CORS | Resolved | Explicit allow for localhost ports 5213 + 7213 |
| Testing | Resolved | Unit + integration + manual (workflow test) |
| State Management | Resolved | Blazor component state + API service |
| Project Structure | Resolved | Backend/frontend separation with test proximity |

**All clarifications complete. Proceed to Phase 1 (Design).**
