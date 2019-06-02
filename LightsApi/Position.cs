namespace LightsApi
{
    public struct Position
    {
        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }
    }
}
