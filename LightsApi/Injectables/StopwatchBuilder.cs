namespace LightsApi.Injectables
{
    public class StopwatchBuilder : IStopwatchBuilder
    {
        public IStopwatch StartNew()
        {
            return new Stopwatch();
        }
    }
}
