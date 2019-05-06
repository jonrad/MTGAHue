namespace MTGADispatcher
{
    public class Zone
    {
        public Zone(int id, string type, int? ownerId)
        {
            Id = id;
            Type = type;
            OwnerId = ownerId;
        }

        public int Id { get; }

        public string Type { get; }

        public int? OwnerId { get; }

        public bool IsBattlefield() => Type == "Battlefield";
    }
}
