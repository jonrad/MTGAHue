namespace MTGADispatcher.Events
{
    public class PlayLand : IMagicEvent
    {
        public PlayLand(Instance instance)
        {
            Instance = instance;
        }

        public Instance Instance { get; }
    }
}
