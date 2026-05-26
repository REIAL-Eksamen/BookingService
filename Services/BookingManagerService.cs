using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;

namespace BookingService.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<ClassBooking> GetAll()
    {
        return _repository.GetAll();
    }

    public ClassBooking? GetById(string bookingId)
    {
        return _repository.GetById(bookingId);
    }

    public IEnumerable<ClassBooking> GetByUserId(string userId)
    {
        return _repository.GetByUserId(userId);
    }

    public ClassBooking? Create(CreateBookingDto request)
    {
        var existingBookings = _repository.GetByUserId(request.UserId);

        var alreadyBooked = existingBookings.Any(booking =>
            booking.ClassSessionId == request.ClassSessionId &&
            booking.Status != BookingStatus.Cancelled);

        if (alreadyBooked)
        {
            return null;
        }

        var booking = new ClassBooking
        {
            UserId = request.UserId,
            ClassSessionId = request.ClassSessionId,
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        _repository.Add(booking);

        return booking;
    }

    public bool Cancel(string bookingId)
    {
        return _repository.Cancel(bookingId, DateTime.UtcNow);
    }
}