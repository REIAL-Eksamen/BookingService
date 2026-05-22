using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using BookingService.Models;

namespace BookingService.Repositories;

public class MongoBookingRepository : IBookingRepository
{
    private readonly IMongoCollection<ClassBooking> _bookings;

    public MongoBookingRepository(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];
        var collectionName = configuration["MongoDB:CollectionName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _bookings = database.GetCollection<ClassBooking>(collectionName);
    }

    public IEnumerable<ClassBooking> GetAll() =>
        _bookings.Find(_ => true).ToList();

    public ClassBooking? GetById(string bookingId) =>
        _bookings.Find(b => b.ClassBookingId == bookingId).FirstOrDefault();

    public IEnumerable<ClassBooking> GetByUserId(string userId) =>
        _bookings.Find(b => b.UserId == userId).ToList();

    public void Add(ClassBooking booking) =>
        _bookings.InsertOne(booking);

    public bool Cancel(string bookingId, DateTime cancelledAt)
    {
        var booking = GetById(bookingId);
        if (booking is null) return false;

        booking.Cancel(cancelledAt);
        var result = _bookings.ReplaceOne(b => b.ClassBookingId == bookingId, booking);
        return result.ModifiedCount > 0;
    }
}