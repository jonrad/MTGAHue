using Xunit;

namespace MagicLights.Integration.Features
{
    [Collection("Application collection")]
    public class BaseFeature
    {
        protected readonly ApplicationFixture application;

        public BaseFeature(ApplicationFixture application)
        {
            this.application = application;
        }
    }
}
