using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet(Name = "GetBookings")]
    public IEnumerable<ClassBooking> Get()
    {
        return _bookingService.GetAll();
    }

    [HttpGet("{bookingId}", Name = "GetBookingById")]
    public ActionResult<ClassBooking> GetById(string bookingId)
    {
        var booking = _bookingService.GetById(bookingId);

        if (booking is null)
        {
            return NotFound();
        }

        return Ok(booking);
    }

    [HttpGet("user/{userId}", Name = "GetBookingsByUser")]
    public ActionResult<IEnumerable<ClassBooking>> GetByUserId(string userId)
    {
        var bookings = _bookingService.GetByUserId(userId);

        return Ok(bookings);
    }

    [HttpPost(Name = "CreateBooking")]
    public async Task<ActionResult<ClassBooking>> Create([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var booking = await _bookingService.CreateAsync(dto);

        if (booking is null)
        {
            return Conflict("Booking could not be created. Class may not exist, may be full, cancelled, done, already started, or already booked by the user.");
        }

        return CreatedAtRoute("GetBookingById", new { bookingId = booking.ClassBookingId }, booking);
    }

    [HttpPut("{bookingId}/cancel", Name = "CancelBooking")]
    public IActionResult Cancel(string bookingId)
    {
        var cancelled = _bookingService.Cancel(bookingId);

        if (!cancelled)
        {
            return NotFound();
        }

        return NoContent();
    }
}