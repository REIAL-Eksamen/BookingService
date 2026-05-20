namespace BookingService.DTOs;

public class BookingResponseDto
{
    public Guid ClassBookingId { get; set; }
    public Guid UserId { get; set; }
    public Guid ClassSessionId { get; set; }
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Status { get; set; }
}