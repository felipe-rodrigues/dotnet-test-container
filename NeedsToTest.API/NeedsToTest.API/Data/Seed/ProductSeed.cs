using NeedsToTest.API.Data.Context;
using NeedsToTest.API.Data.Seed.Generators;

namespace NeedsToTest.API.Data.Seed
{
    public static class ProductSeed
    {
        public static void Seed(MyAmazonDbContext context)
        {
            if(context.Products.Count() == 0)
            {
                var productGenerator = new ProductGenerator();
                var products = productGenerator.Generate(5000);
                context.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
