using System.Net.Http.Json;

namespace BookingService.Clients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"api/users/{userId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> GetUserByAuthIdAsync(string authId)
    {
        var response = await _httpClient.GetAsync($"api/users/by-auth/{authId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }
}
