using System.Net.Http.Json;

namespace BookingService.Clients;

//kalder vores classservice microservice 

public class ClassServiceClient : IClassServiceClient
{
    private readonly HttpClient _httpClient;

    public ClassServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClassDto?> GetClassByIdAsync(string classId)
    {
        var response = await _httpClient.GetAsync($"api/class/{classId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

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
    public ClassroomDto? Classroom { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ClassStatus Status { get; set; }
}

public class ClassroomDto
{
    public string ClassroomId { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public int Capacity { get; set; }
}

public enum ClassStatus
{
    Scheduled,
    Active,
    Cancelled,
    Done
}