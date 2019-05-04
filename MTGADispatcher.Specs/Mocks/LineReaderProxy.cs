using System.Threading;
using System.Threading.Tasks;

namespace MTGADispatcher.Specs.Mocks
{
    public class LineReaderProxy : ILineReader
    {
        private string line = null;

        public AutoResetEvent LineRead = new AutoResetEvent(false);

        public bool Disposed { get; private set; }

        public Task<string> ReadLine()
        {
            if (line != null)
            {
                var result = line;
                line = null;
                return Task.FromResult(result);
            }
            else
            {
                LineRead.Set();
                return Task.FromResult<string>(null);
            }
        }

        public void SetResult(string text)
        {
            line = text;
            LineRead.Reset();
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
