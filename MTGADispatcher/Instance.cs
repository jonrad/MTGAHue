namespace MTGADispatcher
{
    public class Instance
    {
        public Instance(int id, int cardId, MagicColor[] colors)
        {
            Id = id;
            CardId = cardId;
            Colors = colors;
        }

        public int Id { get; }

        public int CardId { get; }

        public MagicColor[] Colors { get; }
    }
}
