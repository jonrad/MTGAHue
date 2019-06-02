using CUE.NET.Devices.Generic;

namespace LightsApi.Cue
{
    public class LedPosition
    {
        public LedPosition(CorsairLed led, Position position)
        {
            Led = led;
            Position = position;
        }

        public CorsairLed Led { get; }

        public Position Position { get; }
    }
}
