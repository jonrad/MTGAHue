using LightsApi.LightSources;
using Machine.Specifications;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(SolidCircleLightSource))]
    class SolidCircleLightSourceSpecs
    {
        static SolidCircleLightSource subject;

        static RGB result;

        static double x, y;

        Establish context = () =>
            subject = new SolidCircleLightSource(RGB.White, 0, 0, 1, .2);

        Because of = () =>
            result = subject.Calculate(x, y);

        class in_center
        {
            Establish context = () =>
                (x, y) = (0, 0);

            It returned_black = () =>
                result.ShouldEqual(RGB.Black);
        }

        class in_inner_border
        {
            Establish context = () =>
                (x, y) = (.2, 0);

            It returned_white = () =>
                result.ShouldEqual(RGB.White);
        }

        class in_outer_border
        {
            Establish context = () =>
                (x, y) = (1, 0);

            It returned_white = () =>
                result.ShouldEqual(RGB.White);
        }

        class outside_of_area
        {
            Establish context = () =>
                (x, y) = (10, 0);

            It returned_black = () =>
                result.ShouldEqual(RGB.Black);
        }

        class half_way
        {
            Establish context = () =>
                (x, y) = (.5, 0);

            It returned_white = () =>
                result.ShouldEqual(RGB.White);
        }
    }
}
