using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Services;

public interface IBookingService
{
    IEnumerable<ClassBooking> GetAll();

    ClassBooking? GetById(string bookingId);

    IEnumerable<ClassBooking> GetByUserId(string userId);

    Task<ClassBooking?> CreateAsync(string userId, CreateBookingDto request);

    bool Cancel(string bookingId);
}