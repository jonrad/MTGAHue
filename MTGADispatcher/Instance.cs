namespace MTGADispatcher
{
    public class Instance
    {
        public Instance(
            int id,
            int cardId,
            int ownerId,
            MagicColor[] colors)
        {
            Id = id;
            CardId = cardId;
            OwnerId = ownerId;
            Colors = colors;
        }

        public int Id { get; }

        public int CardId { get; }

        public int OwnerId { get; }

        public MagicColor[] Colors { get; }
    }
}
