namespace BookingService.Clients;

public interface IClassServiceClient
{
    Task<ClassDto?> GetClassByIdAsync(string classId);
}