# API Contract: Subscriptions Endpoints

**Version**: 1.0.0  
**Generated**: 2026-04-08  
**Base URL**: `http://localhost:5151/api` (or `https://localhost:7151/api` in development)

---

## Endpoint: Add Subscription

### POST `/api/subscriptions`

Create a new feed subscription.

**Request**:

```http
POST /api/subscriptions HTTP/1.1
Host: localhost:5151
Content-Type: application/json

{
    "url": "https://devblogs.microsoft.com/dotnet/feed/"
}
```

**Request Body Schema**:
```json
{
    "url": "string (required, non-empty)"
}
```

**Response** (Success - 200 OK):

```http
HTTP/1.1 200 OK
Content-Type: application/json

[
    {
        "id": 1,
        "url": "https://devblogs.microsoft.com/dotnet/feed/",
        "dateAdded": "2026-04-08T14:30:00Z"
    },
    {
        "id": 2,
        "url": "https://feeds.arstechnica.com/arstechnica/index",
        "dateAdded": "2026-04-08T14:31:00Z"
    }
]
```

**Response Body Schema**:
```json
[
    {
        "id": "integer",
        "url": "string",
        "dateAdded": "ISO 8601 datetime (UTC)"
    }
]
```

**Failure Cases**:
- **400 Bad Request**: URL is null, empty, or missing from body
- **500 Internal Server Error**: Unexpected server error (MVP: not expected to occur)

---

## Endpoint: List Subscriptions

### GET `/api/subscriptions`

Retrieve all subscriptions.

**Request**:

```http
GET /api/subscriptions HTTP/1.1
Host: localhost:5151
Accept: application/json
```

**Query Parameters**: None for MVP

**Response** (Success - 200 OK):

```http
HTTP/1.1 200 OK
Content-Type: application/json

[
    {
        "id": 1,
        "url": "https://devblogs.microsoft.com/dotnet/feed/",
        "dateAdded": "2026-04-08T14:30:00Z"
    },
    {
        "id": 2,
        "url": "https://feeds.arstechnica.com/arstechnica/index",
        "dateAdded": "2026-04-08T14:31:00Z"
    }
]
```

**Response States**:
- **Empty list** (200 OK): `[]` when no subscriptions exist
- **With subscriptions** (200 OK): Array of subscription objects ordered by DateAdded (oldest first)

**Failure Cases**:
- **500 Internal Server Error**: Unexpected server error (MVP: not expected to occur)

---

## Data Types

### Subscription Object

```json
{
    "id": "integer, auto-generated, unique",
    "url": "string, feed URL as-provided by user",
    "dateAdded": "ISO 8601 datetime in UTC (e.g., 2026-04-08T14:30:00Z)"
}
```

---

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 OK | Request successful; subscriptions returned |
| 400 Bad Request | URL missing or invalid in POST body |
| 500 Internal Server Error | Server-side error (not expected for MVP) |

---

## CORS Configuration

The API allows requests from the frontend origin:

```
Allow-Origin: http://localhost:5213, https://localhost:7213
Allow-Methods: GET, POST, OPTIONS
Allow-Headers: Content-Type
```

**Impact**: Frontend JavaScript code can call these endpoints without CORS errors.

---

## Contract Notes for Implementation

1. **POST response includes updated list**: Allows frontend to update UI immediately without a separate GET call
2. **Always returns all subscriptions**: No pagination for MVP (<100 subscriptions expected)
3. **ID generation**: Sequential integer (1, 2, 3...) for simplicity; use GUID in production
4. **UTC timestamps**: All times in UTC; no timezone conversion for MVP
5. **No filtering**: GET returns all subscriptions; filtering deferred to Extended-MVP

---

## Future Extensions (Extended-MVP)

### Potential additions:
- DELETE `/api/subscriptions/{id}` - Remove subscription
- PUT `/api/subscriptions/{id}` - Update subscription (mark as refreshed)
- GET `/api/subscriptions/{id}/items` - Get items from feed
- POST `/api/subscriptions/{id}/refresh` - Manually refresh feed

---

**Version**: 1.0.0 | **Last Updated**: 2026-04-08
