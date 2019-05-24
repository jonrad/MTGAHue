using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MTGADispatcher;

namespace MagicLights.Integration
{
    public class LineReaderProxy : ILineReader
    {
        public static ConcurrentQueue<string> lines = new ConcurrentQueue<string>();

        public void AddText(string text)
        {
            var split = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in split)
            {
                lines.Enqueue(item);
            }
        }

        public void Dispose()
        {
        }

        public Task<string?> ReadLine()
        {
            lines.TryDequeue(out string? result);

            return Task.FromResult<string?>(result);
        }
    }
}
