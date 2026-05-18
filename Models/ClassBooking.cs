namespace BookingService.Models;

public class ClassBooking
{
    public Guid HoldBookingId { get; set; }
    public Guid UserId { get; set; }
    public Guid HoldSessionId { get; set; }
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; } //kan være null
    public BookingStatus Status { get; set; } 
}

// BookingStatus definerer de mulige tilstande en booking kan have.
public enum BookingStatus
{
    Confirmed,
    Cancelled,
    Waitlisted,
    NoShow
}