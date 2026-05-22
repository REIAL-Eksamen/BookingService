using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookingService.Models;

public class ClassBooking
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ClassBookingId { get; set; }

    public string UserId { get; set; } = "";
    public string ClassSessionId { get; set; } = "";
    public DateTime BookedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public BookingStatus Status { get; set; }

    public void Confirm() => Status = BookingStatus.Confirmed;

    public void Cancel(DateTime currentTime)
    {
        Status = BookingStatus.Cancelled;
        CancelledAt = currentTime;
    }

    public void MarkNoShow() => Status = BookingStatus.NoShow;

    public bool CanBeCancelled(DateTime currentTime) =>
        Status == BookingStatus.Confirmed && currentTime < BookedAt;
}

public enum BookingStatus
{
    Confirmed,
    Cancelled,
    NoShow,
    WaitListed
}