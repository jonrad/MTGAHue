using LightsApi.Injectables;
using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Specs
{
    [Subject(typeof(Lights))]
    class LightsSpecs : WithFakes
    {
        static Lights subject;

        Establish context = () =>
        {
            subject = new Lights(The<IDelay>(), The<ILayerBuilder>(), The<ILightClient>(), TimeSpan.FromMilliseconds(1));

            The<ILayerBuilder>().WhenToldTo(l => l.Build(Param.IsAny<Position[]>(), Param.IsAny<TimeSpan>()))
                .Return(() => An<ILayer>());

            The<ILightClient>().WhenToldTo(c => c.Lights).Return(new[]
            {
                new Position(0, 0)
            });

            The<IDelay>().WhenToldTo(d => d.Wait(Param.IsAny<TimeSpan>(), Param.IsAny<CancellationToken>()))
                .Return(() =>
                {
                    subject.Stop();
                    return Task.FromResult(0);
                });
        };

        Cleanup cleanup = () =>
            subject.Stop();

        class when_started
        {
            static ManualResetEvent isCompleted = new ManualResetEvent(false);

            Because context = () =>
            {
                subject.Start();
                isCompleted.WaitOne(1000);
            };

            class with_no_layers
            {
                It set_colors_to_black = () =>
                    The<ILightClient>().WasToldTo(
                        t => t.SetColors(
                            Param<IEnumerable<RGB>>.Matches(rgb => rgb.First().Equals(RGB.Black)),
                            Param.IsAny<CancellationToken>()));

                It did_not_delay = () =>
                    The<IDelay>().WasNotToldTo(d => d.Wait(Param.IsAny<TimeSpan>(), Param.IsAny<CancellationToken>()));
            }

            class with_single_layer
            {
                Establish context = () =>
                {
                    var layer = subject.AddLayer();
                    layer.WhenToldTo(l => l.Colors).Return(new[] { RGB.Red });
                };

                It did_delay = () =>
                    The<IDelay>().WasToldTo(d => d.Wait(Param.IsAny<TimeSpan>(), Param.IsAny<CancellationToken>()));

                It set_colors_to_red = () =>
                    The<ILightClient>().WasToldTo(
                        t => t.SetColors(
                            Param<IEnumerable<RGB>>.Matches(rgb => rgb.First().Equals(RGB.Red)),
                            Param.IsAny<CancellationToken>()));
            }

            class with_two_layers
            {
                Establish context = () =>
                {
                    int count = 0;
                    var setupComplete = new TaskCompletionSource<bool>();

                    The<IDelay>().WhenToldTo(d => d.Wait(Param.IsAny<TimeSpan>(), Param.IsAny<CancellationToken>()))
                        .Return(async () =>
                        {
                            await setupComplete.Task;
                            count++;
                            if (count == 2)
                            {
                                subject.Stop();
                            }
                        });

                    var layer = subject.AddLayer();
                    layer.WhenToldTo(l => l.Colors).Return(new[] { RGB.Red });

                    layer = subject.AddLayer();
                    layer.WhenToldTo(l => l.Colors).Return(new[] { RGB.Blue });

                    setupComplete.SetResult(true);
                };

                It did_delay = () =>
                    The<IDelay>().WasToldTo(d => d.Wait(Param.IsAny<TimeSpan>(), Param.IsAny<CancellationToken>()));

                It set_colors_to_cumalative = () =>
                    The<ILightClient>().WasToldTo(
                        t => t.SetColors(
                            Param<IEnumerable<RGB>>.Matches(rgb => rgb.First().Equals(new RGB(255, 0, 255))),
                            Param.IsAny<CancellationToken>()));
            }
        }
    }
}
