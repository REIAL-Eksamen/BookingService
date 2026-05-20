namespace BookingService.DTOs;

public class CreateBookingDto
{
    public Guid UserId { get; set; }
    public Guid ClassSessionId { get; set; }
}