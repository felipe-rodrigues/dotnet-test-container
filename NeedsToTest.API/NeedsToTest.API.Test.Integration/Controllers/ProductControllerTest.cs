using FluentAssertions;
using NeedsToTest.API.Data.Entities;
using NeedsToTest.API.Test.Integration.ApiFactory;
using NeedsToTest.API.Test.Integration.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedsToTest.API.Test.Integration.Controllers
{
    [Collection("API")]
    public class ProductControllerTest : IAsyncLifetime
    {

        private readonly TestCore _core;

        public ProductControllerTest(TestCore core)
        {
            _core = core;
        }

        public async Task InitializeAsync()
        {
           
        }

        public async Task DisposeAsync()
        {
            await _core.DeleteEntities<Product>();
        }

        [Fact]
        public async Task Get_WithProductsInDatabase_ReturnListOfProducts()
        {

            //Arrange
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Teste",
                Price = 10,
                Categories = new List<string> { "Teste" }
            };

            await _core.InsertEntityDatabase(product);

            //Act
            var response = await _core.Client.GetAsync("/api/Product");


            //Assert
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<List<Product>>();
            Assert.NotEmpty(products);
            products.First().Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task Post_WhenValidRequest_ReturnsCreatedProduct()
        {
            //Arrange
            var product = new Product
            {
                Id = "",
                Name = "Teste",
                Price = 10,
                Categories = new List<string> { "Teste" }
            };

            //Act
            var response = await _core.Client.PostAsJsonAsync("/api/Product", product);

            //Assert
            response.EnsureSuccessStatusCode();
            var productResponse = await response.Content.ReadAsAsync<Product>();
            productResponse.Name.Should().Be(product.Name);
            productResponse.Price.Should().Be(product.Price);
            productResponse.Categories.Should().BeEquivalentTo(product.Categories);
            productResponse.Id.Should().NotBeNullOrEmpty();

            var responseGet = await _core.Client.GetAsync($"/api/Product/{productResponse.Id}");
            responseGet.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_WhenSearchingByCategory_SaveDataToRedis()
        {
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Teste",
                Price = 10,
                Categories = new List<string> { "Teste" }
            };

            await _core.InsertEntityDatabase(product);

            //Act
            var response = await _core.Client.GetAsync("/api/Product?category=Teste");


            response.EnsureSuccessStatusCode();

            var cache = await _core.GetListFromCache<Product>("category:Teste");
            cache.Should().NotBeNull();


        }

       
    }
}
