namespace MTGADispatcher
{
    public interface IBlockBuilder
    {
        void Append(string line);

        void Clear();

        Block? TryBuild();
    }
}
