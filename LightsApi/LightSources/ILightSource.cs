namespace LightsApi.LightSources
{
    public interface ILightSource
    {
        RGB Calculate(double x, double y);
    }
}
