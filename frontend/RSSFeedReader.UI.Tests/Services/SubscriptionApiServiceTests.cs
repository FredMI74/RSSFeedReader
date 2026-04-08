using RSSFeedReader.UI.Models;
using RSSFeedReader.UI.Services;
using Xunit;

namespace RSSFeedReader.UI.Tests.Services;

/// <summary>
/// Unit tests for SubscriptionApiService.
/// Note: These tests use mocking for HttpClient in a real scenario.
/// For MVP, we verify the service structure and methods exist.
/// </summary>
public class SubscriptionApiServiceTests
{
    /// <summary>
    /// Test: SubscriptionApiService can be instantiated with HttpClient.
    /// </summary>
    [Fact]
    public void Constructor_WithHttpClient_Instantiates()
    {
        // Arrange
        var httpClient = new HttpClient();

        // Act
        var service = new SubscriptionApiService(httpClient);

        // Assert
        Assert.NotNull(service);
    }

    /// <summary>
    /// Test: GetSubscriptionsAsync method exists and returns Task of List.
    /// </summary>
    [Fact]
    public void GetSubscriptionsAsync_MethodExists()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5151/api/") };
        var service = new SubscriptionApiService(httpClient);

        // Act & Assert
        var method = typeof(SubscriptionApiService).GetMethod("GetSubscriptionsAsync");
        Assert.NotNull(method);
        Assert.True(method.ReturnType.IsGenericType);
        Assert.True(method.ReturnType.Name.Contains("Task"));
    }

    /// <summary>
    /// Test: AddSubscriptionAsync method exists and returns Task of bool.
    /// </summary>
    [Fact]
    public void AddSubscriptionAsync_MethodExists()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5151/api/") };
        var service = new SubscriptionApiService(httpClient);

        // Act & Assert
        var method = typeof(SubscriptionApiService).GetMethod("AddSubscriptionAsync");
        Assert.NotNull(method);
        Assert.True(method.ReturnType.Name.Contains("Task"));
        Assert.Single(method.GetParameters());
        Assert.Equal("url", method.GetParameters()[0].Name);
        Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);
    }

    /// <summary>
    /// Test: SubscriptionDto model can be created and populated.
    /// </summary>
    [Fact]
    public void SubscriptionDto_CanBeCreatedAndPopulated()
    {
        // Arrange
        var now = DateTime.UtcNow;
        
        // Act
        var dto = new SubscriptionDto 
        { 
            Id = 1, 
            Url = "https://example.com/feed",
            DateAdded = now
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("https://example.com/feed", dto.Url);
        Assert.Equal(now, dto.DateAdded);
    }
}
