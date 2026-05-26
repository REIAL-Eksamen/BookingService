using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BookingService.Tests.Services;

[TestClass]
public class BookingServiceTests
{
    private Mock<IBookingRepository> _mockRepository = null!;
    private BookingService.Services.BookingService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _service = new BookingService.Services.BookingService(_mockRepository.Object);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WhenNoExistingBooking()
    {
        var dto = new CreateBookingDto { UserId = "u1", ClassSessionId = "c1" };
        _mockRepository.Setup(r => r.GetByUserId("u1")).Returns(new List<ClassBooking>());

        var result = _service.Create(dto);

        Assert.IsNotNull(result);
        Assert.AreEqual("u1", result.UserId);
        Assert.AreEqual(BookingStatus.Confirmed, result.Status);
    }

    [TestMethod]
    public void Create_ReturnsNull_WhenUserAlreadyBookedSameClass()
    {
        var dto = new CreateBookingDto { UserId = "u1", ClassSessionId = "c1" };
        var existing = new ClassBooking { UserId = "u1", ClassSessionId = "c1", Status = BookingStatus.Confirmed };
        _mockRepository.Setup(r => r.GetByUserId("u1")).Returns(new List<ClassBooking> { existing });

        var result = _service.Create(dto);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Create_ReturnsBooking_WhenPreviousBookingWasCancelled()
    {
        var dto = new CreateBookingDto { UserId = "u1", ClassSessionId = "c1" };
        var cancelled = new ClassBooking { UserId = "u1", ClassSessionId = "c1", Status = BookingStatus.Cancelled };
        _mockRepository.Setup(r => r.GetByUserId("u1")).Returns(new List<ClassBooking> { cancelled });

        var result = _service.Create(dto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Cancel_ReturnsTrue_WhenBookingExists()
    {
        _mockRepository.Setup(r => r.Cancel("b1", It.IsAny<DateTime>())).Returns(true);

        var result = _service.Cancel("b1");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Cancel_ReturnsFalse_WhenBookingDoesNotExist()
    {
        _mockRepository.Setup(r => r.Cancel("ghost", It.IsAny<DateTime>())).Returns(false);

        var result = _service.Cancel("ghost");

        Assert.IsFalse(result);
    }
}