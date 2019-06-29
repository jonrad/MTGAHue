using MTGADispatcher.BlockProcessing;
using System;
using System.Threading.Tasks;

namespace MTGADispatcher
{
    public class MtgaService : IMagicService, IDisposable
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

        public async Task Stop()
        {
            blockDispatcher.Subscriptions.Unsubscribe<Block>(blockProcessor.Process);
            await blockDispatcher.Stop();
        }

        public void Dispose()
        {
            var _ = Stop();
        }
    }
}
