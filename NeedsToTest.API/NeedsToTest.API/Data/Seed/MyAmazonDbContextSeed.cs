using NeedsToTest.API.Data.Context;

namespace NeedsToTest.API.Data.Seed
{
    public static class MyAmazonDbContextSeed
    {
        public static void Seed(this MyAmazonDbContext context)
        {
            ProductSeed.Seed(context);
        }
    }
}
