namespace MTGADispatcher
{
    public interface IBlockBuilder
    {
        void Append(string line);

        void Clear();

        (bool, Block) TryBuild();
    }
}
