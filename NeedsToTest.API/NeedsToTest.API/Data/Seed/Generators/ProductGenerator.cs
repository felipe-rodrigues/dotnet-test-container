using Bogus;
using NeedsToTest.API.Data.Entities;

namespace NeedsToTest.API.Data.Seed.Generators
{
    public class ProductGenerator : Faker<Product>
    {
        public ProductGenerator()
        {
            RuleFor(p => p.Id, f => f.Random.Guid().ToString());
            RuleFor(p => p.Name, f => f.Commerce.ProductName());
            RuleFor(p => p.Price, f => f.Random.Decimal(10, 10000));
            RuleFor(p => p.Categories, f => f.Commerce.Categories(3).ToList());
        }
    }
}
