namespace BookingService.Clients;

public interface IUserServiceClient
{
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> GetUserByAuthIdAsync(string authId);
}

public class UserDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
}

public enum MembershipStatus
{
    Active,
    Inactive
}
