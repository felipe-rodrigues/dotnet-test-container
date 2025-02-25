using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using NeedsToTest.API.Data.Context;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using StackExchange.Redis;

namespace NeedsToTest.API.Test.Integration.ApiFactory
{
    public class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {

        public readonly MongoDbContainer _database = new MongoDbBuilder()
          .WithImage("mongo:6.0")
          .Build();

        private readonly IContainer _redisContainer = new ContainerBuilder()
            .WithImage("redis:7.0")
            .WithPortBinding(6379, true)
            .WithExposedPort(6379)
            .Build();

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureTestServices((services) =>
            {
                services.RemoveAll(typeof(DbContextOptions));
                services.RemoveAll(typeof(MyAmazonDbContext));

                services.RemoveAll(typeof(IDatabase));

                var connection = _database.GetConnectionString();
                services.AddDbContext<MyAmazonDbContext>(op =>
                {
                    op.UseMongoDB(connection, databaseName: "MarketplaceTest");
                });

                var redisConnection = $"{_redisContainer.Hostname}:{_redisContainer.GetMappedPublicPort(6379)}";
                services.AddSingleton<IDatabase>(cfg =>
                {
                    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnection);
                    return multiplexer.GetDatabase();
                });

            });
        }

        public HttpClient HttpClient { get; private set; }

        public async Task InitializeAsync()
        {
            await _redisContainer.StartAsync();
            await _database.StartAsync();
            HttpClient = CreateClient();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _redisContainer.StopAsync();
            await _database.StopAsync();
        }
    }
}
