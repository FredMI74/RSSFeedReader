# Quickstart: MVP - Add and Manage Feed Subscriptions

**Purpose**: Get the MVP subscription management feature running locally and verify it works end-to-end.  
**Time Required**: ~15 minutes (assuming .NET SDK already installed)  
**Date Created**: 2026-04-08

---

## Prerequisites

- **.NET SDK**: Version 7.0 LTS or later (check with `dotnet --version`)
- **Git**: For branch checkout (optional)
- **Browser**: Chrome, Edge, or Firefox (verify WebAssembly support)
- **Terminal**: PowerShell, Bash, or cmd

---

## Step 1: Project Initialization

### 1a. Create Backend Project

```powershell
cd backend
dotnet new webapi -n RSSFeedReader.Api --framework net7.0
cd RSSFeedReader.Api
```

**Expected output**: Project template created with default controllers and Program.cs

### 1b. Create Frontend Project

```powershell
cd frontend
dotnet new blazorwasm -n RSSFeedReader.UI --framework net7.0
cd RSSFeedReader.UI
```

**Expected output**: Blazor WebAssembly project with template pages (Home, Counter, Weather)

### 1c. Create Unit Test Projects

```powershell
# Backend tests
cd backend
dotnet new xunit -n RSSFeedReader.Api.Tests --framework net7.0

# Frontend tests (optional for MVP)
cd frontend
dotnet new xunit -n RSSFeedReader.UI.Tests --framework net7.0
```

---

## Step 2: Implement Backend (Subscription Management)

### 2a. Create Models/Subscription.cs

```csharp
public class Subscription
{
    public int Id { get; set; }
    public string Url { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
```

### 2b. Create Services/SubscriptionService.cs

```csharp
public class SubscriptionService
{
    private readonly List<Subscription> _subscriptions = new();
    private int _nextId = 1;

    public Subscription AddSubscription(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        var subscription = new Subscription
        {
            Id = _nextId++,
            Url = url,
            DateAdded = DateTime.UtcNow
        };

        _subscriptions.Add(subscription);
        return subscription;
    }

    public IEnumerable<Subscription> GetSubscriptions()
    {
        return _subscriptions.OrderBy(s => s.DateAdded);
    }
}
```

