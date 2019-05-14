using MTGADispatcher.Integration.Fixtures;
using System;
using System.Linq;
using Xunit;

namespace MTGADispatcher.Integration.Features
{
    public class PlayLandFeature : IDisposable
    {
        private readonly TestFixture fixture;

        public PlayLandFeature()
        {
            fixture = new TestFixture();
        }

        public void Dispose()
        {
            fixture.Dispose();
        }

        [Fact]
        public void NotifiesOfLandPlay()
        {
            fixture.CreateGame();

            fixture.WriteContents(@"SimplePlayLand.txt");
            fixture.WaitForGameEnd();

            Assert.Collection(
                fixture.PlayedLands,
                s => 
                {
                    Assert.Equal(
                        s.Instance.Colors.OrderBy(c => c),
                        new[] { MagicColor.Blue, MagicColor.Red });
                });
        }
    }
}
