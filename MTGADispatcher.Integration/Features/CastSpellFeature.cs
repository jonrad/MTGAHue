using System.Linq;
using Xunit;

namespace MTGADispatcher.Integration.Features
{
    public class CastSpellFeature : BaseFeature
    {
        [Fact]
        public void NotifiesOfSpellCast()
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
