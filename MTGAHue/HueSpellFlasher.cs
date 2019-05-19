using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue
{
    public interface IEventEffect<T>
        where T : IMagicEvent
    {
        string Name { get; }

        void OnMagicEvent(T magicEvent);
    }

    public interface IEventEffectBuilder
    {
        IEventEffect<T>[] Get<T>(Game game, ILightLayout layout)
            where T : IMagicEvent;
    }

    public class HueSpellFlasher
        : IEventEffect<CastSpell>
    {
        private Dictionary<MagicColor, RGB> colorMap = new Dictionary<MagicColor, RGB>
        {
            [MagicColor.Black] = new RGB(255, 255, 255) * .3,
            [MagicColor.White] = new RGB(255, 255, 255),
            [MagicColor.Red] = new RGB(255, 0, 0),
            [MagicColor.Green] = new RGB(0, 255, 0),
            [MagicColor.Blue] = new RGB(0, 0, 255)
        };

        private CancellationTokenSource cancellationTokenSource;

        private ISubscriptions<IMagicEvent> subscriptions;

        private readonly ILightLayout layout;

        public HueSpellFlasher(Game game, ILightLayout layout)
        {
            cancellationTokenSource = new CancellationTokenSource();
            subscriptions = game.Events.Subscriptions;
            this.layout = layout;
        }

        public string Name => "Flash";  //Savior of the universe

        private Func<CancellationToken, Task> BuildEffect(RGB color)
        {
            var flash = new ITransition[]
            {
                new LightSourceTransition(new SolidLightSource(color), 500),
                new LightSourceTransition(new SolidLightSource(color * .6), 500)
            };

            var flashCount = 5;

            var transitions =
                new[] { new LightSourceTransition(new SolidLightSource(color * .1), 50) }
                .Concat(Enumerable.Repeat(flash, flashCount).SelectMany(s => s))
                .Concat(new[] { new LightSourceTransition(new SolidLightSource(color * .3), 5000) })
                .ToArray();

            var composite = new CompositeTransition(transitions);

            return token => composite.Transition(layout, token);
        }

        private void RunEffect(RGB color, CancellationToken token)
        {
            var effect = BuildEffect(color);

            effect(token);
        }

        public void OnCastSpell(CastSpell spell)
        {
            CancelPrevious();

            var colors = spell.Instance.Colors;

            if (colors.Length == 0)
            {
                return;
            }

            var rgb = colorMap[colors[0]];

            cancellationTokenSource = new CancellationTokenSource();

            RunEffect(rgb, cancellationTokenSource.Token);
        }

        public void OnMagicEvent(CastSpell magicEvent)
        {
            OnCastSpell(magicEvent);
        }

        private void CancelPrevious()
        {
            cancellationTokenSource.Cancel();
        }

        public void Start()
        {
            subscriptions.Subscribe<CastSpell>(OnCastSpell);
        }

        public void Stop()
        {
            subscriptions.Unsubscribe<CastSpell>(OnCastSpell);
        }
    }
}
