using Microsoft.EntityFrameworkCore;
using NeedsToTest.API.Data.Context;
using NeedsToTest.API.Data.Seed;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbConnectionString = builder.Configuration.GetConnectionString("Mongo");
var redisCon = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddDbContext<MyAmazonDbContext>(op => op.UseMongoDB(dbConnectionString,"Marketplace"));
builder.Services.AddSingleton<IDatabase>(op =>
{
    IConnectionMultiplexer con = ConnectionMultiplexer.Connect(redisCon);
    return con.GetDatabase();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<MyAmazonDbContext>();
            await context.Database.EnsureCreatedAsync();
            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TESTE")))
                context.Seed();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while seeding the database." + ex.Message);
            throw;
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
