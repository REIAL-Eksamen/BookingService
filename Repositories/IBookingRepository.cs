using BookingService.Models;

namespace BookingService.Repositories;

public interface IBookingRepository
{
    IEnumerable<ClassBooking> GetAll();
    ClassBooking? GetById(Guid bookingId);
    IEnumerable<ClassBooking> GetByUserId(Guid userId);
    void Add(ClassBooking booking);
    bool Cancel(Guid bookingId, DateTime cancelledAt);
}