using RSSFeedReader.Api.Models;
using RSSFeedReader.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace RSSFeedReader.Api.Controllers;

/// <summary>
/// API endpoints for managing RSS feed subscriptions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _service;

    public SubscriptionsController(SubscriptionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all feed subscriptions.
    /// </summary>
    /// <returns>List of all subscriptions</returns>
    [HttpGet]
    public IActionResult Get()
    {
        var subscriptions = _service.GetSubscriptions().ToList();
        return Ok(subscriptions);
    }

    /// <summary>
    /// Add a new feed subscription.
    /// </summary>
    /// <param name="request">Request containing the feed URL</param>
    /// <returns>Updated list of all subscriptions</returns>
    [HttpPost]
    public IActionResult Post([FromBody] AddSubscriptionRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("URL is required");

        try
        {
            _service.AddSubscription(request.Url);
            var subscriptions = _service.GetSubscriptions().ToList();
            return Ok(subscriptions);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for adding a new subscription.
/// </summary>
public class AddSubscriptionRequest
{
    /// <summary>
    /// The feed URL to subscribe to.
    /// </summary>
    public required string Url { get; set; }
}
