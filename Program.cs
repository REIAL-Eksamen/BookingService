using MassTransit;
using System.Text;
using BookingService.Repositories;
using BookingService.Clients;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using BookingService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddHttpClient<IClassServiceClient, ClassServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ClassService:BaseUrl"]!);
});

builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UserService:BaseUrl"]!);
});

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "";
var jwtSecret = builder.Configuration["Jwt:Key"] ?? "";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "FitLifeUsers";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingService.Consumers.ClassCancelledConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();