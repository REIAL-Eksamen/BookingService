using BookingService.Clients;
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
    private Mock<IClassServiceClient> _mockClassServiceClient = null!;
    private BookingService.Services.BookingService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockClassServiceClient = new Mock<IClassServiceClient>();

        _service = new BookingService.Services.BookingService(
            _mockRepository.Object,
            _mockClassServiceClient.Object
        );
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsBooking_WhenNoExistingBooking()
    {
        var dto = new CreateBookingDto
        {
            UserId = "u1",
            ClassSessionId = "c1"
        };

        _mockClassServiceClient
            .Setup(client => client.GetClassByIdAsync("c1"))
            .ReturnsAsync(new ClassDto
            {
                Id = "c1",
                ClassName = "Yoga",
                StartTime = DateTime.UtcNow.AddDays(1),
                Status = ClassStatus.Scheduled,
                Classroom = new ClassroomDto
                {
                    ClassroomId = "room1",
                    ClassroomName = "Sal 1",
                    Capacity = 20
                }
            });

        _mockRepository
            .Setup(repository => repository.GetByUserId("u1"))
            .Returns(new List<ClassBooking>());

        _mockRepository
            .Setup(repository => repository.GetAll())
            .Returns(new List<ClassBooking>());

        var result = await _service.CreateAsync(dto);

        Assert.IsNotNull(result);
        Assert.AreEqual("u1", result.UserId);
        Assert.AreEqual("c1", result.ClassSessionId);
        Assert.AreEqual(BookingStatus.Confirmed, result.Status);

        _mockRepository.Verify(
            repository => repository.Add(It.IsAny<ClassBooking>()),
            Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsNull_WhenUserAlreadyBookedSameClass()
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

        _mockClassServiceClient
            .Setup(client => client.GetClassByIdAsync("c1"))
            .ReturnsAsync(new ClassDto
            {
                Id = "c1",
                ClassName = "Yoga",
                StartTime = DateTime.UtcNow.AddDays(1),
                Status = ClassStatus.Scheduled,
                Classroom = new ClassroomDto
                {
                    ClassroomId = "room1",
                    ClassroomName = "Sal 1",
                    Capacity = 20
                }
            });

        _mockRepository
            .Setup(repository => repository.GetByUserId("u1"))
            .Returns(new List<ClassBooking> { existingBooking });

        var result = await _service.CreateAsync(dto);

        Assert.IsNull(result);

        _mockRepository.Verify(
            repository => repository.Add(It.IsAny<ClassBooking>()),
            Times.Never);
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsNull_WhenClassDoesNotExist()
    {
        var dto = new CreateBookingDto
        {
            UserId = "u1",
            ClassSessionId = "missing-class"
        };

        _mockClassServiceClient
            .Setup(client => client.GetClassByIdAsync("missing-class"))
            .ReturnsAsync((ClassDto?)null);

        var result = await _service.CreateAsync(dto);

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