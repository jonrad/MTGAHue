using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightClient
    {
        Task<ILightLayout> GetLayout();
    }
}
