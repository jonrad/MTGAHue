using LightsApi.Injectables;
using LightsApi.LightSources;
using LightsApi.Transitions;
using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Specs
{
    [Subject(typeof(Layer))]
    class LightLayerSpecs : WithFakes
    {
        static Layer subject;

        static CancellationTokenSource cancellationTokenSource;

        Establish context = () =>
        {
            subject = new Layer(
                The<IStopwatchBuilder>(),
                The<IDelay>(),
                new[]
                {
                    new Position(0, 0),
                    new Position(-.5f, .5f),
                    new Position(.5f, -.5f)
                },
                TimeSpan.FromMilliseconds(1));

            cancellationTokenSource = new CancellationTokenSource();

            The<IStopwatchBuilder>().WhenToldTo(t => t.StartNew()).Return(The<IStopwatch>);
            The<IDelay>().WhenToldTo(d => d.Wait(Param.IsAny<TimeSpan>(), Param.IsAny<CancellationToken>()))
                .Return(() =>
                {
                    cancellationTokenSource.Cancel();
                    return Task.FromResult(0);
                });
        };

        class when_transitioning_from_black_to_red_in_4_ms
        {
            static ITransition transition;

            Establish context = () =>
                transition = new LightSourceTransition(new SolidLightSource(RGB.Red), 4);

            class second_0
            {
                Establish context = () =>
                    The<IStopwatch>().WhenToldTo(s => s.ElapsedMilliseconds).Return(0);

                Because of = () =>
                    subject.Transition(transition, cancellationTokenSource.Token).Await();

                It set_color = () =>
                    subject.Colors[0].ShouldEqual(RGB.Black);
            }

            class second_1
            {
                Establish context = () =>
                    The<IStopwatch>().WhenToldTo(s => s.ElapsedMilliseconds).Return(1);

                Because of = () =>
                    subject.Transition(transition, cancellationTokenSource.Token).Await();

                It set_color = () =>
                    subject.Colors[0].ShouldEqual(RGB.Red * (1f / 4f));
            }

            class second_10
            {
                Establish context = () =>
                    The<IStopwatch>().WhenToldTo(s => s.ElapsedMilliseconds).Return(10);

                Because of = () =>
                    subject.Transition(transition, cancellationTokenSource.Token).Await();

                It set_color = () =>
                    subject.Colors[0].ShouldEqual(RGB.Red);
            }
        }
    }
}
