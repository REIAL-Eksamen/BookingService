namespace BookingService.DTOs;

public class BookingResponseDto
{
    public string? ClassBookingId { get; set; }
    public string UserId { get; set; } = "";
    public string ClassSessionId { get; set; } = "";
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Status { get; set; }
}