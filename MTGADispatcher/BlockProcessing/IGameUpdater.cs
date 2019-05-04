namespace MTGADispatcher
{
    public interface IGameUpdater
    {
        void Process(Block block, Game game);
    }
}
