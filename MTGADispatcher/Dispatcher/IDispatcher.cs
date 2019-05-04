namespace MTGADispatcher
{
    public interface IDispatcher<TBase>
        where TBase : class
    {
        void Dispatch<T>(T @event)
            where T : TBase;

        ISubscriptions<TBase> Subscriptions { get; }
    }
}
