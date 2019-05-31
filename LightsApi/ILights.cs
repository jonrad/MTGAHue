namespace LightsApi
{
    public interface ILights
    {
        void Start();

        void Stop();

        ILightLayout AddLayout();

        void RemoveLayout(ILightLayout layout);
    }
}