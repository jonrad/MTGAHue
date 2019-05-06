using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MTGADispatcher.BlockProcessing
{
    public class ServerToClientBlockHandler : IGameUpdater
    {
        private readonly IInstanceBuilder instanceBuilder;

        public ServerToClientBlockHandler(IInstanceBuilder instanceBuilder)
        {
            this.instanceBuilder = instanceBuilder;
        }

        public void Process(Block block, Game game)
        {
            var token = block.JObject["greToClientEvent"]?["greToClientMessages"];
            if (token == null)
            {
                return;
            }

            if (token is JArray clientMessages)
            {
                foreach (var message in clientMessages)
                {
                    var messageType = message["type"].Value<string>();
                    if (messageType == "GREMessageType_GameStateMessage" || messageType == "GREMessageType_QueuedGameStateMessage")
                    {
                        ProcessGameStateMessage(message["gameStateMessage"], game);
                    }
                }
            }
        }

        private void ProcessGameStateMessage(JToken message, Game game)
        {
            ProcessGameObjects(message["gameObjects"] as JArray, game);
            ProcessZoneObjects(message["zones"] as JArray, game);
            ProcessAnnotations(message["annotations"] as JArray, game);
        }

        private void ProcessZoneObjects(JArray zones, Game game)
        {
            if (zones == null)
            {
                return;
            }

            var zone = zones.FirstOrDefault(z => z.Value<string>("type") == "ZoneType_Battlefield");

            if (zone == null)
            {
                return;
            }

            var instanceIds = zone["objectInstanceIds"]?.Values<int>();

            if (instanceIds == null)
            {
                return;
            }

            var instances = instanceIds
                .Select(i =>
                {
                    game.InstancesById.TryGetValue(i, out var instance);
                    return instance;
                })
                .Where(i => i != null)
                .ToArray();

            game.Events.Dispatch(new SetBattlefield(instances));
        }

        private void ProcessGameObjects(JArray gameObjects, Game game)
        {
            if (gameObjects == null)
            {
                return;
            }

            foreach (var gameObject in gameObjects)
            {
                var instance = instanceBuilder.Build(gameObject);

                game.InstancesById[instance.Id] = instance;
            }
        }

        private void ProcessAnnotations(JArray annotations, Game game)
        {
            if (annotations == null)
            {
                return;
            }

            foreach (var annotation in annotations)
            {
                var type = annotation["type"][0].Value<string>();
                if (type == "AnnotationType_ZoneTransfer")
                {
                    var details = annotation["details"] as JArray;
                    var instanceId = annotation["affectedIds"]?.First()?.Value<int>();
                    if (details != null)
                    {
                        string category = null;
                        foreach (var detail in details)
                        {
                            switch (detail["key"].Value<string>())
                            {
                                case "category":
                                    category = detail["valueString"][0].Value<string>();
                                break;
                            }
                        }

                        if (category == "CastSpell")
                        {
                            game.InstancesById.TryGetValue(instanceId.Value, out var instance);
                            game.Events.Dispatch(new CastSpell(instance));
                        }
                    }
                }
            }
        }
    }
}
