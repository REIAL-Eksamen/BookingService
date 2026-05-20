using BookingService.Models;

namespace BookingService.Repositories;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<ClassBooking> _bookings = [];

    public IEnumerable<ClassBooking> GetAll() => _bookings;

    public ClassBooking? GetById(Guid bookingId) =>
        _bookings.FirstOrDefault(b => b.ClassBookingId == bookingId);

    public IEnumerable<ClassBooking> GetByUserId(Guid userId) =>
        _bookings.Where(b => b.UserId == userId);

    public void Add(ClassBooking booking) => _bookings.Add(booking);

    public bool Cancel(Guid bookingId, DateTime cancelledAt)
    {
        var booking = GetById(bookingId);
        if (booking is null) return false;

        booking.Cancel(cancelledAt);
        return true;
    }
}