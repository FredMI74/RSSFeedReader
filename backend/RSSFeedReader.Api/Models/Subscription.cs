namespace RSSFeedReader.Api.Models;

/// <summary>
/// Represents a single RSS/Atom feed subscription.
/// </summary>
public class Subscription
{
    /// <summary>
    /// Unique identifier for the subscription (server-generated).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The feed URL (feed URI path) as provided by the user.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// UTC timestamp when subscription was created.
    /// </summary>
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
