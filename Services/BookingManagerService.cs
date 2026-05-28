using BookingService.Clients;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;

namespace BookingService.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IClassServiceClient _classServiceClient;
    private readonly IUserServiceClient _userServiceClient;

    public BookingService(
        IBookingRepository repository,
        IClassServiceClient classServiceClient,
        IUserServiceClient userServiceClient)
    {
        _repository = repository;
        _classServiceClient = classServiceClient;
        _userServiceClient = userServiceClient;
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

    public async Task<ClassBooking?> CreateAsync(string userId, CreateBookingDto request)
    {
        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user is null)
            return null;

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

        var existingBookings = _repository.GetByUserId(userId);

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
            UserId = userId,
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