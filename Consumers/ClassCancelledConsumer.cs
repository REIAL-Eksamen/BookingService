using FitLife.Events;
using BookingService.Models;
using BookingService.Services;
using MassTransit;

namespace BookingService.Consumers;

// Når et hold aflyses, skal vi sørge for at alle bookinger på holdet også bliver aflyst.
// Denne consumer lytter på ClassCancelledEvent og håndterer oprydningen automatisk.
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

      
        // finder kun de bookinger der stadig er aktive – rører ikke ved
        // dem der allerede er annulleret.
        var bookings = _bookingService.GetAll()
            .Where(b => b.ClassSessionId == classId && b.Status == BookingStatus.Confirmed)
            .ToList();
        // Annuller dem én efter én. Booking-servicen håndterer selv notifikationer
        foreach (var booking in bookings)
            _bookingService.Cancel(booking.ClassBookingId!);

        return Task.CompletedTask;
    }
}
