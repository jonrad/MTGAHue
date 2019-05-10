using System;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MTGADispatcher.Dispatcher
{
    public class Subscriptions<TBase> : ISubscriptions<TBase>
        where TBase : class
    {
        private ConcurrentDictionary<Delegate, IDisposable> delegateSubscriptionMap
            = new ConcurrentDictionary<Delegate, IDisposable>();

        private IConnectableObservable<TBase> subscriptions;

        internal Subscriptions(Subject<TBase> events)
        {
            subscriptions = events.ObserveOn(Scheduler.Default).Publish();
            subscriptions.Connect();
        }

        public void Subscribe<T>(Action<T> action)
            where T : TBase
        {
            var disposable = subscriptions.OfType<T>().Subscribe(action);
            delegateSubscriptionMap.TryAdd(action, disposable);
        }

        public void Unsubscribe<T>(Action<T> action)
            where T : TBase
        {
            if (delegateSubscriptionMap.TryGetValue(action, out var value))
            {
                value.Dispose();
            }
        }
    }
}
