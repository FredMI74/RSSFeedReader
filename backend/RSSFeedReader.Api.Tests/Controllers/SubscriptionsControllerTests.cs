using RSSFeedReader.Api.Controllers;
using RSSFeedReader.Api.Models;
using RSSFeedReader.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace RSSFeedReader.Api.Tests.Controllers;

/// <summary>
/// Integration tests for SubscriptionsController.
/// </summary>
public class SubscriptionsControllerTests
{
    /// <summary>
    /// Helper to create a controller with a fresh service.
    /// </summary>
    private SubscriptionsController CreateController()
    {
        var service = new SubscriptionService();
        return new SubscriptionsController(service);
    }

    /// <summary>
    /// Test T024: GET /api/subscriptions returns empty list initially.
    /// </summary>
    [Fact]
    public void Get_OnFreshController_ReturnsOkWithEmptyList()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<Subscription>>(okResult.Value);
        Assert.Empty(returnedList);
    }

    /// <summary>
    /// Test T025: POST /api/subscriptions with valid URL returns OkWithList.
    /// </summary>
    [Fact]
    public void Post_WithValidUrl_ReturnsOkWithUpdatedList()
    {
        // Arrange
        var controller = CreateController();
        var request = new AddSubscriptionRequest { Url = "https://example.com/feed" };

        // Act
        var result = controller.Post(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<Subscription>>(okResult.Value);
        Assert.Single(returnedList);
    }

    /// <summary>
    /// Test: POST /api/subscriptions with null request returns BadRequest.
    /// </summary>
    [Fact]
    public void Post_WithNullRequest_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.Post(null!);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }

    /// <summary>
    /// Test: POST /api/subscriptions with empty URL returns BadRequest.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Post_WithEmptyUrl_ReturnsBadRequest(string emptyUrl)
    {
        // Arrange
        var controller = CreateController();
        var request = new AddSubscriptionRequest { Url = emptyUrl };

        // Act
        var result = controller.Post(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }

    /// <summary>
    /// Test: POST /api/subscriptions then GET returns the added subscription.
    /// </summary>
    [Fact]
    public void PostThenGet_ReturnsAddedSubscription()
    {
        // Arrange
        var controller = CreateController();
        var testUrl = "https://example.com/feed";
        var request = new AddSubscriptionRequest { Url = testUrl };

        // Act
        controller.Post(request);
        var getResult = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(getResult);
        var returnedList = Assert.IsType<List<Subscription>>(okResult.Value);
        Assert.Single(returnedList);
        Assert.Equal(testUrl, returnedList[0].Url);
    }

    /// <summary>
    /// Test: Multiple POST calls accumulate subscriptions.
    /// </summary>
    [Fact]
    public void Post_MultipleUrls_AccumulatesSubscriptions()
    {
        // Arrange
        var controller = CreateController();
        var urls = new[] { "https://example1.com/feed", "https://example2.com/feed", "https://example3.com/feed" };

        // Act
        foreach (var url in urls)
        {
            var request = new AddSubscriptionRequest { Url = url };
            controller.Post(request);
        }
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<Subscription>>(okResult.Value);
        Assert.Equal(3, returnedList.Count);
    }
}
