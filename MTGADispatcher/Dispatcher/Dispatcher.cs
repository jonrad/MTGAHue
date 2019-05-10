using System.Reactive.Subjects;

namespace MTGADispatcher.Dispatcher
{
    public class Dispatcher<TBase> : IDispatcher<TBase>
        where TBase : class
    {
        private Subject<TBase> events =
            new Subject<TBase>();

        public Dispatcher()
        {
            Subscriptions = new Subscriptions<TBase>(events);
        }

        public ISubscriptions<TBase> Subscriptions { get; }

        public void Dispatch<T>(T @event)
            where T : TBase
        {
            events.OnNext(@event);
        }
    }
}
