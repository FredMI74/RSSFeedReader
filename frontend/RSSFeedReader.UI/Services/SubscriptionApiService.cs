using RSSFeedReader.UI.Models;
using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

/// <summary>
/// Service for communicating with the subscription API backend.
/// </summary>
public class SubscriptionApiService
{
    private readonly HttpClient _httpClient;

    public SubscriptionApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get all subscriptions from the backend API.
    /// </summary>
    /// <returns>List of subscriptions or empty list on error</returns>
    public async Task<List<SubscriptionDto>> GetSubscriptionsAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<SubscriptionDto>>("subscriptions");
            return result ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching subscriptions: {ex.Message}");
            return new();
        }
    }

    /// <summary>
    /// Add a new subscription to the backend API.
    /// </summary>
    /// <param name="url">Feed URL to subscribe to</param>
    /// <returns>True if successful, false on error</returns>
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
