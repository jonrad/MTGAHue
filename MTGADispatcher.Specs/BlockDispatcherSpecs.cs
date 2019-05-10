using Machine.Fakes;
using Machine.Specifications;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Specs.Mocks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MTGADispatcher.Specs
{
    [Subject(typeof(BlockDispatcher))]
    class BlockDispatcherSpecs : WithFakes
    {
        static BlockDispatcher subject;

        static LineReaderProxy line_reader;

        static Exception exception;

        static string[] precleared_lines;

        static List<string> lines;

        static void WriteSync(string text)
        {
            line_reader.SetResult(text);
            if (!line_reader.LineRead.WaitOne(TimeSpan.FromSeconds(2)))
            {
                throw new TimeoutException();
            }
        }

        static void WriteLineSync(string text)
        {
            WriteSync(text + Environment.NewLine);
        }

        static void WriteLinesSync(params string[] lines)
        {
            foreach (var line in lines)
            {
                WriteLineSync(line);
            }
        }

        Establish context = () =>
        {
            lines = new List<string>();

            The<IBlockBuilder>()
                .WhenToldTo(w => w.Append(Param.IsAny<string>()))
                .Callback<string>(s => lines.Add(s));

            The<IBlockBuilder>()
                .WhenToldTo(w => w.Clear())
                .Callback(() =>
                {
                    precleared_lines = lines.ToArray();
                    lines.Clear();
                });

            line_reader = new LineReaderProxy();

            subject = new BlockDispatcher(
                The<IBlockBuilder>(),
                The<IDispatcher<Block>>(),
                line_reader);
        };

        Cleanup cleanup = () =>
            subject.Stop();

        class when_started
        {
            Establish context = () =>
                subject.Start();

            class when_stopped_twice
            {
                Because of = () =>
                {
                    subject.Stop();
                    exception = Catch.Exception(() => subject.Stop());
                };

                It did_nothing = () =>
                    exception.ShouldBeNull();
            }

            class when_started_twice
            {
                Because of = () =>
                    exception = Catch.Exception(() => subject.Start());

                It threw_exception = () =>
                    exception.ShouldBeOfExactType<InvalidOperationException>();
            }

            class when_read_line
            {
                Because of = () =>
                    WriteLineSync("Hello");

                It added_stripped_line_to_block_builder = () =>
                    lines.ShouldContainOnly("Hello");
            }

            class when_read_gre_line
            {
                Establish context = () =>
                    lines.Add("Foo");

                Because of = () =>
                    WriteLineSync($"[Client GRE]");

                It cleared = () =>
                    lines.ShouldContainOnly("[Client GRE]");
            }

            class when_read_unity_cross_thread_line
            {
                Establish context = () =>
                    lines.Add("Foo");

                Because of = () =>
                    WriteLineSync($"[UnityCrossThreadLogger]");

                It cleared = () =>
                    lines.ShouldContainOnly("[UnityCrossThreadLogger]");
            }

            class when_append_json_end
            {
                Because of = () =>
                    WriteLinesSync("}");

                It built_block = () =>
                    The<IBlockBuilder>().WasToldTo(w => w.TryBuild());

                It cleared = () =>
                    lines.ShouldBeEmpty();

                It appended_line_to_Build = () =>
                    precleared_lines.ShouldContainOnly("}");

                It did_not_dispatch_for_invalid_block = () =>
                    The<IDispatcher<Block>>().WasNotToldTo(w => w.Dispatch(Param.IsAny<Block>()));
            }

            class when_block_builder_returns_block
            {
                Establish context = () =>
                    The<IBlockBuilder>()
                        .WhenToldTo(w => w.TryBuild())
                        .Return((true, new Block("Hello world", new JObject())));

                class when_squigly_bracket
                {
                    Because of = () =>
                        WriteLinesSync("}");

                    It dispatched = () =>
                        The<IDispatcher<Block>>().WasToldTo(w => w.Dispatch(Param.IsAny<Block>()));
                }

                class when_square_bracket
                {
                    Because of = () =>
                        WriteLinesSync("]");

                    It dispatched = () =>
                        The<IDispatcher<Block>>().WasToldTo(w => w.Dispatch(Param.IsAny<Block>()));
                }

                class when_random_token
                {
                    Because of = () =>
                        WriteLinesSync("☺");

                    It did_not_dispatch = () =>
                        The<IDispatcher<Block>>().WasNotToldTo(w => w.Dispatch(Param.IsAny<Block>()));
                }
            }
        }
    }
}
