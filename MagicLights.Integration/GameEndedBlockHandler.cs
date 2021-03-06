﻿using MTGADispatcher;
using MTGADispatcher.BlockProcessing;
using Newtonsoft.Json.Linq;

namespace MagicLights.Integration
{
    public class GameEndedBlockHandler : IGameUpdater
    {
        public void Process(Block block, Game game)
        {
            var gameStatus = block.JObject["Game"];
            if (gameStatus == null)
            {
                return;
            }

            if (gameStatus.Value<string>() == "Over")
            {
                game.Events.Dispatch(new GameEnded());
            }
        }
    }
}
