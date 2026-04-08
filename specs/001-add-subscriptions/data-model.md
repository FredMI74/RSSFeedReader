# Data Model: MVP - Add and Manage Feed Subscriptions

**Generated**: 2026-04-08  
**Phase**: 1 (Design)  
**Status**: Complete

## Entities

### Subscription

Represents a single RSS/Atom feed subscription added by the user.

**Entity Definition**:
```csharp
public class Subscription
{
    /// <summary>
    /// Unique identifier for the subscription (GUID or sequential int).
    /// Generated server-side on creation.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The feed URL (full URI path to RSS/Atom feed).
    /// Accepted as-is from user; no validation for MVP.
    /// Examples: https://devblogs.microsoft.com/dotnet/feed/,
    ///           https://feeds.arstechnica.com/arstechnica/index
    /// </summary>
    public string Url { get; set; }
    
    /// <summary>
    /// Timestamp (UTC) when subscription was created.
    /// Used for ordering subscriptions in list (oldest first).
    /// </summary>
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
```

**Attributes**:
- `Id` (int): Server-generated; unique key for this subscription
- `Url` (string): Feed URL; required, non-empty
- `DateAdded` (DateTime): Immutable; set on creation

**Validation Rules**:
- `Url` must be non-null and non-empty (enforced in controller before adding)
- `DateAdded` automatically set to current UTC time
- No format validation for MVP (URL can be any string)
- No RSS/Atom feed verification for MVP

**Relationships**: None for MVP (single entity, no foreign keys or references)

**State Transitions**: None (immutable after creation for MVP; no status/state flag)

---

## Data Transfer Objects

### SubscriptionDto (Frontend)

Represents a subscription for client-side display and serialization.

```csharp
public class SubscriptionDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public DateTime DateAdded { get; set; }
}
```

**Purpose**: Decouples database/API model from frontend representation; enables future addition of computed properties (e.g., feed title, item count) without API changes.

---

## API Contracts

**See [contracts/](./contracts/)** for detailed endpoint specifications.

### Summary of Operations

| Operation | HTTP Method | Endpoint | Purpose |
|-----------|------------|----------|---------|
| Add Subscription | POST | `/api/subscriptions` | Create new subscription with URL |
| List Subscriptions | GET | `/api/subscriptions` | Retrieve all subscriptions |

---

## Storage Schema (In-Memory)

**Runtime representation**:
```csharp
public class SubscriptionService
{
    private readonly List<Subscription> _subscriptions = new();
    
    public void AddSubscription(string url)
    {
        var subscription = new Subscription { Url = url, DateAdded = DateTime.UtcNow };
        subscription.Id = _subscriptions.Count + 1; // Simple sequential ID
        _subscriptions.Add(subscription);
    }
    
    public IEnumerable<Subscription> GetSubscriptions() => _subscriptions;
}
```

**Extended-MVP transition**: This interface allows replacing `List<T>` with EF Core context without changing controllers or service signatures.

---

## Validation & Error Handling

### MVP Scope (No Explicit Validation)
- Empty URLs: Controller skips adding if URL is null/whitespace
- Duplicate URLs: Allowed (no deduplication)
- Malformed URLs: Accepted as-is (no format check)

### Extended-MVP (When Feed Fetching Begins)
- URL format validation (Uri.TryCreate or regex)
- Feed accessibility test (HTTP HEAD request)
- XML/Atom parsing validation

---

## Future Extensions (Extended-MVP, Post-MVP)

### Additional Entities
- **FeedItem**: (Extended-MVP) Title, link, description from parsed feed
- **ReadStatus**: (Post-MVP) Track read/unread state per item per user

### Additional Attributes on Subscription
- `FeedTitle` (Extended-MVP): Parsed from feed metadata
- `LastRefreshed` (Extended-MVP): Timestamp of last successful fetch
- `Status` (Extended-MVP): "Active", "Failed", "Unreachable"
- `ErrorMessage` (Extended-MVP): Reason if fetch failed
- `Tags` (Post-MVP): User-defined categories

### Relationships
- User → Subscriptions (Post-MVP when multi-user support added)
- Subscription → FeedItems (Extended-MVP)

---

**Version**: 0.1.0 | **Last Updated**: 2026-04-08
