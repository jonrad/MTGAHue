using Machine.Fakes;
using Machine.Specifications;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using System;
using System.Threading;

namespace MTGADispatcher.Specs.Dispatcher
{
    [Subject(typeof(Dispatcher<>))]
    class DispatcherSpecs : WithSubject<Dispatcher<IMagicEvent>>
    {
        static AutoResetEvent dispatchFinished;

        static int callCount = 0;

        static FakeEvent lastEvent;

        static void OnFakeEvent(FakeEvent fakeEvent)
        {
            callCount++;
            lastEvent = fakeEvent;
        }

        static void OnFinished(FinishedEvent _)
        {
            dispatchFinished.Set();
        }

        static void DispatchSync<T>(T item)
            where T : IMagicEvent
        {
            Subject.Dispatch(item);
            Subject.Dispatch(new FinishedEvent());
            if (!dispatchFinished.WaitOne(TimeSpan.FromSeconds(1)))
            {
                throw new TimeoutException("Events didn't finish dispatching");
            }
        }

        Establish context = () =>
        {
            dispatchFinished = new AutoResetEvent(false);
            callCount = 0;
            lastEvent = null;
            Subject.Subscriptions.Subscribe<FakeEvent>(OnFakeEvent);
            Subject.Subscriptions.Subscribe<FinishedEvent>(OnFinished);
        };

        class when_event_dispatched
        {
            Because of = () =>
                DispatchSync(new FakeEvent { Id = 1 });

            It called_subscribed_action = () =>
            {
                callCount.ShouldEqual(1);
                lastEvent.ShouldNotBeNull();
                lastEvent.Id.ShouldEqual(1);
            };
        }

        class when_uninteresting_event_dispatched
        {
            Because of = () =>
                DispatchSync(new FakeEvent2());

            It did_not_call_action = () =>
                callCount.ShouldEqual(0);
        }

        class when_unsubscribed_then_dispatched
        {
            Establish context = () =>
                Subject.Subscriptions.Unsubscribe<FakeEvent>(OnFakeEvent);

            Because of = () =>
                DispatchSync(new FakeEvent());

            It did_not_call_action = () =>
                callCount.ShouldEqual(0);
        }

        class when_second_event_subscribed
        {
            static bool called = false;

            static void AnotherOnFakeEvent(FakeEvent fakeEvent) =>
                called = true;

            Establish context = () =>
                Subject.Subscriptions.Subscribe<FakeEvent>(AnotherOnFakeEvent);

            Because of = () =>
                DispatchSync(new FakeEvent());

            It called_first_event = () =>
                callCount.ShouldEqual(1);

            It called_second_event = () =>
                called.ShouldEqual(true);
        }

        class FinishedEvent : IMagicEvent
        {
        }

        class FakeEvent : IMagicEvent
        {
            public int Id { get; set; }
        }

        class FakeEvent2 : IMagicEvent
        {
        }
    }
}
