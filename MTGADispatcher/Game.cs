using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using System.Collections.Generic;

namespace MTGADispatcher
{
    //TODO this whole thing is just a hack to get this to work
    public class Game
    {
        public Game()
            : this(new Dispatcher<IMagicEvent>())
        {
        }

        public Game(IDispatcher<IMagicEvent> events)
        {
            Events = events;
        }

        public readonly Dictionary<int, Instance> InstancesById
            = new Dictionary<int, Instance>();

        public IDispatcher<IMagicEvent> Events { get; }
    }
}
