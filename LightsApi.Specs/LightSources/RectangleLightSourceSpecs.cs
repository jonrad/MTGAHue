using LightsApi.LightSources;
using Machine.Specifications;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(RectangleLightSource))]
    class RectangleLightSourceSpecs
    {
        static double x, y;

        static RGB result;

        static RectangleLightSource subject =
            new RectangleLightSource(RGB.Red, -5, 5, 1, 3); //-5 <= X <= -4, 5 <= Y <= 8

        Because of = () =>
            result = subject.Calculate(x, y);

        class when_out_of_bounds
        {
            Establish context = () =>
                (x, y) = (-2, -2);

            It returned_black = () =>
                result.ShouldEqual(RGB.Black);
        }

        class when_in_bounds
        {
            Establish context = () =>
                (x, y) = (-4.5, 6);

            It should_return_red = () =>
                result.ShouldEqual(RGB.Red);
        }

        class on_left_border
        {
            Establish context = () =>
                (x, y) = (-5, 6);

            It should_return_red = () =>
                result.ShouldEqual(RGB.Red);
        }

        class on_right_border
        {
            Establish context = () =>
                (x, y) = (-4, 6);

            It should_return_red = () =>
                result.ShouldEqual(RGB.Red);
        }

        class on_top_border
        {
            Establish context = () =>
                (x, y) = (-4.5, 5);

            It should_return_red = () =>
                result.ShouldEqual(RGB.Red);
        }

        class on_bottom_border
        {
            Establish context = () =>
                (x, y) = (-4.5, 8);

            It should_return_red = () =>
                result.ShouldEqual(RGB.Red);
        }
    }
}
