using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MTGADispatcher
{
    //TODO: bunch of race conditions here
    public class FileLineReader : ILineReader, IDisposable
    {
        private readonly string path;

        private readonly bool startAtEnd;

        private Stream? stream;

        private StreamReader? reader;

        public FileLineReader(
            string path,
            bool startAtEnd = true)
        {
            this.path = path;
            this.startAtEnd = startAtEnd;
        }

        // This should be an async stream
        public Task<string?> ReadLine()
        {
            if (reader == null)
            {
                TryOpenStream();

                if (reader == null)
                {
                    return Task.FromResult<string?>(null);
                }
            }

            return reader.ReadLineAsync();
        }

        private void TryOpenStream()
        {
            if (!File.Exists(path))
            {
                return;
            }

            stream = File.Open(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            if (startAtEnd)
            {
                stream.Position = stream.Length;
            }

            reader = new StreamReader(stream, Encoding.UTF8, true, 1024, false);
        }

        public void Dispose()
        {
            reader?.Close();
            stream?.Close();
        }
    }
}
