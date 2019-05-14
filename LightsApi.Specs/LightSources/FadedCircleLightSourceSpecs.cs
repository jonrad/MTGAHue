using LightsApi.LightSources;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(FadedCircleLightSource))]
    class FadedCircleLightSourceSpecs
    {
        static FadedCircleLightSource subject =
            new FadedCircleLightSource(RGB.Red, 0, 0, 5, 2);

        static double y;

        static RGB result;

        Because of = () =>
            result = subject.Calculate(0, y);

        class out_of_bounds
        {
            Establish context = () =>
                y = 10;

            It returned_black = () =>
                result.ShouldEqual(RGB.Black);
        }

        class out_of_inner_bounds
        {
            Establish context = () =>
                y = 0;

            It returned_black = () =>
                result.ShouldEqual(RGB.Black);
        }

        class on_radius_line
        {
            Establish context = () =>
                y = 5;

            It returned_red = () =>
                result.ShouldEqual(RGB.Red);
        }

        class on_halfway_inner_line
        {
            Establish context = () =>
                y = 4;

            It returned_faded_red = () =>
                result.ShouldEqual(RGB.Red * .5);
        }

        class on_halfway_outer_line
        {
            Establish context = () =>
                y = 6;

            It returned_faded_red = () =>
                result.ShouldEqual(RGB.Red * .5);
        }
    }
}
