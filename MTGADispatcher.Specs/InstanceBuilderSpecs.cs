using Machine.Fakes;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace MTGADispatcher.Specs
{
    [Subject(typeof(InstanceBuilder))]
    class InstanceBuilderSpecs : WithSubject<InstanceBuilder>
    {
        static JObject input;

        static Instance result;

        Because of = () =>
            result = Subject.Build(input);

        class when_building_colorless
        {
            Establish context = () =>
                input = JObject.Parse(
                    @"{ 'instanceId': 12, 'grpId': 13 }");

            It created_expected_instance_id = () =>
                result.Id.ShouldEqual(12);

            It created_expected_card_id = () =>
                result.CardId.ShouldEqual(13);

            It was_colorless = () =>
                result.Colors.ShouldBeEmpty();
        }

        class when_building_with_single_valid_color
        {
            Establish context = () =>
                input = JObject.Parse(
                    @"{ 'instanceId': 12, 'grpId': 13, 'color': ['CardColor_Green'] }");

            It was_added_green_color = () =>
                result.Colors.ShouldContainOnly(new[] { MagicColor.Green });
        }

        class when_building_with_multiple_valid_color
        {
            Establish context = () =>
                input = JObject.Parse(
                    @"{ 'instanceId': 12, 'grpId': 13, 'color': ['CardColor_Green', 'CardColor_Blue'] }");

            It has_both_colors = () =>
                result.Colors.ShouldContainOnly(new[] { MagicColor.Green, MagicColor.Blue });
        }

        class when_building_with_invalid_color
        {
            Establish context = () =>
                input = JObject.Parse(
                    @"{ 'instanceId': 12, 'grpId': 13, 'color': ['CardColor_Ultraviolet'] }");

            It has_no_colors = () =>
                result.Colors.ShouldBeEmpty();
        }
    }
}
