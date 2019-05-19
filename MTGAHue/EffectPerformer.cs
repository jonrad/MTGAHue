using LightsApi;
using MTGADispatcher.Events;

namespace MTGAHue
{
    public class EffectPerformer<T>
            where T : IMagicEvent
        {
            private readonly ILightLayout layout;

            private readonly IEffect<T> effect;

            public EffectPerformer(
                ILightLayout layout,
                IEffect<T> effect)
            {
                this.layout = layout;
                this.effect = effect;
            }

            public void Perform(T magicEvent)
            {
                var transition = effect.OnMagicEvent(magicEvent);

                if (transition == null)
                {
                    return;
                }

                transition
                    .Transition(layout);
            }
        }
}
