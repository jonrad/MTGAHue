using System;
using System.Runtime.CompilerServices;

namespace LightsApi
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

        public float R { get; set; }

        public float G { get; set; }

        public float B { get; set; }

        public RGB(float r, float g, float b)
        {
            R = Check(r);
            G = Check(g);
            B = Check(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Check(float value)
        {
            if (value < 0)
            {
                throw new ArgumentException();
            }
            else if (value > 255)
            {
                throw new ArgumentException();
            }

            return value;
        }
    }
}
