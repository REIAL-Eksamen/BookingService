using FitLife.Events;
using BookingService.Models;
using BookingService.Services;
using MassTransit;

namespace BookingService.Consumers;

public class ClassCancelledConsumer : IConsumer<ClassCancelledEvent>
{
    private readonly IBookingService _bookingService;

    public ClassCancelledConsumer(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public Task Consume(ConsumeContext<ClassCancelledEvent> context)
    {
        var classId = context.Message.ClassId;

        var bookings = _bookingService.GetAll()
            .Where(b => b.ClassSessionId == classId && b.Status == BookingStatus.Confirmed)
            .ToList();

        foreach (var booking in bookings)
            _bookingService.Cancel(booking.ClassBookingId!);

        return Task.CompletedTask;
    }
}
