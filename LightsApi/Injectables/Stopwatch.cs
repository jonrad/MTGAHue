namespace LightsApi
{
    public class Stopwatch : IStopwatch
    {
        private readonly System.Diagnostics.Stopwatch stopwatch;

        public Stopwatch()
        {
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;
    }
}
