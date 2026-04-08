namespace RSSFeedReader.UI.Models;

/// <summary>
/// Data transfer object for subscription (sent/received from backend).
/// </summary>
public class SubscriptionDto
{
    /// <summary>
    /// Unique identifier for the subscription.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The feed URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// When the subscription was created (UTC).
    /// </summary>
    public DateTime DateAdded { get; set; }
}
