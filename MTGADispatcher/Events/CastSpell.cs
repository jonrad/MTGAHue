namespace MTGADispatcher.Events
{
    public class CastSpell : IMagicEvent
    {
        public CastSpell(Instance instance)
        {
            Instance = instance;
        }

        public Instance Instance { get; }
    }
}
