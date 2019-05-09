namespace MTGAHue
{
    public struct RGB
    {
        public static RGB Red => new RGB(255, 0, 0);

        public static RGB Green => new RGB(0, 255, 0);

        public static RGB Blue => new RGB(0, 0, 255);

        public static RGB Black => new RGB(0, 0, 0);

        public static RGB White => new RGB(255, 255, 255);

        public static RGB operator *(RGB rgb, double multiplier)
        {
            return new RGB((int)(rgb.R * multiplier), (int)(rgb.G * multiplier), (int)(rgb.B * multiplier));
        }

        public int R { get; set; }

        public int G { get; set; }

        public int B { get; set; }

        public RGB(int r, int g, int b)
        {
            R = r < 0 ? 0 : r;

            G = g < 0 ? 0 : g;
            B = b < 0 ? 0 : b;
        }
    }
}
