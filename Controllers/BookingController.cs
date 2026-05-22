using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Clients;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingController> _logger;
    private readonly ClassServiceClient _classServiceClient;

    public BookingController(IBookingRepository bookingRepository, ILogger<BookingController> logger, ClassServiceClient classServiceClient)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
        _classServiceClient = classServiceClient;
    }

    [HttpGet(Name = "GetBookings")]
    public IEnumerable<ClassBooking> Get()
    {
        return _bookingRepository.GetAll();
    }

    [HttpGet("{bookingId}", Name = "GetBookingById")]
    public ActionResult<ClassBooking> GetById(string bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);

        if (booking is null)
        {
            return NotFound();
        }

        return Ok(booking);
    }

    [HttpGet("user/{userId}", Name = "GetBookingsByUser")]
    public ActionResult<IEnumerable<ClassBooking>> GetByUserId(string userId)
    {
        var bookings = _bookingRepository.GetByUserId(userId);
        return Ok(bookings);
    }

    [HttpPost(Name = "CreateBooking")]
    public async Task<ActionResult<ClassBooking>> Create([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Midlertidigt kommenteret ud til ClassService er klar
        // var classExists = await _classServiceClient.GetClassByIdAsync(dto.ClassSessionId);
        // if (classExists is null)
        // {
        //     return NotFound($"Klassen {dto.ClassSessionId} findes ikke.");
        // }

        var booking = new ClassBooking
        {
            UserId = dto.UserId,
            ClassSessionId = dto.ClassSessionId,
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        _bookingRepository.Add(booking);

        return CreatedAtRoute("GetBookingById", new { bookingId = booking.ClassBookingId }, booking);
    }

    [HttpPut("{bookingId}/cancel", Name = "CancelBooking")]
    public IActionResult Cancel(string bookingId)
    {
        var cancelled = _bookingRepository.Cancel(bookingId, DateTime.UtcNow);

        if (!cancelled)
        {
            return NotFound();
        }

        return NoContent();
    }
}