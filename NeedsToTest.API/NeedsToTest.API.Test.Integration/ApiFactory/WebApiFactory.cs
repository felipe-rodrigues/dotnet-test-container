using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using NeedsToTest.API.Data.Context;

namespace NeedsToTest.API.Test.Integration.ApiFactory
{
    public class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {

        public readonly MongoDbContainer _database = new MongoDbBuilder()
          .WithImage("mongo:6.0")
          .Build();

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureTestServices((services) =>
            {
                services.RemoveAll(typeof(DbContextOptions));
                services.RemoveAll(typeof(MyAmazonDbContext));

                var connection = _database.GetConnectionString();

                var options = new DbContextOptionsBuilder<MyAmazonDbContext>()
                                   .UseMongoDB(connection, databaseName: "trackorderstest")
                                   .Options;

                services.AddSingleton(sp => options);
                services.AddDbContext<MyAmazonDbContext>(op =>
                {
                    op.UseMongoDB(connection, databaseName: "MarketplaceTest");
                });

            });
        }

        public HttpClient HttpClient { get; private set; }

        public async Task InitializeAsync()
        {
            await _database.StartAsync();
            HttpClient = CreateClient();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _database.StopAsync();
        }
    }
}
