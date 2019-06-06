using LightsApi.LightSources;
using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Linq;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(GradientLightSource))]
    class GradientLightSourceSpecs : WithFakes
    {
        static GradientLightSource subject;

        static TestCase[] testCases;

        static RGB[] results;

        Because of = () =>
            results = testCases.Select(t => subject.Calculate(t.X, t.Y)).ToArray();

        static void Verify()
        {
            foreach (var (testCase, result) in testCases.Zip(results, (t, r) => (t, r)))
            {
                if (!testCase.Expected.Equals(result))
                {
                    throw new Exception($"Failed test case: {testCase.X},{testCase.Y}. Expected {testCase.Expected} Got {result}");
                }
            }
        }

        class for_horizontal
        {
            Establish context = () =>
            {
                subject = new GradientLightSource(RGB.Red, RGB.Blue, new Position(0, 1), new Position(1, 1));

                testCases = new[]
                {
                    new TestCase(0, 1, RGB.Red),
                    new TestCase(0, 2, RGB.Red),
                    new TestCase(.25, 1, (RGB.Red * .75) + (RGB.Blue * .25)),
                    new TestCase(.5, 1d, (RGB.Red * .5) + (RGB.Blue * .5)),
                    new TestCase(.75, 1d, (RGB.Red * .25) + (RGB.Blue * .75)),
                    new TestCase(1, 1d, RGB.Blue),
                    new TestCase(1.25, 1d, RGB.Black),
                    new TestCase(-.25, 1d, RGB.Black)
                };
            };

            It should_calculate_properly = () =>
                Verify();
        }

        class for_vertical
        {
            Establish context = () =>
            {
                subject = new GradientLightSource(RGB.Red, RGB.Blue, new Position(1, 0), new Position(1, 1));

                testCases = new[]
                {
                    new TestCase(1, 0, RGB.Red),
                    new TestCase(2, 0, RGB.Red),
                    new TestCase(1, .25, (RGB.Red * .75) + (RGB.Blue * .25)),
                    new TestCase(1, .5, (RGB.Red * .5) + (RGB.Blue * .5)),
                    new TestCase(1, .75, (RGB.Red * .25) + (RGB.Blue * .75)),
                    new TestCase(1, 1d, RGB.Blue),
                    new TestCase(1, 1.25, RGB.Black),
                    new TestCase(1, -.25, RGB.Black)
                };
            };

            It should_calculate_properly = () =>
                Verify();
        }

        class for_angled
        {
            Establish context = () =>
            {
                subject = new GradientLightSource(RGB.Red, RGB.Blue, new Position(0, 0), new Position(1, 1));

                testCases = new[]
                {
                    new TestCase(0, 0, RGB.Red),
                    new TestCase(0, 0, RGB.Red),
                    new TestCase(.25, .25, (RGB.Red * .75) + (RGB.Blue * .25)),
                    new TestCase(.5, .5, (RGB.Red * .5) + (RGB.Blue * .5)),
                    new TestCase(.75, .75, (RGB.Red * .25) + (RGB.Blue * .75)),
                    new TestCase(1, 1d, RGB.Blue),
                    new TestCase(1.25, 1.25, RGB.Black),
                    new TestCase(-.25, -.25, RGB.Black)
                };
            };

            It should_calculate_properly = () =>
                Verify();
        }

        class for_angled_reverse
        {
            Establish context = () =>
            {
                subject = new GradientLightSource(RGB.Red, RGB.Blue, new Position(1, 1), new Position(0, 0));

                testCases = new[]
                {
                    new TestCase(0, 0, RGB.Blue),
                    new TestCase(0, 0, RGB.Blue),
                    new TestCase(.25, .25, (RGB.Blue * .75) + (RGB.Red * .25)),
                    new TestCase(.5, .5, (RGB.Blue * .5) + (RGB.Red * .5)),
                    new TestCase(.75, .75, (RGB.Blue * .25) + (RGB.Red * .75)),
                    new TestCase(1, 1d, RGB.Red),
                    new TestCase(1.25, 1.25, RGB.Black),
                    new TestCase(-.25, -.25, RGB.Black)
                };
            };

            It should_calculate_properly = () =>
                Verify();
        }

        private class TestCase
        {
            public TestCase(double x, double y, RGB expected)
            {
                X = x;
                Y = y;
                Expected = expected;
            }

            public double X { get; }

            public double Y { get; }

            public RGB Expected { get; }
        }
    }
}
