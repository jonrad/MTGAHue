namespace LightsApi
{
    public interface ILights
    {
        void Start();

        void Stop();

        ILayer AddLayer();

        void RemoveLayer(ILayer layer);
    }
}
