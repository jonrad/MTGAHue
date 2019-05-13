using MTGADispatcher;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using System;

namespace MTGAHue
{
    public class Debugger : IGameSubscriber
    {
        private readonly ISubscriptions<IMagicEvent> subscriptions;

        public Debugger(Game game)
        {
            subscriptions = game.Events.Subscriptions;
        }

        public void Start()
        {
            subscriptions.Subscribe<CastSpell>(OnCastSpell);
            subscriptions.Subscribe<PlayLand>(OnPlayerLand);
        }

        public void Stop()
        {
            subscriptions.Unsubscribe<CastSpell>(OnCastSpell);
            subscriptions.Unsubscribe<PlayLand>(OnPlayerLand);
        }

        private void OnCastSpell(CastSpell castSpell)
        {
            Console.WriteLine($"Cast Spell with Colors: {string.Join(" ", castSpell.Instance.Colors)}");
        }

        private void OnPlayerLand(PlayLand playLand)
        {
            Console.WriteLine($"Played Land with Colors: {string.Join(" ", playLand.Instance.Colors)}");
        }
    }
}
