﻿using MTGADispatcher.ClientModels;
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
            ProcessAnnotations(message["annotations"] as JArray, game);
        }

        private void ProcessGameObjects(JArray? gameObjects, Game game)
        {
            if (gameObjects == null)
            {
                return;
            }

            foreach (var instanceModel in gameObjects.Select(g => g.ToObject<InstanceModel>()))
            {
                var instance = instanceBuilder.Build(instanceModel);

                game.InstancesById[instance.Id] = instance;
            }
        }

        private void ProcessAnnotations(JArray? annotations, Game game)
        {
            if (annotations == null)
            {
                return;
            }

            foreach (var annotation in annotations)
            {
                var type = annotation["type"][0].Value<string>();
                if (type != "AnnotationType_ZoneTransfer")
                {
                    continue;
                }

                var details = annotation["details"] as JArray;
                var instanceId = annotation["affectedIds"]?.First()?.Value<int>();

                if (details == null)
                {
                    continue;
                }

                string? category = null;
                foreach (var detail in details)
                {
                    if (detail["key"].Value<string>() == "category")
                    {
                        category = detail["valueString"][0].Value<string>();
                        break;
                    }
                }

                if (category == "CastSpell")
                {
                    if (instanceId == null)
                    {
                        continue;
                    }

                    game.InstancesById.TryGetValue(instanceId.Value, out var instance);
                    game.Events.Dispatch(new CastSpell(instance));
                }
                else if (category == "PlayLand")
                {
                    if (instanceId == null)
                    {
                        continue;
                    }

                    game.InstancesById.TryGetValue(instanceId.Value, out var instance);
                    game.Events.Dispatch(new PlayLand(instance));
                }
            }
        }
    }
}
