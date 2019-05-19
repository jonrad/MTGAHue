using Machine.Fakes;
using Machine.Specifications;
using System;

namespace MTGADispatcher.Specs
{
    [Subject(typeof(BlockBuilder))]
    class BlockBuilderSpecs : WithSubject<BlockBuilder>
    {
        static Block block;

        Because of = () =>
            block = Subject.TryBuild();

        class when_empty
        {
            It returned_null_block = () =>
                block.ShouldBeNull();
        }

        class when_garbage_line_added
        {
            Establish context = () =>
                Subject.Append("Garbage");

            It returned_null_block = () =>
                block.ShouldBeNull();
        }

        class for_request_response
        {
            class when_valid_json
            {
                Establish context = () =>
                    Append(@"
==> Log.Info(530):
{
    ""json"": ""stuff""
}");

               It returned_block = () =>
                    block.ShouldNotBeNull();

                It set_title = () =>
                    block.Title.ShouldEqual("Log.Info");

                It set_jobject = () =>
                    block.JObject["json"].ToString().ShouldEqual("stuff");
            }

            class when_invalid_json
            {
                Establish context = () =>
                    Append(@"
==> HelloWorld()
{ foobar }");

                It failed = () =>
                    block.ShouldBeNull();
            }

            class when_array_json
            {
                Establish context = () =>
                    Append(@"
==> HelloWorld()
[ { ""foo"": ""bar"" } ]");

                It returned_object_with_title_as_main_token = () =>
                    block.JObject["HelloWorld"][0]["foo"].ToString().ShouldEqual("bar");
            }
        }

        class for_timestamped_line
        {
            class when_valid_json
            {
                Establish context = () =>
                Append(@"[UnityCrossThreadLogger]6/7/2018 7:21:03 PM: Match to 26848417E29213FE: GreToClientEvent
{
  ""json"": ""stuff""
}");

                It set_title_to_last_word = () =>
                    block.Title.ShouldEqual("GreToClientEvent");

                It should_return_expected_jtoken = () =>
                    block.JObject["json"].ToString().ShouldEqual("stuff");
            }
        }

        private static void Append(string text)
        {
            foreach (var line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                Subject.Append(line);
            }
        }
    }
}
