using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

/// <summary>
/// Service for managing feed subscriptions (in-memory storage for MVP).
/// </summary>
public class SubscriptionService
{
    private readonly List<Subscription> _subscriptions = new();
    private int _nextId = 1;

    /// <summary>
    /// Add a new subscription to the list.
    /// </summary>
    /// <param name="url">Feed URL to subscribe to</param>
    /// <returns>The created subscription</returns>
    /// <exception cref="ArgumentException">Thrown if URL is null or empty</exception>
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

    /// <summary>
    /// Get all subscriptions ordered by date added (oldest first).
    /// </summary>
    /// <returns>List of all subscriptions</returns>
    public IEnumerable<Subscription> GetSubscriptions()
    {
        return _subscriptions.OrderBy(s => s.DateAdded);
    }
}
