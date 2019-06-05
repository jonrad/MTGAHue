using LightsApi.LightSources;
using LightsApi.Transitions;
using Machine.Fakes;
using Machine.Specifications;

namespace LightsApi.Specs.Transitions
{
    [Subject(typeof(FadeInTransition))]
    class FadeInTransitionSpecs : WithFakes
    {
        static FadeInTransition subject;

        static RGB result;

        Establish context = () =>
            subject = new FadeInTransition(new SolidLightSource(RGB.Red), 100);

        class when_calculating
        {
            class at_time_0
            {
                Because of = () =>
                    result = subject.Get(0f, 0f, 0L);

                It calcuted_starting_color = () =>
                    result.ShouldEqual(RGB.Black);
            }

            class at_half_way
            {
                Because of = () =>
                    result = subject.Get(0f, 0f, 50L);

                It calcuted_half_color = () =>
                    result.ShouldEqual(RGB.Red * .5);
            }

            class at_end_time
            {
                Because of = () =>
                    result = subject.Get(0f, 0f, 100L);

                It calcuted_end_color = () =>
                    result.ShouldEqual(RGB.Red);
            }

            class after_end_time
            {
                Because of = () =>
                    result = subject.Get(0f, 0f, 999L);

                It calcuted_end_color = () =>
                    result.ShouldEqual(RGB.Red);
            }
        }
    }
}
