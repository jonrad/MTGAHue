using System;

namespace MTGADispatcher
{
    public interface ISubscriptions<TBase>
        where TBase : class
    {
        void Subscribe<T>(Action<T> action)
            where T : TBase;

        void Unsubscribe<T>(Action<T> action)
            where T : TBase;
    }
}
