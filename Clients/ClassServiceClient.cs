using System.Net.Http.Json;

namespace BookingService.Clients;

public class ClassServiceClient
{
    private readonly HttpClient _httpClient;

    public ClassServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClassDto?> GetClassByIdAsync(string classId)
    {
        var response = await _httpClient.GetAsync($"api/class/{classId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ClassDto>();
    }
}

public class ClassDto
{
    public string? Id { get; set; }
    public string ClassName { get; set; } = "";
    public string ClassDescription { get; set; } = "";
    public string ClassType { get; set; } = "";
    public string InstructorId { get; set; } = "";
    public string CenterId { get; set; } = "";
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? ClassCapacity { get; set; }
    public string Status { get; set; } = "";
}