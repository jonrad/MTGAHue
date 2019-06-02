using LightsApi.LightSources;
using Machine.Specifications;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(AngleFilterLightSource))]
    class AngleFilterLightSourceSpecs //my geometry teacher would be so proud. or not
    {
        static AngleFilterLightSource subject;

        static RGB[] results;

        class from_center
        {
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
                            0,
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
                            0,
                            0,
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
                            0,
                            0,
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
                            0,
                            0,
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
                        0,
                        0,
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

        class when_angle_is_not_centered
        {
            Establish context = () =>
                subject = new AngleFilterLightSource(
                    new SolidCircleLightSource(RGB.Red, -1, 0, 1),
                    -1,
                    0,
                    0,
                    90);

            Because of = () =>
            {
                results = new[]
                {
                    subject.Calculate(0, 1),
                    subject.Calculate(-1, 1)
                };
            };

            It should_calcuate_based_on_angles = () =>
                results.ShouldEqual(new[]
                {
                    RGB.Black,
                    RGB.Red
                });
        }
    }
}
