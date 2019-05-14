using LightsApi.LightSources;
using Machine.Specifications;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(AngleFilterLightSource))]
    class AngleFilterLightSourceSpecs //my geometry teacher would be so proud. or not
    {
        static AngleFilterLightSource subject;

        static RGB[] results;

        Because of = () =>
            results = new[]
            {
                subject.Calculate(1, 1),
                subject.Calculate(-1, 1),
                subject.Calculate(-1, -1),
                subject.Calculate(1, -1),
            };

        class when_angle_does_not_cross_360
        {

            class in_first_quadrant
            {
                Establish context = () =>
                    subject = new AngleFilterLightSource(
                        new SolidLightSource(RGB.Red),
                        0,
                        90);

                It should_match_quadrants = () =>
                    results.ShouldEqual(new[]
                    {
                        RGB.Red,
                        RGB.Black,
                        RGB.Black,
                        RGB.Black
                    });
            }

            class in_second_quadrant
            {
                Establish context = () =>
                    subject = new AngleFilterLightSource(
                        new SolidLightSource(RGB.Red),
                        90,
                        90);

                It should_match_quadrants = () =>
                    results.ShouldEqual(new[]
                    {
                        RGB.Black,
                        RGB.Red,
                        RGB.Black,
                        RGB.Black
                    });
            }

            class in_third_quadrant
            {
                Establish context = () =>
                    subject = new AngleFilterLightSource(
                        new SolidLightSource(RGB.Red),
                        180,
                        90);

                It should_match_quadrants = () =>
                    results.ShouldEqual(new[]
                    {
                        RGB.Black,
                        RGB.Black,
                        RGB.Red,
                        RGB.Black
                    });
            }

            class in_fourth_quadrant
            {
                Establish context = () =>
                    subject = new AngleFilterLightSource(
                        new SolidLightSource(RGB.Red),
                        270,
                        90);

                It should_match_quadrants = () =>
                    results.ShouldEqual(new[]
                    {
                        RGB.Black,
                        RGB.Black,
                        RGB.Black,
                        RGB.Red
                    });
            }
        }

        class when_angle_crosses_360
        {
            Establish context = () =>
                subject = new AngleFilterLightSource(
                    new SolidLightSource(RGB.Red),
                    270,
                    180);

            It should_match_quadrants = () =>
                results.ShouldEqual(new[]
                {
                    RGB.Red,
                    RGB.Black,
                    RGB.Black,
                    RGB.Red
                });
        }
    }
}
