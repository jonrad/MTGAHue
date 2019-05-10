using MTGADispatcher.Dispatcher;
using System;

namespace MTGADispatcher
{
    public interface IBlockDispatcher : IDisposable
    {
        ISubscriptions<Block> Subscriptions { get; }

        void Start();

        void Stop();
    }
}
