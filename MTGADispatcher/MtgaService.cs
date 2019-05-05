using System;

namespace MTGADispatcher
{
    public class MtgaService : IDisposable
    {
        private readonly IBlockProcessor blockProcessor;

        private readonly IBlockDispatcher blockDispatcher;

        public MtgaService(
            IBlockDispatcher blockDispatcher,
            IBlockProcessor blockProcessor)
        {
            this.blockDispatcher = blockDispatcher;
            this.blockProcessor = blockProcessor;
        }

        public void Start()
        {
            blockDispatcher.Subscriptions.Subscribe<Block>(blockProcessor.Process);
            blockDispatcher.Start();
        }

        public void Stop()
        {
            blockDispatcher?.Stop();
            blockDispatcher.Subscriptions.Unsubscribe<Block>(blockProcessor.Process);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
