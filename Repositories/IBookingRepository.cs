using BookingService.Models;

namespace BookingService.Repositories;

public interface IBookingRepository
{
    IEnumerable<ClassBooking> GetAll();
    ClassBooking? GetById(string bookingId);
    IEnumerable<ClassBooking> GetByUserId(string userId);
    void Add(ClassBooking booking);
    bool Cancel(string bookingId, DateTime cancelledAt);
}