using Machine.Specifications;
using System;

namespace LightsApi.Specs
{
    [Subject(typeof(RGB))]
    class RgbSpecs
    {
        static Exception exception;

        class when_creating
        {
            static RGB result;

            static float r, g, b;

            Because of = () =>
                exception = Catch.Exception(() => result = new RGB(r, g, b));

            class in_general
            {
                Establish context = () =>
                    (r, g, b) = (1, 2, 3);

                It set_rgb_values = () =>
                {
                    result.R.ShouldEqual(1);
                    result.G.ShouldEqual(2);
                    result.B.ShouldEqual(3);
                };
            }

            class for_negatives
            {
                class for_r
                {
                    Establish context = () =>
                        r = -1;

                    It threw_exception = () =>
                        exception.ShouldBeOfExactType<ArgumentException>();
                }

                class for_g
                {
                    Establish context = () =>
                        g = -1;

                    It threw_exception = () =>
                        exception.ShouldBeOfExactType<ArgumentException>();
                }

                class for_b
                {
                    Establish context = () =>
                        b = -1;

                    It threw_exception = () =>
                        exception.ShouldBeOfExactType<ArgumentException>();
                }
            }

            class for_large_values
            {
                class for_r
                {
                    Establish context = () =>
                        r = 300;

                    It threw_exception = () =>
                        exception.ShouldBeOfExactType<ArgumentException>();
                }

                class for_g
                {
                    Establish context = () =>
                        g = 300;

                    It threw_exception = () =>
                        exception.ShouldBeOfExactType<ArgumentException>();
                }

                class for_b
                {
                    Establish context = () =>
                        b = 300;

                    It threw_exception = () =>
                        exception.ShouldBeOfExactType<ArgumentException>();
                }
            }
        }

        class when_comparing
        {
            static bool result;

            class when_same
            {
                Because of = () =>
                    result = new RGB(2, 3, 4).Equals(new RGB(2, 3, 4));

                It was_same = () =>
                    result.ShouldBeTrue();
            }

            class when_different
            {
                Because of = () =>
                    result = new RGB(2, 3, 4).Equals(new RGB(1, 2, 3));

                It was_different = () =>
                    result.ShouldBeFalse();
            }
        }

        class when_performing_operations
        {
            static RGB subject;

            static RGB result;

            Establish context = () =>
                subject = new RGB(2, 4, 6);

            class when_multiplying
            {
                Because of = () =>
                    result = subject * 2;

                It multiplied = () =>
                    result.ShouldEqual(new RGB(4, 8, 12));
            }

            class when_adding
            {
                Because of = () =>
                    result = subject + new RGB(1, 10, 0);

                It took_largest_values = () =>
                    result.ShouldEqual(new RGB(2, 10, 6));
            }
        }
    }
}