### 2c. Create Controllers/SubscriptionsController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _service;

    public SubscriptionsController(SubscriptionService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var subscriptions = _service.GetSubscriptions();
        return Ok(subscriptions);
    }

    [HttpPost]
    public IActionResult Post([FromBody] AddSubscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Url))
            return BadRequest("URL is required");

        try
        {
            _service.AddSubscription(request.Url);
            var subscriptions = _service.GetSubscriptions();
            return Ok(subscriptions);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class AddSubscriptionRequest
{
    public string Url { get; set; }
}
```

### 2d. Update Program.cs (CORS + Dependency Injection)

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<SubscriptionService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5213", "https://localhost:7213")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowLocalFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 2e. Update launchSettings.json (Port Configuration)

Set backend to port 5151:

```json
{
  "profiles": {
    "RSSFeedReader.Api": {
      "commandName": "Project",
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5151;https://localhost:7151",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## Step 3: Implement Frontend (Blazor UI)

### 3a. **CRITICAL**: Clean Up Template Pages

Remove demo pages that conflict with MVP:

```powershell
# From frontend/RSSFeedReader.UI/Pages/
Remove-Item Home.razor
Remove-Item Counter.razor
Remove-Item Weather.razor
```

Update `Layout/NavMenu.razor` to remove demo navigation links.

### 3b. Create Services/SubscriptionApiService.cs

```csharp
public class SubscriptionApiService
{
    private readonly HttpClient _httpClient;

    public SubscriptionApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SubscriptionDto>> GetSubscriptionsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<SubscriptionDto>>("subscriptions") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching subscriptions: {ex.Message}");
            return new();
        }
    }

    public async Task<bool> AddSubscriptionAsync(string url)
    {
        try
        {
            var request = new { url };
            var response = await _httpClient.PostAsJsonAsync("subscriptions", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding subscription: {ex.Message}");
            return false;
        }
    }
}

public class SubscriptionDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public DateTime DateAdded { get; set; }
}
```

### 3c. Update wwwroot/appsettings.json

Configure backend API URL:

```json
{
  "ApiBaseUrl": "http://localhost:5151/api/"
}
```

### 3d. Update Program.cs (Bootstrap + HttpClient)

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5151/api/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<SubscriptionApiService>();

await builder.Build().RunAsync();
```

### 3e. Create Pages/Subscriptions.razor

```razor
@page "/"
@inject SubscriptionApiService ApiService

<div class="container mt-4">
    <h1>RSS Feed Subscriptions</h1>
    
    <div class="mb-3">
        <input type="text" class="form-control" placeholder="Enter feed URL..." @bind="newUrl" />
        <button class="btn btn-primary mt-2" @onclick="AddSubscription">Add Subscription</button>
    </div>

    @if (subscriptions == null)
    {
        <p>Loading subscriptions...</p>
    }
    else if (subscriptions.Count == 0)
    {
        <p class="text-muted">No subscriptions yet. Add one to get started!</p>
    }
    else
    {
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>URL</th>
                    <th>Added</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var sub in subscriptions)
                {
                    <tr>
                        <td><a href="@sub.Url" target="_blank">@sub.Url</a></td>
                        <td>@sub.DateAdded.ToString("yyyy-MM-dd HH:mm:ss")</td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<SubscriptionDto> subscriptions = new();
    private string newUrl = "";

    protected override async Task OnInitializedAsync()
    {
        await LoadSubscriptions();
    }

    private async Task LoadSubscriptions()
    {
        subscriptions = await ApiService.GetSubscriptionsAsync();
    }

    private async Task AddSubscription()
    {
        if (string.IsNullOrWhiteSpace(newUrl)) return;

        await ApiService.AddSubscriptionAsync(newUrl);
        newUrl = "";
        await LoadSubscriptions();
    }
}
```

### 3f. Update launchSettings.json (Port Configuration)

Set frontend to port 5213:

```json
{
  "profiles": {
    "RSSFeedReader.UI": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5213;https://localhost:7213",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## Step 4: Verify Setup

### 4a. Test Backend Runs

```powershell
cd backend/RSSFeedReader.Api
dotnet run
```

**Expected**: Backend listens on `http://localhost:5151`  
**Verify**: Open browser, navigate to `http://localhost:5151/api/subscriptions` → should return `[]`

### 4b. Test Frontend Runs

```powershell
cd frontend/RSSFeedReader.UI
dotnet run
```

**Expected**: Frontend loads at `http://localhost:5213`  
**Verify**: Browser opens; no console errors (F12 → Console)

---

## Step 5: End-to-End MVP Workflow Test

1. **Backend running**: `http://localhost:5151/api` ✓
2. **Frontend running**: `http://localhost:5213` ✓
3. **Add subscription**:
   - Enter: `https://devblogs.microsoft.com/dotnet/feed/`
   - Click "Add Subscription"
   - **Expected**: URL appears in table below
4. **Add second subscription**:
   - Enter: `https://feeds.arstechnica.com/arstechnica/index`
   - Click "Add Subscription"
   - **Expected**: Both URLs now appear in table
5. **Refresh page** (Ctrl+R):
   - **Expected**: List is empty (MVP data is in-memory, lost on restart)
   - **Verify this is expected**: Demonstrates "in-memory only" design

---

## Troubleshooting

| Problem | Cause | Solution |
|---------|-------|----------|
| Frontend can't reach API | CORS not configured | Check Program.cs includes `app.UseCors()` |
| "Ambiguous route" error | Template pages not removed | Delete Home.razor, Counter.razor, Weather.razor from Pages/ |
| Port already in use | Another process using 5151 or 5213 | Change port in launchSettings.json; update appsettings.json |
| Subscriptions disappear on refresh | Expected | MVP uses in-memory storage; persistence added in Extended-MVP |
| Blank page in browser | Blazor not running | Check browser console (F12) for errors; rebuild frontend |

---

## Next Steps

After MVP is verified working:

1. **Write unit tests** for SubscriptionService (see [data-model.md](./data-model.md))
2. **Run integration tests** for API endpoints
3. **Code review**: Verify security principle compliance (no hardcoded URLs, CORS explicit, etc.)
4. **Extended-MVP**: Add feed fetching and item display (see ProjectGoals.md)

---

**Version**: 1.0.0 | **Last Updated**: 2026-04-08
