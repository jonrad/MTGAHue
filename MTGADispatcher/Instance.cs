namespace MTGADispatcher
{
    public class Instance
    {
        public Instance(
            int id,
            int cardId,
            int ownerId,
            int controllerId,
            MagicColor[] colors)
        {
            Id = id;
            CardId = cardId;
            OwnerId = ownerId;
            ControllerId = controllerId;
            Colors = colors;
        }

        public int Id { get; }

        public int CardId { get; }

        public int OwnerId { get; }

        public int ControllerId { get; }

        public MagicColor[] Colors { get; }

        public override string ToString()
        {
            return $"[Id={Id} Card={CardId} Owner={OwnerId} Controller={ControllerId} Colors={string.Join(" ", ControllerId)}]";
        }
    }
}
