using Xunit;

namespace MagicLights.Integration
{
    [CollectionDefinition("Application collection")]
    public class ApplicationCollection : ICollectionFixture<ApplicationFixture>
    {
    }
}
