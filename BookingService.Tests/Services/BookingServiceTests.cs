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
        var dto = new CreateBookingDto
        {
            UserId = "u1",
            ClassSessionId = "c1"
        };

        _mockRepository
            .Setup(repository => repository.GetByUserId("u1"))
            .Returns(new List<ClassBooking>());

        var result = _service.Create(dto);

        Assert.IsNotNull(result);
        Assert.AreEqual("u1", result.UserId);
        Assert.AreEqual("c1", result.ClassSessionId);
        Assert.AreEqual(BookingStatus.Confirmed, result.Status);

        _mockRepository.Verify(
            repository => repository.Add(It.IsAny<ClassBooking>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_ReturnsNull_WhenUserAlreadyBookedSameClass()
    {
        var dto = new CreateBookingDto
        {
            UserId = "u1",
            ClassSessionId = "c1"
        };

        var existingBooking = new ClassBooking
        {
            ClassBookingId = "b1",
            UserId = "u1",
            ClassSessionId = "c1",
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        };

        _mockRepository
            .Setup(repository => repository.GetByUserId("u1"))
            .Returns(new List<ClassBooking> { existingBooking });

        var result = _service.Create(dto);

        Assert.IsNull(result);

        _mockRepository.Verify(
            repository => repository.Add(It.IsAny<ClassBooking>()),
            Times.Never);
    }

    [TestMethod]
    public void Cancel_ReturnsFalse_WhenBookingDoesNotExist()
    {
        _mockRepository
            .Setup(repository => repository.Cancel("ghost", It.IsAny<DateTime>()))
            .Returns(false);

        var result = _service.Cancel("ghost");

        Assert.IsFalse(result);
    }
}