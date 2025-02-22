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
        public async Task Get_QuandoDadosNaBase_RetornaListaDeProdutos()
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

       
    }
}
