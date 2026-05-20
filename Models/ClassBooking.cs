namespace BookingService.Models;

public class ClassBooking
{
    public Guid ClassBookingId { get; set; }
    public Guid UserId { get; set; }
    public Guid ClassSessionId { get; set; }
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public BookingStatus Status { get; set; }

    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
    }

    public void Cancel(DateTime currentTime)
    {
        Status = BookingStatus.Cancelled;
        CancelledAt = currentTime;
    }

    public void MarkNoShow()
    {
        Status = BookingStatus.NoShow;
    }

    public bool CanBeCancelled(DateTime currentTime)
    {
        return Status == BookingStatus.Confirmed && currentTime < BookedAt;
    }
}

public enum BookingStatus
{
    Confirmed,
    Cancelled,
    NoShow,
    WaitListed
}