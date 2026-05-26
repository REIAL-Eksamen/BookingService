using BookingService.Repositories;
using BookingService.Clients;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using BookingService.Services;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }); 
builder.Services.AddOpenApi();

builder.Services.AddScoped<IBookingService, BookingService.Services.BookingService>();
builder.Services.AddSingleton<IBookingRepository, MongoBookingRepository>();

builder.Services.AddHttpClient<IClassServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ClassService:BaseUrl"]!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();