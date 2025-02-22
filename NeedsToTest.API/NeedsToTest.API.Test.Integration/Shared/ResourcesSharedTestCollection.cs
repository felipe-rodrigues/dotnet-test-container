using NeedsToTest.API.Test.Integration.ApiFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedsToTest.API.Test.Integration.Shared
{
    [CollectionDefinition("API")]
    public class ResourcesSharedTestCollection : ICollectionFixture<TestCore>
    {
    }
}
