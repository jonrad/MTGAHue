using Newtonsoft.Json.Linq;

namespace MTGADispatcher
{
    public interface IInstanceBuilder
    {
        Instance Build(JToken gameObject);
    }
}
