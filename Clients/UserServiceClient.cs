using System.Net.Http.Json;

namespace BookingService.Clients;

// Snakker med UserService over HTTP og henter brugeroplysninger vi har brug for
// i booking-flows. Vi returnerer null fremfor at kaste exceptions, så kalderen
// selv kan bestemme hvordan de vil håndtere manglende brugere.
public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Bruges når vi allerede kender brugerens interne ID, fx når en bruger
    // slår sine egne bookinger op. Her ved vi præcis hvem vi leder efter.
    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"api/users/{userId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    //bruges når vi kun har authId fra jwt-token fx ved oprettelse af ny booking. 
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
