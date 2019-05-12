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
                result = subject.Transition(The<ILightLayout>());

            It told_light_layout_to_transition = () =>
                The<ILightLayout>().WasToldTo(l => l.Transition(The<ILightSource>(), TimeSpan.FromMilliseconds(100), default));
        }
    }
}
