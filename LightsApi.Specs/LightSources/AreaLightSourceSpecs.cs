using LightsApi.LightSources;
using Machine.Specifications;
using System;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(AreaLightSource))]
    class AreaLightSourceSpecs
    {
        static AreaLightSource subject;

        static RGB result;

        static double x, y;

        Establish context = () =>
            subject = new AreaLightSource(RGB.White, 0, 0, 1);

        Because of = () =>
            result = subject.Calculate(x, y);

        class in_center
        {
            Establish context = () =>
                (x, y) = (0, 0);

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

        class at_border
        {
            Establish context = () =>
                (x, y) = (1, 0);

            It returned_black = () =>
                result.ShouldEqual(RGB.Black);
        }

        class half_way
        {
            Establish context = () =>
                (x, y) = (.5, 0);

            It returned_grey = () =>
                result.ShouldEqual(new RGB(127.5f, 127.5f, 127.5f));
        }

        class half_way_bottom_left
        {
            Establish context = () =>
                (x, y) = (-Math.Sqrt(.5)/2, -Math.Sqrt(.5)/2);

            It returned_grey = () =>
                result.ShouldEqual(new RGB(127.5f, 127.5f, 127.5f));
        }
    }
}
