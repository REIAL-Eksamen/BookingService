using BookingService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookingService.Tests.Models;

[TestClass]
public class ClassBookingTests
{
    [TestMethod]
    public void Confirm_SetsStatusToConfirmed()
    {
        var booking = new ClassBooking { Status = BookingStatus.WaitListed };

        booking.Confirm();

        Assert.AreEqual(BookingStatus.Confirmed, booking.Status);
    }

    [TestMethod]
    public void Cancel_SetsStatusToCancelledAndCancelledAt()
    {
        var booking = new ClassBooking { Status = BookingStatus.Confirmed };
        var cancelTime = new DateTime(2025, 1, 1, 12, 0, 0);

        booking.Cancel(cancelTime);

        Assert.AreEqual(BookingStatus.Cancelled, booking.Status);
        Assert.AreEqual(cancelTime, booking.CancelledAt);
    }

    [TestMethod]
    public void MarkNoShow_SetsStatusToNoShow()
    {
        var booking = new ClassBooking { Status = BookingStatus.Confirmed };

        booking.MarkNoShow();

        Assert.AreEqual(BookingStatus.NoShow, booking.Status);
    }

    [TestMethod]
    public void CanBeCancelled_ReturnsTrueWhenConfirmedAndBeforeBookedAt()
    {
        var booking = new ClassBooking
        {
            Status = BookingStatus.Confirmed,
            BookedAt = DateTime.UtcNow.AddHours(2)
        };

        var result = booking.CanBeCancelled(DateTime.UtcNow);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanBeCancelled_ReturnsFalseWhenAlreadyCancelled()
    {
        var booking = new ClassBooking
        {
            Status = BookingStatus.Cancelled,
            BookedAt = DateTime.UtcNow.AddHours(2)
        };

        var result = booking.CanBeCancelled(DateTime.UtcNow);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CanBeCancelled_ReturnsFalseWhenCurrentTimeIsAfterBookedAt()
    {
        var booking = new ClassBooking
        {
            Status = BookingStatus.Confirmed,
            BookedAt = DateTime.UtcNow.AddHours(-1)
        };

        var result = booking.CanBeCancelled(DateTime.UtcNow);

        Assert.IsFalse(result);
    }
}