using BookingService.Controllers;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BookingService.Tests.Controllers;

[TestClass]
public class BookingControllerTests
{
    private Mock<IBookingService> _mockService = null!;
    private Mock<ILogger<BookingController>> _mockLogger = null!;
    private BookingController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IBookingService>();
        _mockLogger = new Mock<ILogger<BookingController>>();

        _controller = new BookingController(
            _mockService.Object,
            _mockLogger.Object
        );
    }

    [TestMethod]
    public void GetById_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        _mockService.Setup(s => s.GetById("missing")).Returns((ClassBooking?)null);

        var result = _controller.GetById("missing");

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void GetByUserId_ReturnsOk_WithBookings()
    {
        var bookings = new List<ClassBooking>
        {
            new() { ClassBookingId = "b1", UserId = "u1", ClassSessionId = "c1", BookedAt = DateTime.UtcNow }
        };

        _mockService.Setup(s => s.GetByUserId("u1")).Returns(bookings);

        var result = _controller.GetByUserId("u1");

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtRoute_WhenValid()
    {
        var dto = new CreateBookingDto
        {
            UserId = "u1",
            ClassSessionId = "c1"
        };

        var booking = new ClassBooking
        {
            ClassBookingId = "b1",
            UserId = "u1",
            ClassSessionId = "c1",
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        _mockService
            .Setup(s => s.CreateAsync(dto))
            .ReturnsAsync(booking);

        var result = await _controller.Create(dto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));

        var created = result.Result as CreatedAtRouteResult;
        var returned = created?.Value as ClassBooking;

        Assert.IsNotNull(returned);
        Assert.AreEqual("u1", returned.UserId);
        Assert.AreEqual(BookingStatus.Confirmed, returned.Status);
    }

    [TestMethod]
    public async Task Create_ReturnsConflict_WhenUserAlreadyBookedSameClass()
    {
        var dto = new CreateBookingDto
        {
            UserId = "u1",
            ClassSessionId = "c1"
        };

        _mockService
            .Setup(s => s.CreateAsync(dto))
            .ReturnsAsync((ClassBooking?)null);

        var result = await _controller.Create(dto);

        Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
    }

    [TestMethod]
    public void Cancel_ReturnsNoContent_WhenSuccessful()
    {
        _mockService.Setup(s => s.Cancel("b1")).Returns(true);

        var result = _controller.Cancel("b1");

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void Cancel_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        _mockService.Setup(s => s.Cancel("ghost")).Returns(false);

        var result = _controller.Cancel("ghost");

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}