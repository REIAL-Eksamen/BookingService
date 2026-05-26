using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Services;

public interface IBookingService
{
    IEnumerable<ClassBooking> GetAll();

    ClassBooking? GetById(string bookingId);

    IEnumerable<ClassBooking> GetByUserId(string userId);

    Task<ClassBooking?> CreateAsync(CreateBookingDto request);

    bool Cancel(string bookingId);
}