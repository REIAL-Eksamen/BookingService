using BookingService.Clients;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;

namespace BookingService.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IClassServiceClient _classServiceClient;

    public BookingService(
        IBookingRepository repository,
        IClassServiceClient classServiceClient)
    {
        _repository = repository;
        _classServiceClient = classServiceClient;
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

    public async Task<ClassBooking?> CreateAsync(CreateBookingDto request)
    {
        var classInfo = await _classServiceClient.GetClassByIdAsync(request.ClassSessionId);

        if (classInfo is null)
        {
            return null;
        }

        if (classInfo.Status == ClassStatus.Cancelled || classInfo.Status == ClassStatus.Done)
        {
            return null;
        }

        if (classInfo.StartTime is not null && classInfo.StartTime <= DateTime.UtcNow)
        {
            return null;
        }

        var existingBookings = _repository.GetByUserId(request.UserId);

        var alreadyBooked = existingBookings.Any(booking =>
            booking.ClassSessionId == request.ClassSessionId &&
            booking.Status != BookingStatus.Cancelled);

        if (alreadyBooked)
        {
            return null;
        }

        if (classInfo.Classroom is not null)
        {
            var confirmedBookingsForClass = _repository.GetAll().Count(booking =>
                booking.ClassSessionId == request.ClassSessionId &&
                booking.Status == BookingStatus.Confirmed);

            if (confirmedBookingsForClass >= classInfo.Classroom.Capacity)
            {
                return null;
            }
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
        if (string.IsNullOrWhiteSpace(bookingId))
        {
            return false;
        }

        return _repository.Cancel(bookingId, DateTime.UtcNow);
    }
}