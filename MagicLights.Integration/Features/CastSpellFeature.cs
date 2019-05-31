using System;
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
            application.WriteContents(@"SimpleSpell.txt");
            application.WaitForGameEnd();

            throw new NotImplementedException();
            //Assert.NotNull(
                //application.LightLayout.LastLightSource);
        }
    }
}
