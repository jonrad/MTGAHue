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

        private Task task;

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

        public void Stop()
        {
            if (task == null)
            {
                return;
            }

            try
            {
                cancellationTokenSource.Cancel();
                task.Wait();
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
                    var (sucess, block) = ProcessLine(line.TrimEnd());

                    if (sucess)
                    {
                        dispatcher.Dispatch(block);
                    }
                }
            }
        }

        private (bool, Block) ProcessLine(string line)
        {
            if (line.Length == 0)
            {
                return (false, null);
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

            return (false, null);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
