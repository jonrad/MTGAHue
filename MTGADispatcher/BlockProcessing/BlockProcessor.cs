namespace MTGADispatcher.BlockProcessing
{
    public class BlockProcessor : IBlockProcessor
    {
        private readonly Game game;

        private readonly IGameUpdater[] blockHandlers;

        public BlockProcessor(
            Game game,
            IGameUpdater[] blockHandlers)
        {
            this.game = game;
            this.blockHandlers = blockHandlers;
        }

        public void Process(Block block)
        {
            foreach (var blockHandler in blockHandlers)
            {
                blockHandler.Process(block, game);
            }
        }
    }
}
