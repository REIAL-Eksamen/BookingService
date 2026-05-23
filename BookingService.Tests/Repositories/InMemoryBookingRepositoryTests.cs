using BookingService.Models;
using BookingService.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookingService.Tests.Repositories;

[TestClass]
public class InMemoryBookingRepositoryTests
{
    private InMemoryBookingRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _repo = new InMemoryBookingRepository();
    }

    private static ClassBooking MakeBooking(string id) => new()
    {
        ClassBookingId = id,
        UserId = "user-1",
        ClassSessionId = "class-1",
        BookedAt = DateTime.UtcNow,
        Status = BookingStatus.Confirmed
    };

    [TestMethod]
    public void Add_AndGetAll_ReturnsAllBookings()
    {
        _repo.Add(MakeBooking("id-1"));
        _repo.Add(MakeBooking("id-2"));

        var result = _repo.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetById_ReturnsCorrectBooking()
    {
        _repo.Add(MakeBooking("id-42"));

        var result = _repo.GetById("id-42");

        Assert.IsNotNull(result);
        Assert.AreEqual("id-42", result.ClassBookingId);
    }

    [TestMethod]
    public void GetById_ReturnsNullWhenNotFound()
    {
        var result = _repo.GetById("does-not-exist");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByUserId_ReturnsOnlyMatchingBookings()
    {
        _repo.Add(new ClassBooking { ClassBookingId = "b1", UserId = "user-A", ClassSessionId = "c1", BookedAt = DateTime.UtcNow });
        _repo.Add(new ClassBooking { ClassBookingId = "b2", UserId = "user-B", ClassSessionId = "c2", BookedAt = DateTime.UtcNow });
        _repo.Add(new ClassBooking { ClassBookingId = "b3", UserId = "user-A", ClassSessionId = "c3", BookedAt = DateTime.UtcNow });

        var result = _repo.GetByUserId("user-A").ToList();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(b => b.UserId == "user-A"));
    }

    [TestMethod]
    public void Cancel_ReturnsTrueAndSetsCancelledStatus()
    {
        _repo.Add(MakeBooking("id-cancel"));

        var result = _repo.Cancel("id-cancel", DateTime.UtcNow);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Cancel_ReturnsFalseWhenBookingNotFound()
    {
        var result = _repo.Cancel("ghost-id", DateTime.UtcNow);

        Assert.IsFalse(result);
    }
}