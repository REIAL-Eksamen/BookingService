using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingRepository bookingRepository, ILogger<BookingController> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetBookings")]
    public IEnumerable<ClassBooking> Get()
    {
        return _bookingRepository.GetAll();
    }

    [HttpGet("{bookingId}", Name = "GetBookingById")]
    public ActionResult<ClassBooking> GetById(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);

        if (booking is null)
        {
            return NotFound();
        }

        return Ok(booking);
    }

    [HttpGet("user/{userId}", Name = "GetBookingsByUser")]
    public ActionResult<IEnumerable<ClassBooking>> GetByUserId(Guid userId)
    {
        var bookings = _bookingRepository.GetByUserId(userId);
        return Ok(bookings);
    }

    [HttpPost(Name = "CreateBooking")]
    public ActionResult<ClassBooking> Create([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var booking = new ClassBooking
        {
            ClassBookingId = Guid.NewGuid(),
            UserId = dto.UserId,
            ClassSessionId = dto.ClassSessionId,
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        _bookingRepository.Add(booking);

        return CreatedAtRoute("GetBookingById", new { bookingId = booking.ClassBookingId }, booking);
    }

    [HttpPut("{bookingId}/cancel", Name = "CancelBooking")]
    public IActionResult Cancel(Guid bookingId)
    {
        var cancelled = _bookingRepository.Cancel(bookingId, DateTime.UtcNow);

        if (!cancelled)
        {
            return NotFound();
        }

        return NoContent();
    }
}