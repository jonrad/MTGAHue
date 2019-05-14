using LightsApi.LightSources;
using Machine.Specifications;

namespace LightsApi.Specs.LightSources
{
    [Subject(typeof(CompositeLightSource))]
    class CompositeLightSourceSpecs
    {
        static RGB rgb1, rgb2;

        static RGB result;

        class when_single
        {
            Because of = () =>
                result = new CompositeLightSource(new SolidLightSource(rgb1)).Calculate(0, 0);

            class with_red
            {
                Establish context = () =>
                    rgb1 = RGB.Red;

                It returned_red = () =>
                    rgb1.ShouldEqual(RGB.Red);
            }
        }

        class with_two_colors
        {
            Because of = () =>
                result = new CompositeLightSource(new SolidLightSource(rgb1), new SolidLightSource(rgb2)).Calculate(0, 0);

            class when_one_color_overpowers_other
            {
                Establish context = () =>
                    (rgb1, rgb2) = (RGB.Green, RGB.Green * .2);

                It used_max_color = () =>
                    result.ShouldEqual(RGB.Green);
            }

            class when_mixing_colors
            {
                Establish context = () =>
                    (rgb1, rgb2) = (RGB.Red, RGB.Blue);

                It was_additive = () =>
                    result.ShouldEqual(new RGB(255, 0, 255));
            }
        }
    }
}
