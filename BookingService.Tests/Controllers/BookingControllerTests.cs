using BookingService.Clients;
using BookingService.Controllers;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BookingService.Tests.Controllers;

[TestClass]
public class BookingControllerTests
{
    private Mock<IBookingRepository> _mockRepository = null!;
    private Mock<ILogger<BookingController>> _mockLogger = null!;
    private BookingController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingController>>();
        var httpClient = new HttpClient();
        var classServiceClient = new ClassServiceClient(httpClient);

        _controller = new BookingController(
            _mockRepository.Object,
            _mockLogger.Object,
            classServiceClient
        );
    }

    [TestMethod]
    public void Get_ReturnsAllBookings()
    {
        var bookings = new List<ClassBooking>
        {
            new() { ClassBookingId = "b1", UserId = "u1", ClassSessionId = "c1", BookedAt = DateTime.UtcNow },
            new() { ClassBookingId = "b2", UserId = "u2", ClassSessionId = "c2", BookedAt = DateTime.UtcNow }
        };
        _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

        var result = _controller.Get().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetById_ReturnsOk_WhenBookingExists()
    {
        var booking = new ClassBooking { ClassBookingId = "b1", UserId = "u1", ClassSessionId = "c1", BookedAt = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetById("b1")).Returns(booking);

        var result = _controller.GetById("b1");

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var ok = result.Result as OkObjectResult;
        var returned = ok?.Value as ClassBooking;
        Assert.IsNotNull(returned);
        Assert.AreEqual("b1", returned.ClassBookingId);
    }

    [TestMethod]
    public void GetById_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        _mockRepository.Setup(r => r.GetById("missing")).Returns((ClassBooking?)null);

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
        _mockRepository.Setup(r => r.GetByUserId("u1")).Returns(bookings);

        var result = _controller.GetByUserId("u1");

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtRoute_WhenValid()
    {
        var dto = new CreateBookingDto { UserId = "u1", ClassSessionId = "c1" };
        _mockRepository.Setup(r => r.Add(It.IsAny<ClassBooking>()));

        var result = await _controller.Create(dto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));
        var created = result.Result as CreatedAtRouteResult;
        var booking = created?.Value as ClassBooking;
        Assert.IsNotNull(booking);
        Assert.AreEqual("u1", booking.UserId);
        Assert.AreEqual(BookingStatus.Confirmed, booking.Status);
    }

    [TestMethod]
    public async Task Create_CallsRepositoryAdd_Once()
    {
        var dto = new CreateBookingDto { UserId = "u1", ClassSessionId = "c1" };

        await _controller.Create(dto);

        _mockRepository.Verify(r => r.Add(It.IsAny<ClassBooking>()), Times.Once);
    }

    [TestMethod]
    public void Cancel_ReturnsNoContent_WhenSuccessful()
    {
        _mockRepository.Setup(r => r.Cancel("b1", It.IsAny<DateTime>())).Returns(true);

        var result = _controller.Cancel("b1");

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void Cancel_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        _mockRepository.Setup(r => r.Cancel("ghost", It.IsAny<DateTime>())).Returns(false);

        var result = _controller.Cancel("ghost");

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}