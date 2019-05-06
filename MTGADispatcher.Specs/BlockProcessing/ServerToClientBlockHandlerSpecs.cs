using Machine.Fakes;
using Machine.Specifications;
using MTGADispatcher.BlockProcessing;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;

namespace MTGADispatcher.Specs.BlockProcessing
{
    [Subject(typeof(ServerToClientBlockHandler))]
    class ServerToClientBlockHandlerSpecs : WithSubject<ServerToClientBlockHandler>
    {
        private static JObject BuildMessageToClient(string message)
        {
            return JObject.Parse(@"{
    'greToClientEvent': {
        'greToClientMessages': [" + message + @"]
    }
}");
        }

        private static JObject BuildGameState(string message)
        {
            return BuildMessageToClient(@"{
                'type': 'GREMessageType_GameStateMessage',
                'gameStateMessage': " + message + @"
            }");
        }

        static Game game;

        static Block block;

        Establish context = () =>
        {
            game = new Game(The<IDispatcher<IMagicEvent>>());
            block = null;
            The<IInstanceBuilder>()
                .WhenToldTo(i => i.Build(Param.IsAny<JToken>()))
                .Return<JToken>(j => new Instance(j["id"].Value<int>(), j["id"].Value<int>(), 1, 1, new MagicColor[0]));
        };

        Because of = () =>
            Subject.Process(block, game);

        class when_processing_game_objects
        {
            class when_contains_single_game_object
            {
                Establish context = () =>
                    block = new Block("hi mom", BuildGameState(@"{
                        'gameObjects': [
                            { 'id': 1 }
                        ]
                    }"));

                It added_single_instance = () =>
                    game.InstancesById.Count.ShouldEqual(1);
            }
        }

        class when_processing_annotations
        {
            class when_contains_cast_spell
            {
                Establish context = () =>
                    block = new Block("hi mom", BuildGameState(@"{
                        'annotations': [ { 
                            'type': ['AnnotationType_ZoneTransfer'],
                            'affectedIds': [1],
                            'details': [
                                { 'key': 'foo', 'valueString': ['bar']},
                                { 'key': 'category', 'valueString': ['CastSpell'] }
                            ]
                        } ]
                    }"));

                It cast_a_spell_exclamation_point = () =>
                    game.Events.WasToldTo(d => d.Dispatch(Param.IsAny<CastSpell>()));
            }
        }
    }
}
