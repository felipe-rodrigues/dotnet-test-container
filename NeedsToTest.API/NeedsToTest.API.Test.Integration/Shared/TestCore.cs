using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NeedsToTest.API.Data.Context;
using NeedsToTest.API.Test.Integration.ApiFactory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NeedsToTest.API.Test.Integration.Shared
{
    public class TestCore : IAsyncLifetime
    {
        private static WebApiFactory _factory = null!;
        private static IServiceScopeFactory _scopeFactory = null;
        public HttpClient Client { get; set; }

        public async Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("TESTE", "true");
            _factory = new WebApiFactory();
            await _factory.InitializeAsync();
            Client = _factory.HttpClient;
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public async Task DisposeAsync()
        {
            Environment.SetEnvironmentVariable("TESTE", null);
            await _factory.DisposeAsync();
        }

        #region HelperMethods   

        public async Task InsertEntityDatabase<T>(T request) where T: class
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyAmazonDbContext>();
            var dbSet = db.Set<T>();
            dbSet.Add(request);
            await db.SaveChangesAsync();
        }

        public async Task InsertRangeEntityDatabase<T>(List<T> request) where T : class
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyAmazonDbContext>();
            var dbSet = db.Set<T>();
            dbSet.AddRange(request);
            await db.SaveChangesAsync();
        }

        public async Task DeleteEntities<T>() where T : class
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyAmazonDbContext>();
            var dbSet = db.Set<T>();
            var allRecords = await dbSet.ToListAsync();
            dbSet.RemoveRange(allRecords);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetListFromCache<T>(string key) where T : class
        {
            using var scope = _scopeFactory.CreateScope();
            var redis = scope.ServiceProvider.GetRequiredService<IDatabase>();
            var cacheData = await redis.StringGetAsync(key);
            var data = JsonSerializer.Deserialize<IEnumerable<T>>(cacheData);
            return data;
        }

        #endregion
    }
}
