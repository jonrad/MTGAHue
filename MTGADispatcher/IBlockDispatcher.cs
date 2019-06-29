using MTGADispatcher.Dispatcher;
using System;
using System.Threading.Tasks;

namespace MTGADispatcher
{
    public interface IBlockDispatcher : IDisposable
    {
        ISubscriptions<Block> Subscriptions { get; }

        void Start();

        Task Stop();
    }
}
