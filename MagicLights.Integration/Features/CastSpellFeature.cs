using LightsApi;
using System.Threading;
using Xunit;

namespace MagicLights.Integration.Features
{
    public class CastSpellFeature : BaseFeature
    {
        public CastSpellFeature(ApplicationFixture application)
            : base(application)
        {
        }

        [Fact]
        public void WhenRun()
        {
            application.Start();
            application.WriteContents(@"SimpleSpell.txt");
            application.WaitForGameEnd();

            //TODO: this is a big bag of nope
            //Need to inject the delayable of 0
            Thread.Sleep(500);

            Assert.Equal(
                RGB.White,
                application.LightClient.LastColors?[0]);
        }
    }
}
