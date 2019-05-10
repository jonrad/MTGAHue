using MTGADispatcher.Integration.Fixtures;
using System;
using System.Linq;
using Xunit;

namespace MTGADispatcher.Integration.Features
{
    public class CastSpellFeature : IDisposable
    {
        private readonly TestFixture fixture;

        public CastSpellFeature()
        {
            fixture = new TestFixture();
        }

        public void Dispose()
        {
            fixture.Dispose();
        }

        [Fact]
        public void CastSingleMulticoloredSpell()
        {
            GivenGameStarted();

            fixture.WriteContents(@"SimpleSpell.txt");
            fixture.WaitForGameEnd();

            Assert.Collection(
                fixture.SpellsCast,
                s => 
                {
                    Assert.Equal(s.Instance.Colors.OrderBy(c => c), new[] { MagicColor.Black, MagicColor.White });
                });
        }

        private void GivenGameStarted()
        {
            fixture.CreateGame();
        }
    }
}
