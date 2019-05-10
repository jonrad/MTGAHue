namespace MTGADispatcher.BlockProcessing
{
    public interface IGameUpdater
    {
        void Process(Block block, Game game);
    }
}
