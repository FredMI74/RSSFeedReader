using RSSFeedReader.Api.Services;
using Xunit;

namespace RSSFeedReader.Api.Tests.Services;

/// <summary>
/// Unit tests for SubscriptionService.
/// </summary>
public class SubscriptionServiceTests
{
    /// <summary>
    /// Test T022: GetSubscriptions returns empty list on fresh instance.
    /// </summary>
    [Fact]
    public void GetSubscriptions_OnFreshService_ReturnsEmptyList()
    {
        // Arrange
        var service = new SubscriptionService();

        // Act
        var result = service.GetSubscriptions();

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Test T023: AddSubscription adds subscription with correct Url and DateAdded.
    /// </summary>
    [Fact]
    public void AddSubscription_WithValidUrl_AddsSubscriptionAndReturnsIt()
    {
        // Arrange
        var service = new SubscriptionService();
        var testUrl = "https://example.com/feed";

        // Act
        service.AddSubscription(testUrl);
        var result = service.GetSubscriptions().ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(testUrl, result[0].Url);
        Assert.NotEqual(default(DateTime), result[0].DateAdded);
        Assert.True(result[0].DateAdded <= DateTime.UtcNow);
    }

    /// <summary>
    /// Test T024: AddSubscription with multiple subscriptions maintains order.
    /// </summary>
    [Fact]
    public void AddSubscription_WithMultipleUrls_MaintainsInsertionOrder()
    {
        // Arrange
        var service = new SubscriptionService();
        var url1 = "https://example1.com/feed";
        var url2 = "https://example2.com/feed";
        var url3 = "https://example3.com/feed";

        // Act
        service.AddSubscription(url1);
        var sub1Id = service.GetSubscriptions().First().Id;

        service.AddSubscription(url2);
        var subs = service.GetSubscriptions().ToList();
        var sub2Id = subs.First(s => s.Url == url2).Id;

        service.AddSubscription(url3);
        var allSubs = service.GetSubscriptions().ToList();

        // Assert
        Assert.Equal(3, allSubs.Count);
        Assert.True(sub1Id < sub2Id); // IDs should be sequential
        Assert.Equal(url1, allSubs[0].Url);
        Assert.Equal(url2, allSubs[1].Url);
        Assert.Equal(url3, allSubs[2].Url);
    }

    /// <summary>
    /// Test T025: AddSubscription with null or empty URL throws ArgumentException.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddSubscription_WithInvalidUrl_ThrowsArgumentException(string invalidUrl)
    {
        // Arrange
        var service = new SubscriptionService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.AddSubscription(invalidUrl));
        Assert.Contains("URL", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Test: GetSubscriptions always returns new IEnumerable (caller cannot modify internal list).
    /// </summary>
    [Fact]
    public void GetSubscriptions_ReturnsIndependentEnumerable()
    {
        // Arrange
        var service = new SubscriptionService();
        service.AddSubscription("https://example.com/feed");

        // Act
        var result1 = service.GetSubscriptions();
        var result2 = service.GetSubscriptions();

        // Assert - Results should be distinct objects
        Assert.NotSame(result1, result2);
    }
}
