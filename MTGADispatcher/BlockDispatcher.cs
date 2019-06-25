using MTGADispatcher.Dispatcher;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTGADispatcher
{
    public class BlockDispatcher : IBlockDispatcher
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly IDispatcher<Block> dispatcher;

        private readonly ILineReader lineReader;

        private IBlockBuilder blockBuilder;

        private Task? task;

        public BlockDispatcher(
            IBlockBuilder blockBuilder,
            IDispatcher<Block> dispatcher,
            ILineReader lineReader)
        {
            this.blockBuilder = blockBuilder;
            this.dispatcher = dispatcher;
            this.lineReader = lineReader;
        }

        public ISubscriptions<Block> Subscriptions => dispatcher.Subscriptions;

        public void Start()
        {
            if (task != null)
            {
                throw new InvalidOperationException("Already started");
            }

            task =
                GetBlocks(cancellationTokenSource.Token)
                .ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled);
        }

        public async Task Stop()
        {
            if (task == null)
            {
                return;
            }

            try
            {
                cancellationTokenSource.Cancel();
                await task;
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                task = null;
            }
        }

        private async Task GetBlocks(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var line = await lineReader.ReadLine();
                if (line == null)
                {
                    await Task.Delay(50, cancellationToken);
                }
                else
                {
                    var block = ProcessLine(line.TrimEnd());

                    if (block != null)
                    {
                        dispatcher.Dispatch(block);
                    }
                }
            }
        }

        private Block? ProcessLine(string line)
        {
            if (line.Length == 0)
            {
                return null;
            }

            if (line.StartsWith("[UnityCrossThreadLogger]") || line.StartsWith("[Client GRE]"))
            {
                blockBuilder.Clear();
                blockBuilder.Append(line);
            }
            else if (line.StartsWith("]") || line.StartsWith("}"))
            {
                blockBuilder.Append(line);
                var result = blockBuilder.TryBuild();

                blockBuilder.Clear();

                return result;
            }
            else
            {
                blockBuilder.Append(line);
            }

            return null;
        }

        public void Dispose()
        {
            var _ = Stop();
        }
    }
}
