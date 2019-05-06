using System.Collections.Generic;

namespace MTGADispatcher.Events
{
    //Occurs when a card is moved from one zone to the next
    //Eg when a card is moved from the deck to the battlefield
    public class ZoneTransfer : IMagicEvent
    {
        private readonly Instance[] instances;

        public ZoneTransfer(
            Zone source,
            Zone destination,
            Instance[] instances)
        {
            Source = source;
            Destination = destination;
            this.instances = instances;
        }

        public IEnumerable<Instance> Instances => instances;

        public Zone Source { get; }

        public Zone Destination { get; }

        public override string ToString()
        {
            return $"Zone Transfer: {string.Join(" ", instances.Select(i => i.ToString()))}";
        }
    }
}
