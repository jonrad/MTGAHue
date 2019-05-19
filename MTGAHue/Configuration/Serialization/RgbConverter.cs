using LightsApi;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace MTGAHue.Configuration.Serialization
{
    public class RgbConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RGB);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            if (!int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var intValue))
            {
                throw new ArgumentException();
            }

            var blue = intValue % 256;
            var green = (intValue >> 8) % 256;
            var red = (intValue >> 16) % 256;

            return new RGB(red, green, blue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rgb = (RGB)value;
            var intValue = (((int)rgb.R) << 16) + (((int)rgb.G) << 8) + ((int)rgb.B);

            writer.WriteValue(intValue.ToString("X6"));
        }
    }
}
