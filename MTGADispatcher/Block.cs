using Newtonsoft.Json.Linq;

namespace MTGADispatcher
{
    public class Block
    {
        public Block(string title, JObject jObject)
        {
            Title = title;
            JObject = jObject;
        }

        public string Title { get; }

        public JObject JObject { get; }
    }
}
