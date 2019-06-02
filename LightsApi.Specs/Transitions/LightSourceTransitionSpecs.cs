using LightsApi.LightSources;
using LightsApi.Transitions;
using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Threading.Tasks;

namespace LightsApi.Specs.Transitions
{
    [Subject(typeof(LightSourceTransition))]
    class LightSourceTransitionSpecs : WithFakes
    {
        static LightSourceTransition subject;

        static Task result;

        Establish context = () =>
            subject = new LightSourceTransition(The<ILightSource>(), 100);

        class when_transitioning
        {
            Because of = () =>
                result = subject.Transition(The<ILayer>());

            It told_light_layer_to_transition = () =>
                The<ILayer>().WasToldTo(l => l.Transition(The<ILightSource>(), TimeSpan.FromMilliseconds(100), default));
        }
    }
}
